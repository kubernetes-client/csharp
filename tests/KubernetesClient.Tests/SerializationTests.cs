using k8s.Tests.Mock;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class SerializationTests
    {
        private readonly ITestOutputHelper testOutput;

        private enum Animals
        {
            Dog,
            Cat,
            Mouse,
        }

        public SerializationTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public async Task SerializeEnumUsingCamelCase()
        {
            using var server = new MockKubeApiServer(testOutput);

            var config = new KubernetesClientConfiguration { Host = server.Uri.ToString() };
            config.AddJsonOptions(options =>
            {
                // Insert the converter at the front of the list so it overrides
                // the default JsonStringEnumConverter without namingPolicy.
                options.Converters.Insert(index: 0, new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
            var client = new Kubernetes(config);

            var customObject = Animals.Dog;

            var result = await client.CustomObjects.CreateNamespacedCustomObjectWithHttpMessagesAsync(customObject, "TestGroup", "TestVersion", "TestNamespace", "TestPlural").ConfigureAwait(false);
            var content = await result.Request.Content.ReadAsStringAsync();
            Assert.Equal(@"""dog""", content);

            string animal = KubernetesJson.Serialize(Animals.Cat);
            Assert.Equal(@"""cat""", animal);
        }
    }
}
