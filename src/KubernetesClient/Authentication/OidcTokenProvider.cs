using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
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
        private DateTime _expiry;

        public OidcTokenProvider(string clientId, string clientSecret, string idpIssuerUrl, string idToken, string refreshToken)
        {
            _idToken = idToken;
            _refreshToken = refreshToken;
            _oidcClient = getClient(clientId, clientSecret, idpIssuerUrl);
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (_accessToken == null || DateTime.UtcNow.AddSeconds(30) > _expiry)
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", _accessToken);
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
                var result =
                    await _oidcClient.RefreshTokenAsync(_refreshToken).ConfigureAwait(false);
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
