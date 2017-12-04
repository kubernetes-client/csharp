using k8s.Models;
using Newtonsoft.Json;
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

                Assert.Equal("123", JsonConvert.SerializeObject(intorstr));
            }

            {
                IntstrIntOrString intorstr = "12%";
                Assert.Equal("\"12%\"", JsonConvert.SerializeObject(intorstr));
            }
        }

        [Fact]
        public void Deserialize()
        {
            {
                var v = JsonConvert.DeserializeObject<IntstrIntOrString>("1234");
                Assert.Equal("1234", v.Value);
            }

            {
                var v = JsonConvert.DeserializeObject<IntstrIntOrString>("\"12%\"");
                Assert.Equal("12%", v.Value);
            }
        }
    }
}
