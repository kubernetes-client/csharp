using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class Kubernetes_Generic_Tests : IClassFixture<KindCluster>
    {
        public Kubernetes_Generic_Tests(ITestOutputHelper log, KindCluster cluster)
        {
            _log = log;
            _cluster = cluster;
        }

        private readonly ITestOutputHelper _log;
        private readonly KindCluster _cluster;

        [Fact(Skip = "Integration Test")]
        public async Task KubernetesGenericMethodsIntegrationTest()
        {
            var configMapName = Guid.NewGuid().ToString("N");
            var ns = "default";
            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            IKubernetes client = new Kubernetes(config);
            var configMap = new V1ConfigMap().Initialize();
            configMap.Metadata.Name = configMapName;
            configMap.Data = new Dictionary<string, string> { { "foo", "bar" } };

            var configMap2 = new V1ConfigMap().Initialize();
            configMap2.Metadata.Name = configMapName;
            configMap2.Data = new Dictionary<string, string> { { "mega", "plumbus" } };

            // create
            var creationResult = await client.CreateWithHttpMessagesAsync(configMap, ns);

            creationResult.Response.EnsureSuccessStatusCode();
            creationResult.Body.Data.Should().BeEquivalentTo(configMap.Data);

            // list
            var listResult = await client.ListWithHttpMessagesAsync<V1ConfigMap>(ns);

            listResult.Response.EnsureSuccessStatusCode();
            listResult.Body.Items.Should().NotBeEmpty();
            listResult.Body.Items.Where(x => x.Metadata.Name == configMapName).Should().ContainSingle();

            // get
            var getResult = await client.ReadWithHttpMessagesAsync<V1ConfigMap>(configMapName, ns);

            getResult.Response.EnsureSuccessStatusCode();
            getResult.Body.Data.Should().BeEquivalentTo(configMap.Data);

            // replace
            var replaceResult = await client.ReplaceWithHttpMessagesAsync(configMap2, configMapName, ns);

            replaceResult.Response.EnsureSuccessStatusCode();
            replaceResult.Body.Data.Should().BeEquivalentTo(configMap2.Data);

            // patch
            var patchDocument = new JsonPatchDocument<V1ConfigMap>();
            patchDocument.Add(x => x.Data["mega"], "fleeb");
            var patch = new V1Patch(patchDocument);

            var patchResult = await client.PatchWithHttpMessagesAsync<V1ConfigMap>(patch, configMapName, ns);

            patchResult.Body.Should().NotBeNull();
            patchResult.Body.Data.Should().Contain("mega", "fleeb");

            // delete
            var deleteResult = await client.DeleteWithHttpMessagesAsync<V1ConfigMap>(configMapName, ns);

            deleteResult.Response.EnsureSuccessStatusCode();
        }
    }
}
