using FluentAssertions;
using k8s.Models;
using k8s.Util.Informer.Cache;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace k8s.Tests.Util.Informer.Cache
{
    public class ReflectorTest
    {
        private readonly ITestOutputHelper _ouputHelper;

        public ReflectorTest(ITestOutputHelper outputHelper)
        {
            _ouputHelper = outputHelper;
        }

        [Fact(DisplayName = "Create default reflector success")]
        public void CreateReflectorSuccess()
        {
            /*using var apiClient = new Kubernetes(_clientConfiguration);
            var cache = new Cache<V1Pod>();
            var queue = new DeltaFifo(Caches.MetaNamespaceKeyFunc, cache, _deltasLogger);
            var listerWatcher = new ListWatcher<V1Pod, V1PodList>(apiClient, ListAllPods);
            var logger = LoggerFactory.Create(builder => builder.AddXUnit(_ouputHelper).SetMinimumLevel(LogLevel.Trace)).CreateLogger<k8s.Util.Cache.Reflector>();
            var reflector = new k8s.Util.Cache.Reflector<V1Pod, V1PodList>(listerWatcher, queue, logger);

            reflector.Should().NotBeNull();*/
        }
    }
}
