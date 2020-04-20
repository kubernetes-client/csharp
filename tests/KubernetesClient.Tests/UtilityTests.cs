using System;
using System.Text;
using Xunit;

namespace k8s.Tests
{
    public class UtilityTests
    {
        [Fact]
        public void TestQueryStringUtilities()
        {
            var sb = new StringBuilder();
            Assert.Throws<ArgumentNullException>(() => Utilities.AddQueryParameter(null, "key", "value"));
            Assert.Throws<ArgumentNullException>(() => Utilities.AddQueryParameter(sb, null, "value"));
            Assert.Throws<ArgumentNullException>(() => Utilities.AddQueryParameter(sb, "", "value"));

            Utilities.AddQueryParameter(sb, "key", "value");
            Utilities.AddQueryParameter(sb, "key", "a=b");
            Utilities.AddQueryParameter(sb, "+key", null);
            Utilities.AddQueryParameter(sb, "ekey", "");
            Assert.Equal("?key=value&key=a%3Db&%2Bkey=&ekey=", sb.ToString());
        }
    }
}
