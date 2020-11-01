using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Authentication;

namespace k8s.Tests
{
    public class TokenFileAuthTests
    {
        public async Task TestToken()
        {
            var auth = new TokenFileAuth("assets/token1");
            var result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("token1");

            auth.TokenFile = "assets/token2";
            result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("token1");

            auth.TokenExpiresAt = DateTime.UtcNow;
            result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(false);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("token2");
        }
    }
}
