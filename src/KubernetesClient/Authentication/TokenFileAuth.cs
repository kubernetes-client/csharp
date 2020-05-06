using System;
using System.Net.Http.Headers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace k8s.Authentication
{
    public class TokenFileAuth : ITokenProvider
    {
        private string _token;
        internal string _token_file { get; set; }
        internal DateTime _token_expires_at { get; set; }

        public TokenFileAuth(string token_file)
        {
            _token_file = token_file;
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (_token_expires_at < DateTime.UtcNow)
            {
                _token = File.ReadAllText(_token_file).Trim();
                // in fact, the token has a expiry of 10 minutes and kubelet
                // refreshes it at 8 minutes of its lifetime. setting the expiry
                // of 1 minute makes sure the token is reloaded regularly so
                // that its actual expiry is after the declared expiry here,
                // which is as suffciently true as the time of reading a token
                // < 10-8-1 minute.
                _token_expires_at = DateTime.UtcNow.AddMinutes(1);
            }
            return new AuthenticationHeaderValue("Bearer", _token);
        }
    }
}
