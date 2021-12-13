using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class IntOrStringTests
    {
        [Fact]
        public void Serialize()
        {
            {
                var v = 123;
                IntstrIntOrString intorstr = v;

                Assert.Equal("123", KubernetesJson.Serialize(intorstr));
            }

            {
                IntstrIntOrString intorstr = "12%";
                Assert.Equal("\"12%\"", KubernetesJson.Serialize(intorstr));
            }
        }

        [Fact]
        public void Deserialize()
        {
            {
                var v = KubernetesJson.Deserialize<IntstrIntOrString>("1234");
                Assert.Equal("1234", v.Value);
            }

            {
                var v = KubernetesJson.Deserialize<IntstrIntOrString>("\"12%\"");
                Assert.Equal("12%", v.Value);
            }
        }
    }
}
