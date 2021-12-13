using System.Net.Http.Headers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace k8s.Authentication
{
    public class TokenFileAuth : ITokenProvider
    {
        private string token;
        internal string TokenFile { get; set; }
        internal DateTime TokenExpiresAt { get; set; }

        public TokenFileAuth(string tokenFile)
        {
            TokenFile = tokenFile;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (TokenExpiresAt < DateTime.UtcNow)
            {
                token = await File.ReadAllTextAsync(TokenFile, cancellationToken)
                    .ContinueWith(r => r.Result.Trim(), cancellationToken)
                    .ConfigureAwait(false);
                // in fact, the token has a expiry of 10 minutes and kubelet
                // refreshes it at 8 minutes of its lifetime. setting the expiry
                // of 1 minute makes sure the token is reloaded regularly so
                // that its actual expiry is after the declared expiry here,
                // which is as sufficiently true as the time of reading a token
                // < 10-8-1 minute.
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(1);
            }

            return new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
