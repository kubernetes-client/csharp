using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using k8s.Exceptions;
using Microsoft.Rest;

namespace k8s.Authentication
{
    public class GcpTokenProvider : ITokenProvider
    {
        private readonly string _gcloudCli;
        private string _token;
        private DateTime _expiry;

        public GcpTokenProvider(string gcloudCli)
        {
            _gcloudCli = gcloudCli;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (DateTime.UtcNow.AddSeconds(30) > _expiry)
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", _token);
        }

        private async Task RefreshToken()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = _gcloudCli,
                    Arguments = "config config-helper --format=json",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                },
                EnableRaisingEvents = true,
            };
            var tcs = new TaskCompletionSource<bool>();
            process.Exited += (sender, arg) =>
            {
                tcs.SetResult(true);
            };
            process.Start();
            var output = process.StandardOutput.ReadToEndAsync();
            var err = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(tcs.Task, output, err).ConfigureAwait(false);

            if (process.ExitCode != 0)
            {
                throw new KubernetesClientException($"Unable to obtain a token via gcloud command. Error code {process.ExitCode}. \n {err}");
            }

            dynamic json = JsonSerializer.Deserialize(await output.ConfigureAwait(false), new
            {
                credential = new
                {
                    access_token = "",
                    token_expiry = DateTime.UtcNow,
                },
            }.GetType());

            _token = json.credential.access_token;
            _expiry = json.credential.token_expiry;
        }
    }
}
