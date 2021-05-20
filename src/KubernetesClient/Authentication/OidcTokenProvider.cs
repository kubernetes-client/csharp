using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Rest;
using IdentityModel.OidcClient;
using k8s.Exceptions;

namespace k8s.Authentication
{
    public class OidcTokenProvider : ITokenProvider
    {
        private OidcClient _oidcClient;
        private string _idToken;
        private string _refreshToken;
        private string _accessToken;
        private DateTimeOffset _expiry;

        public OidcTokenProvider(string clientId, string clientSecret, string idpIssuerUrl, string idToken, string refreshToken)
        {
            _idToken = idToken;
            _refreshToken = refreshToken;
            _oidcClient = getClient(clientId, clientSecret, idpIssuerUrl);
            _expiry = getExpiryFromToken();
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if ((_accessToken == null && _idToken == null) || DateTimeOffset.UtcNow.AddSeconds(30) > _expiry)
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", _accessToken ?? _idToken);
        }

        private DateTimeOffset getExpiryFromToken()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(_accessToken ?? _idToken);
            var expiry = token.Payload.Exp ?? 0;
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(expiry);
            return dateTime;
        }

        private OidcClient getClient(string clientId, string clientSecret, string idpIssuerUrl)
        {
            OidcClientOptions options = new OidcClientOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret ?? "",
                Authority = idpIssuerUrl,
            };

            return new OidcClient(options);
        }

        private async Task RefreshToken()
        {
            try
            {
                var result = await _oidcClient.RefreshTokenAsync(_refreshToken).ConfigureAwait(false);

                if (result.IsError)
                {
                    throw new Exception($"{result.Error}: {result.ErrorDescription}");
                }

                _accessToken = result.AccessToken;
                _idToken = result.IdentityToken;
                _refreshToken = result.RefreshToken;
                _expiry = result.AccessTokenExpiration;
            }
            catch (Exception e)
            {
                throw new KubernetesClientException($"Unable to refresh OIDC token. \n {e.Message}", e);
            }
        }
    }
}
