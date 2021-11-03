using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Models;
using k8s.Tests.Mock;
using k8s.Util.Common;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests.Util.Common.Generic
{
    public class GenericKubernetesApiTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public GenericKubernetesApiTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "Create constructor success")]
        public void CreateConstSuccess()
        {
            using var server = new MockKubeApiServer(_outputHelper);
            var genericApi = Helpers.BuildGenericApi(server.Uri);
            genericApi.Should().NotBeNull();
        }

        [Fact(DisplayName = "Get namespaced object success")]
        public async Task GetNamespacedObject()
        {
            var serverOptions = new MockKubeApiServerOptions(MockKubeServerFlags.GetPod);
            using var server = new MockKubeApiServer(_outputHelper, serverOptions.ShouldNext);
            var podName = "nginx-1493591563-xb2v4";
            var genericApi = Helpers.BuildGenericApi(server.Uri);

            var resp = await genericApi.GetAsync<V1Pod>(Namespaces.NamespaceDefault, podName).ConfigureAwait(false);

            resp.Should().NotBeNull();
            resp.Metadata.Name.Should().Be(podName);
            resp.Metadata.NamespaceProperty.Should().Be(Namespaces.NamespaceDefault);
        }

        [Fact(DisplayName = "List namespaced object success")]
        public async Task ListNamespacedObject()
        {
            var serverOptions = new MockKubeApiServerOptions(MockKubeServerFlags.ListPods);
            using var server = new MockKubeApiServer(_outputHelper, serverOptions.ShouldNext);
            var genericApi = Helpers.BuildGenericApi(server.Uri);

            var resp = await genericApi.ListAsync<V1PodList>(Namespaces.NamespaceDefault).ConfigureAwait(false);

            resp.Should().NotBeNull();
            resp.Items.Should().NotBeNull();
        }

        [Fact(DisplayName = "Patch namespaced object success")]
        public async Task PatchNamespacedObject()
        {
            using var server = new MockKubeApiServer(_outputHelper);
            var podName = "nginx-1493591563-xb2v4";
            var genericApi = Helpers.BuildGenericApi(server.Uri);

            var resp = await genericApi.PatchAsync<V1Pod>(Namespaces.NamespaceDefault, podName).ConfigureAwait(false);

            resp.Should().NotBeNull();
        }

        [Fact(DisplayName = "Update object success")]
        public async Task UpdateObject()
        {
            using var server = new MockKubeApiServer(_outputHelper);
            var pod = Helpers.CreatePods(1).First();
            var genericApi = Helpers.BuildGenericApi(server.Uri);

            var resp = await genericApi.UpdateAsync(pod).ConfigureAwait(false);

            resp.Should().NotBeNull();
        }

        [Fact(DisplayName = "Delete namespaced object success")]
        public async Task DeleteNamespacedObject()
        {
            using var server = new MockKubeApiServer(_outputHelper);
            var podName = "nginx-1493591563-xb2v4";
            var genericApi = Helpers.BuildGenericApi(server.Uri);

            var resp = await genericApi.DeleteAsync<V1Pod>(Namespaces.NamespaceDefault, podName).ConfigureAwait(false);

            resp.Should().NotBeNull();
        }

        [Fact(DisplayName = "Watch namespaced object success")]
        public void WatchNamespacedObject()
        {
            using var cts = new CancellationTokenSource();
            var serverOptions = new MockKubeApiServerOptions(MockKubeServerFlags.ModifiedPod);
            using var server = new MockKubeApiServer(_outputHelper, serverOptions.ShouldNext);
            var genericApi = Helpers.BuildGenericApi(server.Uri);

            using var resp = genericApi.Watch<V1Pod>(Namespaces.NamespaceDefault, (actionType, pod) => { }, exception => { }, () => { }, cts.Token);

            resp.Should().NotBeNull();
            cts.CancelAfter(1000);
            serverOptions.ServerShutdown?.Set();
        }
    }
}
