using k8s.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <summary>
        /// Gets a function which returns a <see cref="WebSocketBuilder"/> which <see cref="Kubernetes"/> will use to
        /// create a new <see cref="WebSocket"/> connection to the Kubernetes cluster.
        /// </summary>
        public Func<WebSocketBuilder> CreateWebSocketBuilder { get; set; } = () => new WebSocketBuilder();

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default",
            string command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true,
            bool tty = true, string webSocketSubProtol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            return WebSocketNamespacedPodExecAsync(name, @namespace, new string[] { command }, container, stderr, stdin,
                stdout, tty, webSocketSubProtol, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IStreamDemuxer> MuxedStreamNamespacedPodExecAsync(
            string name,
            string @namespace = "default", IEnumerable<string> command = null, string container = null,
            bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true,
            string webSocketSubProtol = WebSocketProtocol.V4BinaryWebsocketProtocol,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var webSocket = await WebSocketNamespacedPodExecAsync(name, @namespace,
                    command, container, tty: tty, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var muxer = new StreamDemuxer(webSocket);
            return muxer;
        }

        /// <inheritdoc/>
        public virtual Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default",
            IEnumerable<string> command = null, string container = null, bool stderr = true, bool stdin = true,
            bool stdout = true, bool tty = true,
            string webSocketSubProtol = WebSocketProtocol.V4BinaryWebsocketProtocol,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (!command.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            var commandArray = command.ToArray();
            foreach (var c in commandArray)
            {
                if (c.Length > 0 && c[0] == 0xfeff)
                {
                    throw new InvalidOperationException(
                        $"Detected an attempt to execute a command which starts with a Unicode byte order mark (BOM). This is probably incorrect. The command was {c}");
                }
            }

            // Tracing
            var shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                var tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("command", command);
                tracingParameters.Add("container", container);
                tracingParameters.Add("name", name);
                tracingParameters.Add("namespace", @namespace);
                tracingParameters.Add("stderr", stderr);
                tracingParameters.Add("stdin", stdin);
                tracingParameters.Add("stdout", stdout);
                tracingParameters.Add("tty", tty);
                tracingParameters.Add("webSocketSubProtol", webSocketSubProtol);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, nameof(WebSocketNamespacedPodExecAsync),
                    tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri);
            uriBuilder.Scheme = BaseUri.Scheme == "https" ? "wss" : "ws";

            if (!uriBuilder.Path.EndsWith("/", StringComparison.InvariantCulture))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += $"api/v1/namespaces/{@namespace}/pods/{name}/exec";

            var query = new StringBuilder();

            foreach (var c in command)
            {
                Utilities.AddQueryParameter(query, "command", c);
            }

            if (!string.IsNullOrEmpty(container))
            {
                Utilities.AddQueryParameter(query, "container", container);
            }

            query.Append("&stderr=")
                .Append(stderr
                    ? '1'
                    : '0'); // the query string is guaranteed not to be empty here because it has a 'command' param
            query.Append("&stdin=").Append(stdin ? '1' : '0');
            query.Append("&stdout=").Append(stdout ? '1' : '0');
            query.Append("&tty=").Append(tty ? '1' : '0');
            uriBuilder.Query =
                query.ToString(1, query.Length - 1); // UriBuilder.Query doesn't like leading '?' chars, so trim it

            return StreamConnectAsync(uriBuilder.Uri, invocationId, webSocketSubProtol, customHeaders,
                cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodPortForwardAsync(string name, string @namespace,
            IEnumerable<int> ports, string webSocketSubProtocol = null,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            if (ports == null)
            {
                throw new ArgumentNullException(nameof(ports));
            }

            // Tracing
            var shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                var tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("name", name);
                tracingParameters.Add("@namespace", @namespace);
                tracingParameters.Add("ports", ports);
                tracingParameters.Add("webSocketSubProtocol", webSocketSubProtocol);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, nameof(WebSocketNamespacedPodPortForwardAsync),
                    tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri);
            uriBuilder.Scheme = BaseUri.Scheme == "https" ? "wss" : "ws";

            if (!uriBuilder.Path.EndsWith("/", StringComparison.InvariantCulture))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += $"api/v1/namespaces/{@namespace}/pods/{name}/portforward";

            var q = new StringBuilder();
            foreach (var port in ports)
            {
                if (q.Length != 0)
                {
                    q.Append('&');
                }

                q.Append("ports=").Append(port.ToString(CultureInfo.InvariantCulture));
            }

            uriBuilder.Query = q.ToString();

            return StreamConnectAsync(uriBuilder.Uri, invocationId, webSocketSubProtocol, customHeaders,
                cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodAttachAsync(string name, string @namespace,
            string container = default, bool stderr = true, bool stdin = false, bool stdout = true,
            bool tty = false, string webSocketSubProtol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (@namespace == null)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            // Tracing
            var shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                var tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("container", container);
                tracingParameters.Add("name", name);
                tracingParameters.Add("namespace", @namespace);
                tracingParameters.Add("stderr", stderr);
                tracingParameters.Add("stdin", stdin);
                tracingParameters.Add("stdout", stdout);
                tracingParameters.Add("tty", tty);
                tracingParameters.Add("webSocketSubProtol", webSocketSubProtol);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, nameof(WebSocketNamespacedPodAttachAsync),
                    tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri);
            uriBuilder.Scheme = BaseUri.Scheme == "https" ? "wss" : "ws";

            if (!uriBuilder.Path.EndsWith("/", StringComparison.InvariantCulture))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += $"api/v1/namespaces/{@namespace}/pods/{name}/attach";

            var query = new StringBuilder();
            query.Append("?stderr=").Append(stderr ? '1' : '0');
            query.Append("&stdin=").Append(stdin ? '1' : '0');
            query.Append("&stdout=").Append(stdout ? '1' : '0');
            query.Append("&tty=").Append(tty ? '1' : '0');
            Utilities.AddQueryParameter(query, "container", container);
            uriBuilder.Query =
                query.ToString(1, query.Length - 1); // UriBuilder.Query doesn't like leading '?' chars, so trim it

            return StreamConnectAsync(uriBuilder.Uri, invocationId, webSocketSubProtol, customHeaders,
                cancellationToken);
        }

        protected async Task<WebSocket> StreamConnectAsync(Uri uri, string invocationId = null,
            string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var shouldTrace = ServiceClientTracing.IsEnabled;

            // Create WebSocket transport objects
            var webSocketBuilder = CreateWebSocketBuilder();

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    webSocketBuilder.SetRequestHeader(header.Key, string.Join(" ", header.Value));
                }
            }

            // Set Credentials
            foreach (var cert in this.HttpClientHandler.ClientCertificates.OfType<X509Certificate2>())
            {
                webSocketBuilder.AddClientCertificate(cert);
            }

            if (Credentials != null)
            {
                // Copy the default (credential-related) request headers from the HttpClient to the WebSocket
                var message = new HttpRequestMessage();
                await Credentials.ProcessHttpRequestAsync(message, cancellationToken).ConfigureAwait(false);

                foreach (var header in message.Headers)
                {
                    webSocketBuilder.SetRequestHeader(header.Key, string.Join(" ", header.Value));
                }
            }

            if (this.CaCerts != null)
            {
                webSocketBuilder.ExpectServerCertificate(this.CaCerts);
            }

            if (this.SkipTlsVerify)
            {
                webSocketBuilder.SkipServerCertificateValidation();
            }

            if (webSocketSubProtocol != null)
            {
                webSocketBuilder.Options.AddSubProtocol(webSocketSubProtocol);
            }

            // Send Request
            cancellationToken.ThrowIfCancellationRequested();

            WebSocket webSocket = null;
            try
            {
                webSocket = await webSocketBuilder.BuildAndConnectAsync(uri, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (WebSocketException wse) when (wse.WebSocketErrorCode == WebSocketError.HeaderError ||
                                                 (wse.InnerException is WebSocketException &&
                                                 ((WebSocketException)wse.InnerException).WebSocketErrorCode ==
                                                 WebSocketError.HeaderError))
            {
                // This usually indicates the server sent an error message, like 400 Bad Request. Unfortunately, the WebSocket client
                // class doesn't give us a lot of information about what went wrong. So, retry the connection.
                var uriBuilder = new UriBuilder(uri);
                uriBuilder.Scheme = uri.Scheme == "wss" ? "https" : "http";

                var response = await HttpClient.GetAsync(uriBuilder.Uri, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.SwitchingProtocols)
                {
                    // This should never happen - the server just allowed us to switch to WebSockets but the previous call didn't work.
                    // Rethrow the original exception
                    response.Dispose();
                    throw;
                }
                else
                {
#if NET5_0
                    var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
                    // Try to parse the content as a V1Status object
                    var genericObject = SafeJsonConvert.DeserializeObject<KubernetesObject>(content);
                    V1Status status = null;

                    if (genericObject.ApiVersion == "v1" && genericObject.Kind == "Status")
                    {
                        status = SafeJsonConvert.DeserializeObject<V1Status>(content);
                    }

                    var ex =
                        new HttpOperationException(
                            $"The operation returned an invalid status code: {response.StatusCode}", wse)
                        {
                            Response = new HttpResponseMessageWrapper(response, content),
                            Body = status != null ? (object)status : content,
                        };

                    response.Dispose();

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                if (shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                throw;
            }
            finally
            {
                if (shouldTrace)
                {
                    ServiceClientTracing.Exit(invocationId, null);
                }
            }

            return webSocket;
        }
    }
}
