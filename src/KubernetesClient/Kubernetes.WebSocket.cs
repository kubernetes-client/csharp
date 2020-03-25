using k8s.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
#if (NET452 || NETSTANDARD2_0)
using System.Net.Security;
#endif
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
        public Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default", string command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true, string webSocketSubProtol = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return WebSocketNamespacedPodExecAsync(name, @namespace, new string[] { command }, container, stderr, stdin, stdout, tty, webSocketSubProtol, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<IStreamDemuxer> MuxedStreamNamespacedPodExecAsync(string name, string @namespace = "default", IEnumerable<string> command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true, string webSocketSubProtol = WebSocketProtocol.V4BinaryWebsocketProtocol, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            WebSocket webSocket = await this.WebSocketNamespacedPodExecAsync(name: name, @namespace: @namespace, command: command, container: container, tty: tty, cancellationToken: cancellationToken).ConfigureAwait(false);
            StreamDemuxer muxer = new StreamDemuxer(webSocket);
            return muxer;
        }

        /// <inheritdoc/>
        public virtual Task<WebSocket> WebSocketNamespacedPodExecAsync(string name, string @namespace = "default", IEnumerable<string> command = null, string container = null, bool stderr = true, bool stdin = true, bool stdout = true, bool tty = true, string webSocketSubProtol = WebSocketProtocol.V4BinaryWebsocketProtocol, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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
                    throw new InvalidOperationException($"Detected an attempt to execute a command which starts with a Unicode byte order mark (BOM). This is probably incorrect. The command was {c}");
                }
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
                tracingParameters.Add("webSocketSubProtol", webSocketSubProtol);
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

            return this.StreamConnectAsync(uriBuilder.Uri, _invocationId, webSocketSubProtol, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodPortForwardAsync(string name, string @namespace, IEnumerable<int> ports, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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
                tracingParameters.Add("webSocketSubProtocol", webSocketSubProtocol);
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



            return StreamConnectAsync(uriBuilder.Uri, _invocationId, webSocketSubProtocol, customHeaders, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<WebSocket> WebSocketNamespacedPodAttachAsync(string name, string @namespace, string container = default(string), bool stderr = true, bool stdin = false, bool stdout = true, bool tty = false, string webSocketSubProtol = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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
                tracingParameters.Add("webSocketSubProtol", webSocketSubProtol);
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

            return StreamConnectAsync(uriBuilder.Uri, _invocationId, webSocketSubProtol, customHeaders, cancellationToken);
        }

        protected async Task<WebSocket> StreamConnectAsync(Uri uri, string invocationId = null, string webSocketSubProtocol = null, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
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
                await this.Credentials.ProcessHttpRequestAsync(message, cancellationToken).ConfigureAwait(false);

                foreach (var _header in message.Headers)
                {
                    webSocketBuilder.SetRequestHeader(_header.Key, string.Join(" ", _header.Value));
                }
            }

#if (NET452 || NETSTANDARD2_0)
            if (this.CaCerts != null)
            {
                webSocketBuilder.SetServerCertificateValidationCallback(this.ServerCertificateValidationCallback);
            }
#endif

#if NETCOREAPP2_1
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
#endif // NETCOREAPP2_1

            // Send Request
            cancellationToken.ThrowIfCancellationRequested();

            WebSocket webSocket = null;
            try
            {
                webSocket = await webSocketBuilder.BuildAndConnectAsync(uri, CancellationToken.None).ConfigureAwait(false);
            }
            catch (WebSocketException wse) when (wse.WebSocketErrorCode == WebSocketError.HeaderError || (wse.InnerException is WebSocketException && ((WebSocketException)wse.InnerException).WebSocketErrorCode == WebSocketError.HeaderError))
            {
                // This usually indicates the server sent an error message, like 400 Bad Request. Unfortunately, the WebSocket client
                // class doesn't give us a lot of information about what went wrong. So, retry the connection.
                var uriBuilder = new UriBuilder(uri);
                uriBuilder.Scheme = uri.Scheme == "wss" ? "https" : "http";

                var response = await this.HttpClient.GetAsync(uriBuilder.Uri, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.SwitchingProtocols)
                {
                    // This should never happen - the server just allowed us to switch to WebSockets but the previous call didn't work.
                    // Rethrow the original exception
                    response.Dispose();
                    throw;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // Try to parse the content as a V1Status object
                    var genericObject = SafeJsonConvert.DeserializeObject<KubernetesObject>(content);
                    V1Status status = null;

                    if (genericObject.ApiVersion == "v1" && genericObject.Kind == "Status")
                    {
                        status = SafeJsonConvert.DeserializeObject<V1Status>(content);
                    }

                    var ex = new HttpOperationException($"The operation returned an invalid status code: {response.StatusCode}", wse)
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

#if (NET452 || NETSTANDARD2_0)
                if (this.CaCerts != null)
                {
                    webSocketBuilder.CleanupServerCertificateValidationCallback(this.ServerCertificateValidationCallback);
                }
#endif
            }
            return webSocket;
        }

#if (NET452 || NETSTANDARD2_0)
        internal bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return Kubernetes.CertificateValidationCallBack(sender, this.CaCerts, certificate, chain, sslPolicyErrors);
        }
#endif
    }
}
