using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace k8s
{
    public partial class Kubernetes
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        public System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets json serialization settings.
        /// </summary>
        public JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Gets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

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
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        protected Kubernetes(System.Uri baseUri, params DelegatingHandler[] handlers)
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
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        protected Kubernetes(System.Uri baseUri, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
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
        /// <exception cref="System.ArgumentNullException">
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
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
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
        /// <exception cref="System.ArgumentNullException">
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
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public Kubernetes(System.Uri baseUri, ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
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
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        public Kubernetes(System.Uri baseUri, ServiceClientCredentials credentials, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
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
            BaseUri = new System.Uri("http://localhost");
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter(),
                    },
            };
            DeserializationSettings = new JsonSerializerSettings
            {
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter(),
                    },
            };
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
                JsonSerializer jsonSerializer = JsonSerializer.Create(DeserializationSettings);
                jsonSerializer.CheckAdditionalContent = true;
#if NET5_0_OR_GREATER
                using (Stream stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
#else
                using (Stream stream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
#endif
                using (JsonTextReader reader = new JsonTextReader(new StreamReader(stream)))
                {
                    result.Body = (T)jsonSerializer.Deserialize(reader, typeof(T));
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
    }
}
