using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace k8s.tests.Informer.Impl
{
    public class DefaultSharedIndexInformerTests
    {
        private string _namespace = "default";
        private string _podName = "apod";
        private string _container = "container";
        [Fact]
        public async Task NamespacedPodInformerNormalBehavior()
        {
            var startRV = "1000";
            var endRV = "1001";
            var podList =
                new V1PodList()
                {
                    Metadata = new V1ListMeta
                    {
                        ResourceVersion = startRV
                    },
                    Items = new List<V1Pod>()
                };
            var k8s = Substitute.For<IKubernetes>();
            k8s.ListNamespacedPodWithHttpMessagesAsync(_namespace, watch: Arg.Is(true))
                .Returns(new HttpOperationResponse<V1PodList>()
                {
                    Response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                    },
                    Body = podList
                });
            var watchResponse = new Watcher<V1Pod>.WatchEvent
            {
                Type = WatchEventType.Added,
                Object = new V1Pod
                {
                    Metadata = new V1ObjectMeta
                    {
                        NamespaceProperty = _namespace,
                        Name = _podName,
                        ResourceVersion = endRV
                    }
                }
            };
            
            k8s.ListNamespacedPodWithHttpMessagesAsync(_namespace, watch: Arg.Is(true))
                .Returns(new HttpOperationResponse<V1PodList>()
                {
                    Response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(watchResponse))
                    }
                });
            var result = await k8s.ListNamespacedPodWithHttpMessagesAsync(_namespace, watch: true);
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);

            JsonConvert.DeserializeObject<Watcher<V1Pod>>(result.Response.Content.ReadAsStringAsync().Result).Should().Be(watchResponse);
            var response = await k8s.ListNamespacedPodAsync(_namespace, watch: true);
            response.Should().Be(podList);
        }
    }
}