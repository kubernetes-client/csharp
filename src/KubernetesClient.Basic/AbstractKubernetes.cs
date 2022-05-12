using System.Net.Http;
using System.Net.Http.Headers;

namespace k8s;

public abstract partial class AbstractKubernetes
{
    private static class HttpMethods
    {
        public static readonly HttpMethod Delete = HttpMethod.Delete;
        public static readonly HttpMethod Get = HttpMethod.Get;
        public static readonly HttpMethod Head = HttpMethod.Head;
        public static readonly HttpMethod Options = HttpMethod.Options;
        public static readonly HttpMethod Post = HttpMethod.Post;
        public static readonly HttpMethod Put = HttpMethod.Put;
        public static readonly HttpMethod Trace = HttpMethod.Trace;
        public static readonly HttpMethod Patch = new HttpMethod("Patch");
    }

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
                return $"?{string.Join("&", parameters)}";
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

    protected virtual MediaTypeHeaderValue GetHeader(object body)
    {
        if (body == null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        if (body is V1Patch patch)
        {
            return GetHeader(patch);
        }

        return MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
    }

    private MediaTypeHeaderValue GetHeader(V1Patch body)
    {
        if (body == null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        switch (body.Type)
        {
            case V1Patch.PatchType.JsonPatch:
                return MediaTypeHeaderValue.Parse("application/json-patch+json; charset=utf-8");
            case V1Patch.PatchType.MergePatch:
                return MediaTypeHeaderValue.Parse("application/merge-patch+json; charset=utf-8");
            case V1Patch.PatchType.StrategicMergePatch:
                return MediaTypeHeaderValue.Parse("application/strategic-merge-patch+json; charset=utf-8");
            case V1Patch.PatchType.ApplyPatch:
                return MediaTypeHeaderValue.Parse("application/apply-patch+yaml; charset=utf-8");
            default:
                throw new ArgumentOutOfRangeException(nameof(body.Type), "");
        }
    }

    protected abstract Task<HttpOperationResponse<T>> CreateResultAsync<T>(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse, bool? watch, CancellationToken cancellationToken);

    protected abstract HttpRequestMessage CreateRequest(string relativeUri, HttpMethod method, IReadOnlyDictionary<string, IReadOnlyList<string>> customHeaders);

    protected abstract Task<HttpResponseMessage> SendRequestRaw(string requestContent, HttpRequestMessage httpRequest, CancellationToken cancellationToken);
}
