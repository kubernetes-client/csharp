using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

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
        public Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default", string command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WebSocketNamespacedPodExecAsync(name, @namespace, new string[] { command }, container, stderr, stdin, stdout, tty, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default", IEnumerable<string> command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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

            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("command", command);
                tracingParameters.Add("container", container);
                tracingParameters.Add("name", name);
                tracingParameters.Add("namespace", @namespace);
                tracingParameters.Add("stderr", stderr);
                tracingParameters.Add("stdin", stdin);
                tracingParameters.Add("stdout", stdout);
                tracingParameters.Add("tty", tty);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, nameof(WebSocketNamespacedPodExecAsync), tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(BaseUri);
            uriBuilder.Scheme = BaseUri.Scheme == "https" ? "wss" : "ws";

            if (!uriBuilder.Path.EndsWith("/"))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += $"api/v1/namespaces/{@namespace}/pods/{name}/exec";

            var query = string.Empty;

            foreach (var c in command)
            {
                query = QueryHelpers.AddQueryString(query, "command", c);
            }

            if (container != null)
            {
                query = QueryHelpers.AddQueryString(query, "container", Uri.EscapeDataString(container));
            }

            query = QueryHelpers.AddQueryString(query, new Dictionary<string, string>
            {
                {"stderr", stderr ? "1" : "0"},
                {"stdin", stdin ? "1" : "0"},
                {"stdout", stdout ? "1" : "0"},
                {"tty", tty ? "1" : "0"}
            }).TrimStart('?');

            uriBuilder.Query = query;

            return this.StreamConnectAsync(uriBuilder.Uri, _invocationId, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodPortForwardAsync(string name, string @namespace, IEnumerable<int> ports, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("name", name);
                tracingParameters.Add("@namespace", @namespace);
                tracingParameters.Add("ports", ports);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, nameof(WebSocketNamespacedPodPortForwardAsync), tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(this.BaseUri);
            uriBuilder.Scheme = this.BaseUri.Scheme == "https" ? "wss" : "ws";

            if (!uriBuilder.Path.EndsWith("/"))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += $"api/v1/namespaces/{@namespace}/pods/{name}/portforward";

            var q = "";
            foreach (var port in ports)
            {
                q = QueryHelpers.AddQueryString(q, "ports", $"{port}");
            }
            uriBuilder.Query = q.TrimStart('?');



            return StreamConnectAsync(uriBuilder.Uri, _invocationId, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodAttachAsync(string name, string @namespace, string container = default(string), bool stderr = true, bool stdin = false, bool stdout = true, bool tty = false, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("container", container);
                tracingParameters.Add("name", name);
                tracingParameters.Add("namespace", @namespace);
                tracingParameters.Add("stderr", stderr);
                tracingParameters.Add("stdin", stdin);
                tracingParameters.Add("stdout", stdout);
                tracingParameters.Add("tty", tty);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, nameof(WebSocketNamespacedPodAttachAsync), tracingParameters);
            }

            // Construct URL
            var uriBuilder = new UriBuilder(this.BaseUri);
            uriBuilder.Scheme = this.BaseUri.Scheme == "https" ? "wss" : "ws";

            if (!uriBuilder.Path.EndsWith("/"))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path += $"api/v1/namespaces/{@namespace}/pods/{name}/attach";

            uriBuilder.Query = QueryHelpers.AddQueryString(string.Empty, new Dictionary<string, string>
            {
                { "container", container},
                { "stderr", stderr ? "1": "0"},
                { "stdin", stdin ? "1": "0"},
                { "stdout", stdout ? "1": "0"},
                { "tty", tty ? "1": "0"}
            }).TrimStart('?');

            return StreamConnectAsync(uriBuilder.Uri, _invocationId, customHeaders, cancellationToken);
        }

        protected async Task<WebSocket> StreamConnectAsync(Uri uri, string invocationId = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            bool _shouldTrace = ServiceClientTracing.IsEnabled;

            // Create WebSocket transport objects
            WebSocketBuilder webSocketBuilder = this.CreateWebSocketBuilder();

            // Set Headers
            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    webSocketBuilder.SetRequestHeader(_header.Key, string.Join(" ", _header.Value));
                }
            }

            // Set Credentials
#if NET452
            foreach (var cert in ((WebRequestHandler)this.HttpClientHandler).ClientCertificates.OfType<X509Certificate2>())
#else
            foreach (var cert in this.HttpClientHandler.ClientCertificates.OfType<X509Certificate2>())
#endif
            {
                webSocketBuilder.AddClientCertificate(cert);
            }

            if (this.Credentials != null)
            {
                // Copy the default (credential-related) request headers from the HttpClient to the WebSocket
                HttpRequestMessage message = new HttpRequestMessage();
                await this.Credentials.ProcessHttpRequestAsync(message, cancellationToken);

                foreach (var _header in message.Headers)
                {
                    webSocketBuilder.SetRequestHeader(_header.Key, string.Join(" ", _header.Value));
                }
            }

#if NETCOREAPP2_1
            if (this.CaCert != null)
            {
                webSocketBuilder.ExpectServerCertificate(this.CaCert);
            }
            if (this.SkipTlsVerify)
            {
                webSocketBuilder.SkipServerCertificateValidation();
            }
            webSocketBuilder.Options.RequestedSubProtocols.Add(K8sProtocol.ChannelV1);
#endif // NETCOREAPP2_1

            // Send Request
            cancellationToken.ThrowIfCancellationRequested();

            WebSocket webSocket = null;
            try
            {
                webSocket = await webSocketBuilder.BuildAndConnectAsync(uri, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(invocationId, ex);
                }

                throw;
            }
            finally
            {
                if (_shouldTrace)
                {
                    ServiceClientTracing.Exit(invocationId, null);
                }
            }
            return webSocket;
        }
    }
}
