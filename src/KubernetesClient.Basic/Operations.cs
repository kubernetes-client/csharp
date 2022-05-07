using System.Net.Http;

namespace k8s;

internal class Operations
{
    private readonly AbstractKubernetes kubernetes;

    public TimeSpan HttpClientTimeout => kubernetes.HttpClientTimeout;

    public Operations(AbstractKubernetes kubernetes)
    {
        this.kubernetes = kubernetes;
    }

    protected Task<HttpResponseMessage> SendRequest<T>(T body, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        if (body != null)
        {
            var requestContent = KubernetesJson.Serialize(body);
            httpRequest.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
            httpRequest.Content.Headers.ContentType = kubernetes.GetHeader(body);
            return SendRequestRaw(requestContent, httpRequest, cancellationToken);
        }

        return SendRequestRaw("", httpRequest, cancellationToken);
    }

    internal Task<HttpOperationResponse<T>> CreateResultAsync<T>(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse, bool? watch, CancellationToken cancellationToken)
    {
        return kubernetes.CreateResultAsync<T>(httpRequest, httpResponse, watch, cancellationToken);
    }

    internal HttpRequestMessage CreateRequest(string relativeUri, HttpMethod method, IReadOnlyDictionary<string, IReadOnlyList<string>> customHeaders)
    {
        return kubernetes.CreateRequest(relativeUri, method, customHeaders);
    }

    internal Task<HttpResponseMessage> SendRequestRaw(string requestContent, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        return kubernetes.SendRequestRaw(requestContent, httpRequest, cancellationToken);
    }
}
