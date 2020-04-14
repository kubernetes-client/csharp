using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using k8s.Exceptions;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

namespace k8s.Authentication
{
    public class GcpTokenProvider : ITokenProvider
    {
        private readonly string _gcloudCli;
        private DateTime _expiry;
        private string _token;

        public GcpTokenProvider(string gcloudCli)
        {
            _gcloudCli = gcloudCli;
        }

        public Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (DateTime.UtcNow.AddSeconds(30) > _expiry)
            {
                RefreshToken();
            }
            return Task.FromResult(new AuthenticationHeaderValue("Bearer", _token));
        }

        private void RefreshToken()
        {
            var process = new Process();
            process.StartInfo.FileName = _gcloudCli;
            process.StartInfo.Arguments = "config config-helper --format=json";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            //* Read the output (or the error)
            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            var err = process.StandardError.ReadToEnd();
            Console.WriteLine(err);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new KubernetesClientException($"Unable to obtain a token via gcloud command. Error code {process.ExitCode}. \n {err}");
            }

            var json = JToken.Parse(output);
            _token = json["credential"]["access_token"].Value<string>();
            _expiry = json["credential"]["token_expiry"].Value<DateTime>();
        }
    }
}
