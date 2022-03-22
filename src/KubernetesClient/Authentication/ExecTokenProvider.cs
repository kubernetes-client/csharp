using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using k8s.KubeConfigModels;
using k8s.Autorest;

namespace k8s.Authentication
{
    public class ExecTokenProvider : ITokenProvider
    {
        private readonly ExternalExecution exec;
        private ExecCredentialResponse response;

        public ExecTokenProvider(ExternalExecution exec)
        {
            this.exec = exec;
        }

        private bool NeedsRefresh()
        {
            if (response?.Status == null)
            {
                return true;
            }

            if (response.Status.Expiry == null)
            {
                return false;
            }

            return DateTime.UtcNow.AddSeconds(30) > response.Status.Expiry;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (NeedsRefresh())
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", response.Status.Token);
        }

        private async Task RefreshToken()
        {
            response =
                await Task.Run(() => KubernetesClientConfiguration.ExecuteExternalCommand(this.exec)).ConfigureAwait(false);
        }
    }
}
