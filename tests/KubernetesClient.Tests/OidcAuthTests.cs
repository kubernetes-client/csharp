using FluentAssertions;
using k8s.Authentication;
using k8s.Exceptions;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
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
            var result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(true);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be(unexpiredIdToken);

            try
            {
                // attempt to refresh id token when expired
                auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, expiredIdToken, refreshToken);
                result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(true);
                result.Scheme.Should().Be("Bearer");
                result.Parameter.Should().Be(expiredIdToken);
                Assert.Fail("should not be here");
            }
            catch (KubernetesClientException e)
            {
                Assert.StartsWith("Unable to refresh OIDC token.", e.Message);
            }

            try
            {
                // attempt to refresh id token when null
                auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, null, refreshToken);
                result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(true);
                result.Scheme.Should().Be("Bearer");
                result.Parameter.Should().Be(expiredIdToken);
                Assert.Fail("should not be here");
            }
            catch (KubernetesClientException e)
            {
                Assert.StartsWith("Unable to refresh OIDC token.", e.Message);
            }
        }

        [Fact]
        public async Task TestOidcAuthWithWireMock()
        {
            // Arrange
            var server = WireMockServer.Start();
            var idpIssuerUrl = server.Url + "/token";
            var clientId = "CLIENT_ID";
            var clientSecret = "CLIENT_SECRET";
            var expiredIdToken = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjB9.f37LFpIw_XIS5TZt3wdtEjjyCNshYy03lOWpyDViRM0";
            var refreshToken = "REFRESH_TOKEN";
            var newIdToken = "NEW_ID_TOKEN";
            var expiresIn = 3600;

            // Simulate a successful token refresh response
            server
                .Given(Request.Create().WithPath("/token").UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody($@"{{
                        ""id_token"": ""{newIdToken}"",
                        ""refresh_token"": ""{refreshToken}"",
                        ""expires_in"": {expiresIn}
                    }}"));

            var auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, expiredIdToken, refreshToken);

            // Act
            var result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None);

            // Assert
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be(newIdToken);

            // Verify that the expiry is set correctly
            var expectedExpiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn);
            var actualExpiry = typeof(OidcTokenProvider)
                .GetField("_expiry", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(auth) as DateTimeOffset?;
            actualExpiry.Should().NotBeNull();
            actualExpiry.Value.Should().BeCloseTo(expectedExpiry, precision: TimeSpan.FromSeconds(5));

            // Verify that the refresh token is set correctly
            var actualRefreshToken = typeof(OidcTokenProvider)
                .GetField("_refreshToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(auth) as string;
            actualRefreshToken.Should().NotBeNull();
            actualRefreshToken.Should().Be(refreshToken);

            // Stop the server
            server.Stop();
        }

        [Fact]
        public async Task TestOidcAuthWithServerError()
        {
            // Arrange
            var server = WireMockServer.Start();
            var idpIssuerUrl = server.Url + "/token";
            var clientId = "CLIENT_ID";
            var clientSecret = "CLIENT_SECRET";
            var expiredIdToken = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjB9.f37LFpIw_XIS5TZt3wdtEjjyCNshYy03lOWpyDViRM0";
            var refreshToken = "REFRESH_TOKEN";

            // Simulate a server error response
            server
                .Given(Request.Create().WithPath("/token").UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithBody(@"{ ""error"": ""server_error"" }"));

            var auth = new OidcTokenProvider(clientId, clientSecret, idpIssuerUrl, expiredIdToken, refreshToken);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KubernetesClientException>(
                () => auth.GetAuthenticationHeaderAsync(CancellationToken.None));
            exception.Message.Should().StartWith("Unable to refresh OIDC token.");
            exception.InnerException.Message.Should().Contain("500");

            // Stop the server
            server.Stop();
        }
    }
}
