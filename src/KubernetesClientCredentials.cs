namespace k8s
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using k8s.Exceptions;
    using Microsoft.Rest;

    /// <summary>
    /// Class to set the Kubernetes Client Credentials for token based auth
    /// </summary>
    public class KubernetesClientCredentials : ServiceClientCredentials
    {
        public KubernetesClientCredentials(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            this.AuthenticationToken = token;
            this.AuthenticationScheme = "Bearer";
        }

        public KubernetesClientCredentials(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            this.AuthenticationToken = Utils.Base64Encode(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", userName, password));
            this.AuthenticationScheme = "Basic";
        }

        private string AuthenticationToken { get; }

        private string AuthenticationScheme { get; }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(this.AuthenticationScheme))
            {
                throw new KubernetesClientException("AuthenticationScheme cannot be null. Please set the AuthenticationScheme to Basic/Bearer");
            }

            if (string.IsNullOrWhiteSpace(this.AuthenticationToken))
            {
                throw new KubernetesClientException("AuthenticationToken cannot be null. Please set the authentication token");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(this.AuthenticationScheme, this.AuthenticationToken);

            await base.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}