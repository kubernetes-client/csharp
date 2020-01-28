using System.Collections.Generic;
using FluentAssertions;
using k8s.Informer.Cache;
using k8s.Models;
using Org.BouncyCastle.Utilities;
using Xunit;

namespace k8s.tests.Informer.Cache
{
    public class ListerTests
    {
        [Fact]
        public void TestListerBasic() {
            var podCache = new Cache<V1Pod>();

            var namespacedPodLister = new Lister<V1Pod>(podCache, "default");
            var emptyPodList = namespacedPodLister.List();
            emptyPodList.Should().BeEmpty();

            podCache.Replace(new List<V1Pod>
                {
                    new V1Pod
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Name = "foo1",
                            NamespaceProperty = "default"
                        }
                    },
                    new V1Pod
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Name = "foo2",
                            NamespaceProperty = "default"
                        }
                    },
                    new V1Pod
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Name = "foo3",
                            NamespaceProperty = "default"
                        }
                    }
                    }, "0");
            List<V1Pod> namespacedPodList = namespacedPodLister.List();
            namespacedPodList.Should().HaveCount(3);

            Lister<V1Pod> allNamespacedPodLister = new Lister<V1Pod>(podCache);
            List<V1Pod> allPodList = allNamespacedPodLister.List();
            allPodList.Should().HaveCount(3);

            namespacedPodList = allNamespacedPodLister.Namespace("default").List();
            namespacedPodList.Should().HaveCount(3);
        }
    }
}