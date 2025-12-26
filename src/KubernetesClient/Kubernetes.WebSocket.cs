using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default",
            string command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true,
            bool tty = true, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            return WebSocketNamespacedPodExecAsync(name, @namespace, new string[] { command }, container, stderr, stdin,
                stdout, tty, webSocketSubProtocol, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IStreamDemuxer> MuxedStreamNamespacedPodExecAsync(
            string name,
            string @namespace = "default", IEnumerable<string> command = null, string container = null,
            bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true,
            string webSocketSubProtocol = WebSocketProtocol.V4BinaryWebsocketProtocol,
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
            string webSocketSubProtocol = WebSocketProtocol.V4BinaryWebsocketProtocol,
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

            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri)
            {
                Scheme = BaseUri.Scheme,
            };

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

            return StreamConnectAsync(uriBuilder.Uri, webSocketSubProtocol, customHeaders,
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


            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri)
            {
                Scheme = BaseUri.Scheme,
            };

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

            return StreamConnectAsync(uriBuilder.Uri, webSocketSubProtocol, customHeaders,
                cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodAttachAsync(string name, string @namespace,
            string container = default, bool stderr = true, bool stdin = false, bool stdout = true,
            bool tty = false, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null,
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

            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri)
            {
                Scheme = BaseUri.Scheme,
            };

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

            return StreamConnectAsync(uriBuilder.Uri, webSocketSubProtocol, customHeaders,
                cancellationToken);
        }

        partial void BeforeRequest();
        partial void AfterRequest();

        protected async Task<WebSocket> StreamConnectAsync(Uri uri, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var requestStream = new ProducerConsumerStream();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionExact,
                Content = new DuplexStreamContent(requestStream),
            };

            if (!string.IsNullOrWhiteSpace(TlsServerName))
            {
                requestMessage.Headers.Host = TlsServerName;
            }

            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    requestMessage.Headers.Remove(header.Key);
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (Credentials != null)
            {
                await Credentials.ProcessHttpRequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(webSocketSubProtocol))
            {
                requestMessage.Headers.TryAddWithoutValidation("X-Stream-Protocol-Version", webSocketSubProtocol);
            }

            cancellationToken.ThrowIfCancellationRequested();

            HttpResponseMessage response = null;
            Stream responseStream = null;
            try
            {
                BeforeRequest();
                response = await HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
#if NET5_0_OR_GREATER
                    var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
                    var genericObject = KubernetesJson.Deserialize<KubernetesObject>(content);
                    V1Status status = null;

                    if (genericObject.ApiVersion == "v1" && genericObject.Kind == "Status")
                    {
                        status = KubernetesJson.Deserialize<V1Status>(content);
                    }

                    var ex = new HttpOperationException($"The operation returned an invalid status code: {response.StatusCode}")
                    {
                        Response = new HttpResponseMessageWrapper(response, content),
                        Body = status != null ? status : content,
                    };

                    response.Dispose();
                    throw ex;
                }

#if NET5_0_OR_GREATER
                responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
                responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif

                var webSocket = new Http2WebSocket(requestStream, responseStream, response, webSocketSubProtocol);
                response = null;
                responseStream = null;

                return webSocket;
            }
            catch (Exception ex)
            {
                try
                {
                    requestStream?.Complete();
                }
                catch (Exception cleanupEx)
                {
                    throw new AggregateException(ex, cleanupEx);
                }
                throw;
            }
            finally
            {
                responseStream?.Dispose();
                response?.Dispose();
                AfterRequest();
            }
        }
    }
}
