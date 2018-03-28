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

        [Fact]
        public void WriteToString()
        {
            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta()
                {
                    Name = "foo"
                }
            };

            var yaml = Yaml.SaveToString(pod);
            Assert.Equal(@"apiVersion: v1
kind: Pod
metadata:
  name: foo", yaml);
        }

        [Fact]
        public void CpuRequestAndLimitFromString()
        {
            // Taken from https://raw.githubusercontent.com/kubernetes/website/master/docs/tasks/configure-pod-container/cpu-request-limit.yaml, although
            // the 'namespace' property on 'metadata' was removed since it was rejected by the C# client.
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: cpu-demo
spec:
  containers:
  - name: cpu-demo-ctr
    image: vish/stress
    resources:
      limits:
        cpu: ""1""
      requests:
        cpu: ""0.5""
    args:
            - -cpus
            - ""2""";

            var obj = Yaml.LoadFromString<V1Pod>(content);

            Assert.NotNull(obj?.Spec?.Containers);
            var container = Assert.Single(obj.Spec.Containers);

            Assert.NotNull(container.Resources);
            Assert.NotNull(container.Resources.Limits);
            Assert.NotNull(container.Resources.Requests);

            var cpuLimit = Assert.Single(container.Resources.Limits);
            var cpuRequest = Assert.Single(container.Resources.Requests);

            Assert.Equal("cpu", cpuLimit.Key);
            Assert.Equal("1", cpuLimit.Value.ToString());

            Assert.Equal("cpu", cpuRequest.Key);
            Assert.Equal("500m", cpuRequest.Value.ToString());
        }
    }
}
