using k8s.Exceptions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace k8s.Authentication
{
    public class OidcTokenProvider : ITokenProvider
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _idpIssuerUrl;

        private string _idToken;
        private string _refreshToken;
        private DateTimeOffset _expiry;

        public OidcTokenProvider(string clientId, string clientSecret, string idpIssuerUrl, string idToken, string refreshToken)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _idpIssuerUrl = idpIssuerUrl;
            _idToken = idToken;
            _refreshToken = refreshToken;
            _expiry = GetExpiryFromToken();
        }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            if (_idToken == null || DateTime.UtcNow.AddSeconds(30) > _expiry)
            {
                await RefreshToken().ConfigureAwait(false);
            }

            return new AuthenticationHeaderValue("Bearer", _idToken);
        }

        private DateTimeOffset GetExpiryFromToken()
        {
            var parts = _idToken.Split('.');
            if (parts.Length != 3)
            {
                return default;
            }

            try
            {
                var payload = parts[1];
                var jsonBytes = Base64UrlDecode(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);

                using var document = JsonDocument.Parse(json);
                if (document.RootElement.TryGetProperty("exp", out var expElement))
                {
                    var exp = expElement.GetInt64();
                    return DateTimeOffset.FromUnixTimeSeconds(exp);
                }
            }
            catch
            {
                // ignore to default
            }

            return default;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }

            return Convert.FromBase64String(output);
        }

        private async Task RefreshToken()
        {
            try
            {
                using var httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _idpIssuerUrl);
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "grant_type", "refresh_token" },
                        { "client_id", _clientId },
                        { "client_secret", _clientSecret },
                        { "refresh_token", _refreshToken },
                    });

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jsonDocument = JsonDocument.Parse(responseContent);

                if (jsonDocument.RootElement.TryGetProperty("id_token", out var idTokenElement))
                {
                    _idToken = idTokenElement.GetString();
                }

                if (jsonDocument.RootElement.TryGetProperty("refresh_token", out var refreshTokenElement))
                {
                    _refreshToken = refreshTokenElement.GetString();
                }

                if (jsonDocument.RootElement.TryGetProperty("expires_in", out var expiresInElement))
                {
                    var expiresIn = expiresInElement.GetInt32();
                    _expiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn);
                }
            }
            catch (Exception e)
            {
                throw new KubernetesClientException($"Unable to refresh OIDC token. \n {e.Message}", e);
            }
        }
    }
}
