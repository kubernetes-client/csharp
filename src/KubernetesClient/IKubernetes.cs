using System.Net.Http;

namespace k8s
{
    public partial interface IKubernetes
    {
        /// <summary>
        /// Gets the <see cref="HttpClient"/> used for making HTTP requests.
        /// </summary>
        HttpClient HttpClient { get; }
    }
}
