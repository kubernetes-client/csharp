using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using k8s.Autorest;
using System.Net.Http.Headers;


namespace k8s
{
    public abstract partial class AbstractKubernetes
    {
        private sealed class QueryBuilder
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

        public virtual TimeSpan HttpClientTimeout { get; set; } = TimeSpan.FromSeconds(100);

        protected abstract Task<HttpOperationResponse<T>> CreateResultAsync<T>(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse, bool? watch, CancellationToken cancellationToken);

        protected abstract HttpRequestMessage CreateRequest(string relativeUri, string method, IDictionary<string, IList<string>> customHeaders);

        protected abstract MediaTypeHeaderValue GetHeader(object body);

        protected abstract Task<HttpResponseMessage> SendRequestRaw(string requestContent, HttpRequestMessage httpRequest, CancellationToken cancellationToken);
    }
}
