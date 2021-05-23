using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Authentication;
using k8s.Exceptions;
using Xunit;

namespace k8s.Tests
{
    public class OidcAuthTests
    {
        [Fact]
        public async Task TestOidcAuth()
        {
            var clientId = "CLIENT_ID";
            var clientSecret = "CLIENT_SECRET";
            var idpIssuerUrl = "https://idp.issuer.url";
            var unexpiredIdToken = "eyJhbGciOiJIUzI1NiJ9.eyJpYXQiOjAsImV4cCI6MjAwMDAwMDAwMH0.8Ata5uKlrqYfeIaMwS91xVgVFHu7ntHx1sGN95i2Zho";
            var expiredIdToken = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjB9.f37LFpIw_XIS5TZt3wdtEjjyCNshYy03lOWpyDViRM0";
            var refreshToken = "REFRESH_TOKEN";

            // use unexpired id token as bearer, do not attempt to refresh
            var auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, unexpiredIdToken, refreshToken);
            var result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be(unexpiredIdToken);

            try
            {
                // attempt to refresh id token when expired
                auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, expiredIdToken, refreshToken);
                result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
                result.Scheme.Should().Be("Bearer");
                result.Parameter.Should().Be(expiredIdToken);
                Assert.True(false, "should not be here");
            }
            catch (KubernetesClientException e)
            {
                Assert.StartsWith("Unable to refresh OIDC token.", e.Message);
            }

            try
            {
                // attempt to refresh id token when null
                auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, null, refreshToken);
                result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
                result.Scheme.Should().Be("Bearer");
                result.Parameter.Should().Be(expiredIdToken);
                Assert.True(false, "should not be here");
            }
            catch (KubernetesClientException e)
            {
                Assert.StartsWith("Unable to refresh OIDC token.", e.Message);
            }
        }
    }
}
