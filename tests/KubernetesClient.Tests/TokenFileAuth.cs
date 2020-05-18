using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Authentication;
using Xunit;

namespace k8s.Tests
{
    public class TokenFileAuthTests
    {
        public async Task Token()
        {
            var auth = new TokenFileAuth("assets/token1");
            var result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("token1");

            auth._token_file = "assets/token2";
            result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("token1");

            auth._token_expires_at = DateTime.UtcNow;
            result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None);
            result.Scheme.Should().Be("Bearer");
            result.Parameter.Should().Be("token2");
        }
    }
}
