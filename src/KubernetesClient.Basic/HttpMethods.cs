using System.Net.Http;

namespace k8s;

internal static class HttpMethods
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
