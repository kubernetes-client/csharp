using k8s.Authentication;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    public class TokenFileAuthTests
    {
        [Fact]
        public async Task TestToken()
        {
            var auth = new TokenFileAuth("assets/token1");
            var result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(true);
            Assert.Equal("Bearer", result.Scheme);
            Assert.Equal("token1", result.Parameter);

            auth.TokenFile = "assets/token2";
            result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(true);
            Assert.Equal("Bearer", result.Scheme);
            Assert.Equal("token1", result.Parameter);

            auth.TokenExpiresAt = DateTime.UtcNow.AddSeconds(-1);
            result = await auth.GetAuthenticationHeaderAsync(CancellationToken.None).ConfigureAwait(true);
            Assert.Equal("Bearer", result.Scheme);
            Assert.Equal("token2", result.Parameter);
        }
    }
}
