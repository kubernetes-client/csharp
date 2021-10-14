using IdentityModel.OidcClient;
using k8s.Exceptions;
using Microsoft.Rest;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Authentication
{
    public class OidcTokenProvider : ITokenProvider
    {
        private OidcClient _oidcClient;
        private string _idToken;
        private string _refreshToken;
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
            if (_idToken == null || DateTime.UtcNow.AddSeconds(30) > _expiry)
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", _idToken);
        }

        private DateTime getExpiryFromToken()
        {
            int expiry;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var token = handler.ReadJwtToken(_idToken);
                expiry = token.Payload.Exp ?? 0;
            }
            catch
            {
                expiry = 0;
            }

            return DateTimeOffset.FromUnixTimeSeconds(expiry).UtcDateTime;
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
                    throw new Exception(result.Error);
                }

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
