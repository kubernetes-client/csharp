using System.Net.Http.Headers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using k8s.Autorest;

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

#if NETSTANDARD2_1_OR_GREATER
        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
#else
        public Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
#endif
        {
            if (TokenExpiresAt < DateTime.UtcNow)
            {
#if NETSTANDARD2_1_OR_GREATER
                token = await File.ReadAllTextAsync(TokenFile, cancellationToken)
                    .ContinueWith(r => r.Result.Trim(), cancellationToken)
                    .ConfigureAwait(false);
#else
                token = File.ReadAllText(TokenFile).Trim();
#endif
                // in fact, the token has a expiry of 10 minutes and kubelet
                // refreshes it at 8 minutes of its lifetime. setting the expiry
                // of 1 minute makes sure the token is reloaded regularly so
                // that its actual expiry is after the declared expiry here,
                // which is as sufficiently true as the time of reading a token
                // < 10-8-1 minute.
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(1);
            }
#if NETSTANDARD2_1_OR_GREATER
            return new AuthenticationHeaderValue("Bearer", token);
#else
            return Task.FromResult(new AuthenticationHeaderValue("Bearer", token));
#endif
        }
    }
}
