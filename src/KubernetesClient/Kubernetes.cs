using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace k8s
{
    public partial class Kubernetes
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='httpClient'>
        /// HttpClient to be used
        /// </param>
        /// <param name='disposeHttpClient'>
        /// True: will dispose the provided httpClient on calling Kubernetes.Dispose(). False: will not dispose provided httpClient</param>
        protected Kubernetes(HttpClient httpClient, bool disposeHttpClient)
            : base(httpClient, disposeHttpClient)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        protected Kubernetes(params DelegatingHandler[] handlers)
            : base(handlers)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        protected Kubernetes(HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
            : base(rootHandler, handlers)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        protected Kubernetes(Uri baseUri, params DelegatingHandler[] handlers)
            : this(handlers)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        protected Kubernetes(Uri baseUri, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
            : this(rootHandler, handlers)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Subscription credentials which uniquely identify client subscription.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public Kubernetes(ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
            : this(handlers)
        {
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Credentials.InitializeServiceClient(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Subscription credentials which uniquely identify client subscription.
        /// </param>
        /// <param name='httpClient'>
        /// HttpClient to be used
        /// </param>
        /// <param name='disposeHttpClient'>
        /// True: will dispose the provided httpClient on calling Kubernetes.Dispose(). False: will not dispose provided httpClient</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        [Obsolete]
        public Kubernetes(ServiceClientCredentials credentials, HttpClient httpClient, bool disposeHttpClient)
            : this(httpClient, disposeHttpClient)
        {
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Credentials.InitializeServiceClient(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Subscription credentials which uniquely identify client subscription.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public Kubernetes(ServiceClientCredentials credentials, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
            : this(rootHandler, handlers)
        {
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Credentials.InitializeServiceClient(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='credentials'>
        /// Required. Subscription credentials which uniquely identify client subscription.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public Kubernetes(Uri baseUri, ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
            : this(handlers)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Credentials.InitializeServiceClient(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kubernetes"/> class.
        /// </summary>
        /// <param name='baseUri'>
        /// Optional. The base URI of the service.
        /// </param>
        /// <param name='credentials'>
        /// Required. Subscription credentials which uniquely identify client subscription.
        /// </param>
        /// <param name='rootHandler'>
        /// Optional. The http client handler used to handle http transport.
        /// </param>
        /// <param name='handlers'>
        /// Optional. The delegating handlers to add to the http client pipeline.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public Kubernetes(Uri baseUri, ServiceClientCredentials credentials, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
            : this(rootHandler, handlers)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Credentials.InitializeServiceClient(this);
        }

        /// <summary>
        /// Initializes client properties.
        /// </summary>
        private void Initialize()
        {
            BaseUri = new Uri("http://localhost");
            CustomInitialize();
        }

        private async Task<HttpOperationResponse<T>> CreateResultAsync<T>(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse, bool? watch, CancellationToken cancellationToken)
        {
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
                    result.Body = KubernetesJson.Deserialize<T>(stream);
                }
            }
            catch (JsonException ex)
            {
                httpRequest.Dispose();
                httpResponse.Dispose();
                throw new SerializationException("Unable to deserialize the response.", ex);
            }

            return result;
        }

        private class QueryBuilder
        {
            private List<string> parameters = new List<string>();

            public void Append(string key, params object[] values)
            {
                foreach (var value in values)
                {
                    switch (value)
                    {
                        case int intval:
                            parameters.Add($"{key}={intval}");
                            break;
                        case string strval:
                            parameters.Add($"{key}={Uri.EscapeDataString(strval)}");
                            break;
                        case bool boolval:
                            parameters.Add($"{key}={(boolval ? "true" : "false")}");
                            break;
                        default:
                            // null
                            break;
                    }
                }
            }

            public override string ToString()
            {
                if (parameters.Count > 0)
                {
                    return "?" + string.Join("&", parameters);
                }

                return "";
            }
        }

        private HttpRequestMessage CreateRequest(string url, HttpMethod method, IDictionary<string, IList<string>> customHeaders)
        {
            var httpRequest = new HttpRequestMessage();
            httpRequest.Method = method;
            httpRequest.RequestUri = new Uri(url);
            httpRequest.Version = HttpVersion.Version20;
            // Set Headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    httpRequest.Headers.Remove(header.Key);
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return httpRequest;
        }

        private Task<HttpResponseMessage> SendRequest<T>(T body, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            if (body != null)
            {
                var requestContent = KubernetesJson.Serialize(body);
                httpRequest.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                httpRequest.Content.Headers.ContentType = GetHeader(body);
                return SendRequestRaw(requestContent, httpRequest, cancellationToken);
            }

            return SendRequestRaw("", httpRequest, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendRequestRaw(string requestContent, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            // Set Credentials
            if (Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            // Send Request
            cancellationToken.ThrowIfCancellationRequested();
            var httpResponse = await HttpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();

            if (!httpResponse.IsSuccessStatusCode)
            {
                string responseContent = null;
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                if (httpResponse.Content != null)
                {
                    responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }

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
    }
}
