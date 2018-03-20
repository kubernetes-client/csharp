/*
 * This (temporary) code has been adapted from Microsoft's .NET Core 2.0.4 codebase. Original code copyright (c) .NET Foundation and Contributors.
 * Hopefully, once .NET Core 2.1 lands, we can drop it in favour of the built-in ManagedWebSocket and SocketHttpHandler classes (providing they support custom validation of server certificates).
 *
 * Original code: https://github.com/dotnet/corefx/blob/v2.0.4/src/System.Net.WebSockets.Client/src/System/Net/WebSockets/WebSocketHandle.Managed.cs#L74
 * License: https://github.com/dotnet/corefx/blob/v2.0.4/LICENSE.TXT
 *
 */

#if NETCOREAPP2_1

using k8s;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreFX
{
    /// <summary>
    ///     Connection factory for Kubernetes web sockets.
    /// </summary>
    internal static class K8sWebSocket
    {
        /// <summary>
        ///     GUID appended by the server as part of the security key response.
        ///
        ///     Defined in the RFC.
        /// </summary>
        const string WSServerGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        /// <summary>
        ///     Asynchronously connect to a Kubernetes WebSocket.
        /// </summary>
        /// <param name="uri">
        ///     The target URI.
        /// </param>
        /// <param name="options">
        ///     <see cref="KubernetesWebSocketOptions"/> that control the WebSocket's configuration and connection process.
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A <see cref="WebSocket"/> representing the connection.
        /// </returns>
        public static async Task<WebSocket> ConnectAsync(Uri uri, KubernetesWebSocketOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Connect to the remote server
                Socket connectedSocket = await ConnectSocketAsync(uri.Host, uri.Port, cancellationToken).ConfigureAwait(false);
                Stream stream = new NetworkStream(connectedSocket, ownsSocket: true);

                // Upgrade to SSL if needed
                if (uri.Scheme == "wss")
                {
                    X509Certificate2Collection clientCertificates = new X509Certificate2Collection();
                    foreach (X509Certificate2 clientCertificate in options.ClientCertificates)
                        clientCertificates.Add(clientCertificate);

                    var sslStream = new SslStream(
                        innerStream: stream,
                        leaveInnerStreamOpen: false,
                        userCertificateValidationCallback: options.ServerCertificateCustomValidationCallback
                    );
                    await
                        sslStream.AuthenticateAsClientAsync(
                            uri.Host,
                            clientCertificates,
                            options.EnabledSslProtocols,
                            checkCertificateRevocation: false
                        )
                        .ConfigureAwait(false);

                    stream = sslStream;
                }

                // Create the security key and expected response, then build all of the request headers
                (string secKey, string webSocketAccept) = CreateSecKeyAndSecWebSocketAccept();
                byte[] requestHeader = BuildRequestHeader(uri, options, secKey);

                // Write out the header to the connection
                await stream.WriteAsync(requestHeader, 0, requestHeader.Length, cancellationToken).ConfigureAwait(false);

                // Parse the response and store our state for the remainder of the connection
                string subprotocol = await ParseAndValidateConnectResponseAsync(stream, options, webSocketAccept, cancellationToken).ConfigureAwait(false);

                return WebSocket.CreateClientWebSocket(
                    stream,
                    subprotocol,
                    options.ReceiveBufferSize,
                    options.SendBufferSize,
                    options.KeepAliveInterval,
                    false,
                    WebSocket.CreateClientBuffer(options.ReceiveBufferSize, options.SendBufferSize)
                );
            }
            catch (Exception unexpectedError)
            {
                throw new WebSocketException("WebSocket connection failure.", unexpectedError);
            }
        }

        /// <summary>Connects a socket to the specified host and port, subject to cancellation and aborting.</summary>
        /// <param name="host">The host to which to connect.</param>
        /// <param name="port">The port to which to connect on the host.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        /// <returns>The connected Socket.</returns>
        private static async Task<Socket> ConnectSocketAsync(string host, int port, CancellationToken cancellationToken)
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);

            ExceptionDispatchInfo lastException = null;
            foreach (IPAddress address in addresses)
            {
                var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    using (cancellationToken.Register(() => socket.Dispose()))
                    {
                        try
                        {
                            await socket.ConnectAsync(address, port).ConfigureAwait(false);
                        }
                        catch (ObjectDisposedException objectDisposed)
                        {
                            // If the socket was disposed because cancellation was requested, translate the exception
                            // into a new OperationCanceledException.  Otherwise, let the original ObjectDisposedexception propagate.
                            if (cancellationToken.IsCancellationRequested)
                            {
                                throw new OperationCanceledException(new OperationCanceledException().Message, objectDisposed, cancellationToken);
                            }
                        }
                    }
                    cancellationToken.ThrowIfCancellationRequested(); // in case of a race and socket was disposed after the await

                    return socket;
                }
                catch (Exception exc)
                {
                    socket.Dispose();
                    lastException = ExceptionDispatchInfo.Capture(exc);
                }
            }

            lastException?.Throw();

            Debug.Fail("We should never get here. We should have already returned or an exception should have been thrown.");
            throw new WebSocketException("WebSocket connection failure.");
        }

        /// <summary>Creates a byte[] containing the headers to send to the server.</summary>
        /// <param name="uri">The Uri of the server.</param>
        /// <param name="options">The options used to configure the websocket.</param>
        /// <param name="secKey">The generated security key to send in the Sec-WebSocket-Key header.</param>
        /// <returns>The byte[] containing the encoded headers ready to send to the network.</returns>
        private static byte[] BuildRequestHeader(Uri uri, KubernetesWebSocketOptions options, string secKey)
        {
            StringBuilder builder = new StringBuilder()
                .Append("GET ")
                .Append(uri.PathAndQuery)
                .Append(" HTTP/1.1\r\n");

            // Add all of the required headers, honoring Host header if set.
            string hostHeader;
            if (!options.RequestHeaders.TryGetValue(HttpKnownHeaderNames.Host, out hostHeader))
                hostHeader = uri.Host;

            builder.Append("Host: ");
            if (String.IsNullOrEmpty(hostHeader))
            {
                builder.Append(uri.IdnHost).Append(':').Append(uri.Port).Append("\r\n");
            }
            else
            {
                builder.Append(hostHeader).Append("\r\n");
            }

            builder.Append("Connection: Upgrade\r\n");
            builder.Append("Upgrade: websocket\r\n");
            builder.Append("Sec-WebSocket-Version: 13\r\n");
            builder.Append("Sec-WebSocket-Key: ").Append(secKey).Append("\r\n");

            // Add all of the additionally requested headers
            foreach (string key in options.RequestHeaders.Keys)
            {
                if (String.Equals(key, HttpKnownHeaderNames.Host, StringComparison.OrdinalIgnoreCase))
                {
                    // Host header handled above
                    continue;
                }

                builder.Append(key).Append(": ").Append(options.RequestHeaders[key]).Append("\r\n");
            }

            // Add the optional subprotocols header
            if (options.RequestedSubProtocols.Count > 0)
            {
                builder.Append(HttpKnownHeaderNames.SecWebSocketProtocol).Append(": ");
                builder.Append(options.RequestedSubProtocols[0]);
                for (int i = 1; i < options.RequestedSubProtocols.Count; i++)
                {
                    builder.Append(", ").Append(options.RequestedSubProtocols[i]);
                }
                builder.Append("\r\n");
            }

            // End the headers
            builder.Append("\r\n");

            // Return the bytes for the built up header
            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        /// <summary>Read and validate the connect response headers from the server.</summary>
        /// <param name="stream">The stream from which to read the response headers.</param>
        /// <param name="options">The options used to configure the websocket.</param>
        /// <param name="expectedSecWebSocketAccept">The expected value of the Sec-WebSocket-Accept header.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        /// <returns>The agreed upon subprotocol with the server, or null if there was none.</returns>
        static async Task<string> ParseAndValidateConnectResponseAsync(Stream stream, KubernetesWebSocketOptions options, string expectedSecWebSocketAccept, CancellationToken cancellationToken)
        {
            // Read the first line of the response
            string statusLine = await ReadResponseHeaderLineAsync(stream, cancellationToken).ConfigureAwait(false);

            // Depending on the underlying sockets implementation and timing, connecting to a server that then
            // immediately closes the connection may either result in an exception getting thrown from the connect
            // earlier, or it may result in getting to here but reading 0 bytes.  If we read 0 bytes and thus have
            // an empty status line, treat it as a connect failure.
            if (String.IsNullOrEmpty(statusLine))
            {
                throw new WebSocketException("Connection failure.");
            }

            const string ExpectedStatusStart = "HTTP/1.1 ";
            const string ExpectedStatusStatWithCode = "HTTP/1.1 101"; // 101 == SwitchingProtocols

            // If the status line doesn't begin with "HTTP/1.1" or isn't long enough to contain a status code, fail.
            if (!statusLine.StartsWith(ExpectedStatusStart, StringComparison.Ordinal) || statusLine.Length < ExpectedStatusStatWithCode.Length)
            {
                throw new WebSocketException(WebSocketError.HeaderError);
            }

            // If the status line doesn't contain a status code 101, or if it's long enough to have a status description
            // but doesn't contain whitespace after the 101, fail.
            if (!statusLine.StartsWith(ExpectedStatusStatWithCode, StringComparison.Ordinal) ||
                (statusLine.Length > ExpectedStatusStatWithCode.Length && !char.IsWhiteSpace(statusLine[ExpectedStatusStatWithCode.Length])))
            {
                throw new WebSocketException(WebSocketError.HeaderError, $"Connection failure (status line = '{statusLine}').");
            }

            // Read each response header. Be liberal in parsing the response header, treating
            // everything to the left of the colon as the key and everything to the right as the value, trimming both.
            // For each header, validate that we got the expected value.
            bool foundUpgrade = false, foundConnection = false, foundSecWebSocketAccept = false;
            string subprotocol = null;
            string line;
            while (!String.IsNullOrEmpty(line = await ReadResponseHeaderLineAsync(stream, cancellationToken).ConfigureAwait(false)))
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex == -1)
                {
                    throw new WebSocketException(WebSocketError.HeaderError);
                }

                string headerName = line.SubstringTrim(0, colonIndex);
                string headerValue = line.SubstringTrim(colonIndex + 1);

                // The Connection, Upgrade, and SecWebSocketAccept headers are required and with specific values.
                ValidateAndTrackHeader(HttpKnownHeaderNames.Connection, "Upgrade", headerName, headerValue, ref foundConnection);
                ValidateAndTrackHeader(HttpKnownHeaderNames.Upgrade, "websocket", headerName, headerValue, ref foundUpgrade);
                ValidateAndTrackHeader(HttpKnownHeaderNames.SecWebSocketAccept, expectedSecWebSocketAccept, headerName, headerValue, ref foundSecWebSocketAccept);

                // The SecWebSocketProtocol header is optional.  We should only get it with a non-empty value if we requested subprotocols,
                // and then it must only be one of the ones we requested.  If we got a subprotocol other than one we requested (or if we
                // already got one in a previous header), fail. Otherwise, track which one we got.
                if (String.Equals(HttpKnownHeaderNames.SecWebSocketProtocol, headerName, StringComparison.OrdinalIgnoreCase) &&
                    !String.IsNullOrWhiteSpace(headerValue))
                {
                    if (options.RequestedSubProtocols.Count > 0)
                    {
                        string newSubprotocol = options.RequestedSubProtocols.Find(requested => String.Equals(requested, headerValue, StringComparison.OrdinalIgnoreCase));
                        if (newSubprotocol == null || subprotocol != null)
                        {
                            throw new WebSocketException(
                                String.Format("Unsupported sub-protocol '{0}' (expected one of [{1}]).",
                                    newSubprotocol,
                                    String.Join(", ", options.RequestedSubProtocols)
                                )
                            );
                        }
                        subprotocol = newSubprotocol;
                    }
                }
            }
            if (!foundUpgrade || !foundConnection || !foundSecWebSocketAccept)
            {
                throw new WebSocketException("Connection failure.");
            }

            return subprotocol;
        }

        /// <summary>Validates a received header against expected values and tracks that we've received it.</summary>
        /// <param name="targetHeaderName">The header name against which we're comparing.</param>
        /// <param name="targetHeaderValue">The header value against which we're comparing.</param>
        /// <param name="foundHeaderName">The actual header name received.</param>
        /// <param name="foundHeaderValue">The actual header value received.</param>
        /// <param name="foundHeader">A bool tracking whether this header has been seen.</param>
        private static void ValidateAndTrackHeader(
            string targetHeaderName, string targetHeaderValue,
            string foundHeaderName, string foundHeaderValue,
            ref bool foundHeader)
        {
            bool isTargetHeader = String.Equals(targetHeaderName, foundHeaderName, StringComparison.OrdinalIgnoreCase);
            if (!foundHeader)
            {
                if (isTargetHeader)
                {
                    if (!String.Equals(targetHeaderValue, foundHeaderValue, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new WebSocketException(
                            $"Invalid value for '{foundHeaderName}' header: '{foundHeaderValue}' (expected '{targetHeaderValue}')."
                        );
                    }
                    foundHeader = true;
                }
            }
            else
            {
                if (isTargetHeader)
                {
                    throw new WebSocketException("Connection failure.");
                }
            }
        }

        /// <summary>Reads a line from the stream.</summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="cancellationToken">The CancellationToken used to cancel the websocket.</param>
        /// <returns>The read line, or null if none could be read.</returns>
        private static async Task<string> ReadResponseHeaderLineAsync(Stream stream, CancellationToken cancellationToken)
        {
            StringBuilder sb = new StringBuilder();

            var arr = new byte[1];
            char prevChar = '\0';
            try
            {
                // TODO: Reading one byte is extremely inefficient.  The problem, however,
                // is that if we read multiple bytes, we could end up reading bytes post-headers
                // that are part of messages meant to be read by the managed websocket after
                // the connection.  The likely solution here is to wrap the stream in a BufferedStream,
                // though a) that comes at the expense of an extra set of virtual calls, b)
                // it adds a buffer when the managed websocket will already be using a buffer, and
                // c) it's not exposed on the version of the System.IO contract we're currently using.
                while (await stream.ReadAsync(arr, 0, 1, cancellationToken).ConfigureAwait(false) == 1)
                {
                    // Process the next char
                    char curChar = (char)arr[0];
                    if (prevChar == '\r' && curChar == '\n')
                    {
                        break;
                    }
                    sb.Append(curChar);
                    prevChar = curChar;
                }

                if (sb.Length > 0 && sb[sb.Length - 1] == '\r')
                {
                    sb.Length = sb.Length - 1;
                }

                return sb.ToString();
            }
            finally
            {
                sb.Clear();
            }
        }

        /// <summary>
        ///     Create a security key for sending in the Sec-WebSocket-Key header and the associated response we expect to receive as the Sec-WebSocket-Accept header value.
        /// </summary>
        /// <returns>A key-value pair of the request header security key and expected response header value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA5350", Justification = "Required by RFC6455")]
        static (string secKey, string expectedResponse) CreateSecKeyAndSecWebSocketAccept()
        {
            string secKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            using (SHA1 sha = SHA1.Create())
            {
                return (
                    secKey,
                    Convert.ToBase64String(
                        sha.ComputeHash(Encoding.ASCII.GetBytes(secKey + WSServerGuid))
                    )
                );
            }
        }

        static void ValidateHeader(HttpHeaders headers, string name, string expectedValue)
        {
            if (!headers.TryGetValues(name, out IEnumerable<string> values))
                ThrowConnectFailure();

            Debug.Assert(values is string[]);
            string[] array = (string[])values;
            if (array.Length != 1 || !String.Equals(array[0], expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                throw new WebSocketException(
                    $"Invalid WebSocker response header '{name}': [{String.Join(", ", array)}]"
                );
            }
        }

        static void ThrowConnectFailure() => throw new WebSocketException("Connection failure.");
    }

    /// <summary>
    ///     Well-known HTTP header names from CoreFX used by <see cref="K8sWebSocket"/>.
    /// </summary>
    static class HttpKnownHeaderNames
    {
        public const string Accept = "Accept";
        public const string AcceptCharset = "Accept-Charset";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptLanguage = "Accept-Language";
        public const string AcceptPatch = "Accept-Patch";
        public const string AcceptRanges = "Accept-Ranges";
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
        public const string AccessControlMaxAge = "Access-Control-Max-Age";
        public const string Age = "Age";
        public const string Allow = "Allow";
        public const string AltSvc = "Alt-Svc";
        public const string Authorization = "Authorization";
        public const string CacheControl = "Cache-Control";
        public const string Connection = "Connection";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLanguage = "Content-Language";
        public const string ContentLength = "Content-Length";
        public const string ContentLocation = "Content-Location";
        public const string ContentMD5 = "Content-MD5";
        public const string ContentRange = "Content-Range";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string ContentType = "Content-Type";
        public const string Cookie = "Cookie";
        public const string Cookie2 = "Cookie2";
        public const string Date = "Date";
        public const string ETag = "ETag";
        public const string Expect = "Expect";
        public const string Expires = "Expires";
        public const string From = "From";
        public const string Host = "Host";
        public const string IfMatch = "If-Match";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";
        public const string IfRange = "If-Range";
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        public const string KeepAlive = "Keep-Alive";
        public const string LastModified = "Last-Modified";
        public const string Link = "Link";
        public const string Location = "Location";
        public const string MaxForwards = "Max-Forwards";
        public const string Origin = "Origin";
        public const string P3P = "P3P";
        public const string Pragma = "Pragma";
        public const string ProxyAuthenticate = "Proxy-Authenticate";
        public const string ProxyAuthorization = "Proxy-Authorization";
        public const string ProxyConnection = "Proxy-Connection";
        public const string PublicKeyPins = "Public-Key-Pins";
        public const string Range = "Range";
        public const string Referer = "Referer"; // NB: The spelling-mistake "Referer" for "Referrer" must be matched.
        public const string RetryAfter = "Retry-After";
        public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
        public const string SecWebSocketExtensions = "Sec-WebSocket-Extensions";
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        public const string SecWebSocketProtocol = "Sec-WebSocket-Protocol";
        public const string SecWebSocketVersion = "Sec-WebSocket-Version";
        public const string Server = "Server";
        public const string SetCookie = "Set-Cookie";
        public const string SetCookie2 = "Set-Cookie2";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
        public const string TE = "TE";
        public const string TSV = "TSV";
        public const string Trailer = "Trailer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string Upgrade = "Upgrade";
        public const string UpgradeInsecureRequests = "Upgrade-Insecure-Requests";
        public const string UserAgent = "User-Agent";
        public const string Vary = "Vary";
        public const string Via = "Via";
        public const string WWWAuthenticate = "WWW-Authenticate";
        public const string Warning = "Warning";
        public const string XAspNetVersion = "X-AspNet-Version";
        public const string XContentDuration = "X-Content-Duration";
        public const string XContentTypeOptions = "X-Content-Type-Options";
        public const string XFrameOptions = "X-Frame-Options";
        public const string XMSEdgeRef = "X-MSEdge-Ref";
        public const string XPoweredBy = "X-Powered-By";
        public const string XRequestID = "X-Request-ID";
        public const string XUACompatible = "X-UA-Compatible";
    }

    /// <summary>
    ///     Extension methods for <see cref="String"/>s from the CoreFX codebase (used by <see cref="K8sWebSocket"/>).
    /// </summary>
    static class CoreFXStringExtensions
    {
        public static string SubstringTrim(this string value, int startIndex)
        {
            return SubstringTrim(value, startIndex, value.Length - startIndex);
        }

        public static string SubstringTrim(this string value, int startIndex, int length)
        {
            Debug.Assert(value != null, "string must be non-null");
            Debug.Assert(startIndex >= 0, "startIndex must be non-negative");
            Debug.Assert(length >= 0, "length must be non-negative");
            Debug.Assert(startIndex <= value.Length - length, "startIndex + length must be <= value.Length");

            if (length == 0)
            {
                return String.Empty;
            }

            int endIndex = startIndex + length - 1;

            while (startIndex <= endIndex && char.IsWhiteSpace(value[startIndex]))
            {
                startIndex++;
            }

            while (endIndex >= startIndex && char.IsWhiteSpace(value[endIndex]))
            {
                endIndex--;
            }

            int newLength = endIndex - startIndex + 1;
            Debug.Assert(newLength >= 0 && newLength <= value.Length, "Expected resulting length to be within value's length");

            return
                newLength == 0 ? String.Empty :
                newLength == value.Length ? value :
                value.Substring(startIndex, newLength);
        }
    }
}

#endif // NETCOREAPP2_1
