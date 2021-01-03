using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Rest;
using IdentityModel.OidcClient;
using System.Security.Claims;

namespace k8s.Authentication
{
    public class OidcTokenProvider : ITokenProvider
    {
        private OidcClient _oidcClient;
        private string _idToken;
        private string _refreshToken;
        private string _accessToken;
        private DateTime _expiry;

        public OidcTokenProvider(string clientId, string clientSecret, string idpIssuerUrl, string idToken, string refreshToken)
        {
            _idToken = idToken;
            _refreshToken = refreshToken;
            OidcClientOptions options = new OidcClientOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Authority = idpIssuerUrl,
            };
            _oidcClient = new OidcClient(options);
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (_expiry == null || DateTime.UtcNow.AddSeconds(30) > _expiry)
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task RefreshToken()
        {
            Console.WriteLine("refreshing Token");
            IdentityModel.OidcClient.Results.RefreshTokenResult result = await _oidcClient.RefreshTokenAsync(_refreshToken).ConfigureAwait(false);
            _accessToken = result.AccessToken;
            _idToken = result.IdentityToken;
            _refreshToken = result.RefreshToken;
            _expiry = result.AccessTokenExpiration;
            Console.WriteLine("idToken");
            Console.WriteLine(_idToken);
            Console.WriteLine("accessToken");
            Console.WriteLine(_accessToken);
            Console.WriteLine("refreshToken");
            Console.WriteLine(_refreshToken);
            Console.WriteLine("expiry");
            Console.WriteLine(_expiry);
        }
    }
}
