using System.IO;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class YamlTests {
        [Fact]
        public void LoadFromString()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

           var obj = Yaml.LoadFromString<V1Pod>(content);

           Assert.Equal("foo", obj.Metadata.Name);
        }

        [Fact]
        public void LoadFromStream()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                var obj = Yaml.LoadFromStreamAsync<V1Pod>(stream).Result;

                Assert.Equal("foo", obj.Metadata.Name);
            }
        }
    }
}
