using k8s.Authentication;
using System.Net;
using System.Net.Http;

namespace k8s
{
    public partial class Kubernetes : AbstractKubernetes, IKubernetes
    {
        private Uri baseuri;

        /// <summary>
        /// The base URI of the service.
        /// </summary>
        public Uri BaseUri
        {
            get => baseuri;
            set
            {
                var baseUrl = value?.AbsoluteUri ?? throw new ArgumentNullException(nameof(BaseUri));
                baseuri = new Uri(baseUrl + (baseUrl.EndsWith("/") ? "" : "/"));
            }
        }

        /// <summary>
        /// Subscription credentials which uniquely identify client subscription.
        /// </summary>
        public ServiceClientCredentials Credentials { get; private set; }

        public HttpClient HttpClient { get; protected set; }

        /// <summary>
        /// Reference to the first HTTP handler (which is the start of send HTTP
        /// pipeline).
        /// </summary>
        private HttpMessageHandler FirstMessageHandler { get; set; }

        /// <summary>
        /// Reference to the innermost HTTP handler (which is the end of send HTTP
        /// pipeline).
        /// </summary>
#if NET5_0_OR_GREATER
        private SocketsHttpHandler HttpClientHandler { get; set; }
#else
        private HttpClientHandler HttpClientHandler { get; set; }
#endif

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private bool DisableHttp2 { get; set; }
#endif

        /// <summary>
        /// Initializes client properties.
        /// </summary>
        private void Initialize()
        {
            BaseUri = new Uri("http://localhost");
        }

        protected override async Task<HttpOperationResponse<T>> CreateResultAsync<T>(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse, bool? watch, CancellationToken cancellationToken)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            var result = new HttpOperationResponse<T>() { Request = httpRequest, Response = httpResponse };

            if (watch == true)
            {
                httpResponse.Content = new LineSeparatedHttpContent(httpResponse.Content, cancellationToken);
            }

            try
            {
#if NET5_0_OR_GREATER
                using (Stream stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
#else
                using (Stream stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
#endif
                {
                    result.Body = KubernetesJson.Deserialize<T>(stream, jsonSerializerOptions);
                }
            }
            catch (JsonException)
            {
                httpRequest.Dispose();
                httpResponse.Dispose();
                throw;
            }

            return result;
        }

        protected override Task<HttpResponseMessage> SendRequest<T>(string relativeUri, HttpMethod method, IReadOnlyDictionary<string, IReadOnlyList<string>> customHeaders, T body, CancellationToken cancellationToken)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(BaseUri, relativeUri),
            };

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            if (!DisableHttp2)
            {
                httpRequest.Version = HttpVersion.Version20;
            }
#endif
            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    httpRequest.Headers.Remove(header.Key);
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (body != null)
            {
                var requestContent = KubernetesJson.Serialize(body, jsonSerializerOptions);
                httpRequest.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                httpRequest.Content.Headers.ContentType = GetHeader(body);
                return SendRequestRaw(requestContent, httpRequest, cancellationToken);
            }

            return SendRequestRaw("", httpRequest, cancellationToken);
        }

        protected virtual async Task<HttpResponseMessage> SendRequestRaw(string requestContent, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            // Set Credentials
            if (Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            if (!string.IsNullOrWhiteSpace(TlsServerName))
            {
                httpRequest.Headers.Host = TlsServerName;
            }

            // Send Request
            cancellationToken.ThrowIfCancellationRequested();
            var httpResponse = await HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();

            if (!httpResponse.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

                var ex = new HttpOperationException($"Operation returned an invalid status code '{statusCode}', response body {responseContent}");
                ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }

                throw ex;
            }

            return httpResponse;
        }

        /// <summary>
        /// Indicates whether the ServiceClient has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Dispose the ServiceClient.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the HttpClient and Handlers.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                // Dispose the client
                HttpClient?.Dispose();
                HttpClient = null;

                // Dispose the certificates
                if (CaCerts is not null)
                {
                    foreach (var caCert in CaCerts)
                    {
                        caCert.Dispose();
                    }

                    CaCerts.Clear();
                }

                ClientCert?.Dispose();
                ClientCert = null;

                FirstMessageHandler?.Dispose();
                FirstMessageHandler = null;

                HttpClientHandler?.Dispose();
                HttpClientHandler = null;
            }
        }
    }
}
