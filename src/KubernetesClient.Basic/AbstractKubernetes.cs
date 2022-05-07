using System.Net.Http;
using System.Net.Http.Headers;

namespace k8s;

public abstract partial class AbstractKubernetes
{


    public virtual TimeSpan HttpClientTimeout { get; set; } = TimeSpan.FromSeconds(100);

    protected internal virtual MediaTypeHeaderValue GetHeader(object body)
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

    protected internal abstract Task<HttpOperationResponse<T>> CreateResultAsync<T>(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse, bool? watch, CancellationToken cancellationToken);

    protected internal abstract HttpRequestMessage CreateRequest(string relativeUri, HttpMethod method, IReadOnlyDictionary<string, IReadOnlyList<string>> customHeaders);

    protected internal abstract Task<HttpResponseMessage> SendRequestRaw(string requestContent, HttpRequestMessage httpRequest, CancellationToken cancellationToken);
}
