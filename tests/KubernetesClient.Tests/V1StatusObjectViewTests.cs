using k8s.Models;
using k8s.Tests.Mock;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class V1StatusObjectViewTests
    {
        private readonly ITestOutputHelper testOutput;

        public V1StatusObjectViewTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public void ReturnStatus()
        {
            var v1Status = new V1Status
            {
                Message = "test message",
                Status = "test status"
            };

            using (var server = new MockKubeApiServer(testOutput, resp: JsonConvert.SerializeObject(v1Status)))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var status = client.DeleteNamespace(new V1DeleteOptions(), "test");

                Assert.False(status.HasObject);
                Assert.Equal(v1Status.Message, status.Message);
                Assert.Equal(v1Status.Status, status.Status);
            }
        }

        [Fact]
        public void ReturnObject()
        {
            var corev1Namespace = new V1Namespace()
            {
                Metadata = new V1ObjectMeta()
                {
                    Name = "test name"
                },
                Status = new V1NamespaceStatus()
                {
                    Phase = "test termating"
                }
            };

            using (var server = new MockKubeApiServer(testOutput, resp: JsonConvert.SerializeObject(corev1Namespace)))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var status = client.DeleteNamespace(new V1DeleteOptions(), "test");

                Assert.True(status.HasObject);

                var obj = status.ObjectView<V1Namespace>();

                Assert.Equal(obj.Metadata.Name, corev1Namespace.Metadata.Name);
                Assert.Equal(obj.Status.Phase, corev1Namespace.Status.Phase);
            }

        }
    }
    }
