using System.Linq;
using FluentAssertions;
using k8s.Models;
using k8s.Util.Informer.Cache;
using Xunit;

namespace k8s.Tests.Util.Informer.Cache
{
    public class ListerTest
    {
        [Fact(DisplayName = "Create default lister success")]
        public void CreateListerDefaultsSuccess()
        {
            var cache = new Cache<V1Pod>();
            var lister = new Lister<V1Pod>(cache);

            lister.Should().NotBeNull();
        }

        [Fact(DisplayName = "List with null namespace success")]
        public void ListNullNamespaceSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();
            var lister = new Lister<V1Pod>(cache);

            cache.Add(aPod);
            var pods = lister.List();

            pods.Should().HaveCount(1);
            pods.Should().Contain(aPod);
            // Can't 'Get' the pod due to no namespace specified in Lister constructor
        }

        [Fact(DisplayName = "List with custom namespace success")]
        public void ListCustomNamespaceSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();
            var lister = new Lister<V1Pod>(cache, aPod.Metadata.NamespaceProperty);

            cache.Add(aPod);
            var pods = lister.List();

            pods.Should().HaveCount(1);
            pods.Should().Contain(aPod);
            lister.Get(aPod.Metadata.Name).Should().Be(aPod);
        }

        [Fact(DisplayName = "Get with null namespace success")]
        public void GetNullNamespaceSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();
            var lister = new Lister<V1Pod>(cache);

            cache.Add(aPod);
            var pod = lister.Get(aPod.Metadata.Name);

            // it's null because the namespace was not set in Lister constructor, but the pod did have a namespace.
            // So it can't build the right key name for lookup in Cache
            pod.Should().BeNull();
        }

        [Fact(DisplayName = "Get with custom namespace success")]
        public void GetCustomNamespaceSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();
            var lister = new Lister<V1Pod>(cache, aPod.Metadata.NamespaceProperty);

            cache.Add(aPod);
            var pod = lister.Get(aPod.Metadata.Name);

            pod.Should().Be(aPod);
        }

        [Fact(DisplayName = "Set custom namespace success")]
        public void SetCustomNamespaceSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();
            var lister = new Lister<V1Pod>(cache);

            cache.Add(aPod);
            var pod = lister.Get(aPod.Metadata.Name);
            pod.Should().BeNull();

            lister = lister.Namespace(aPod.Metadata.NamespaceProperty);

            pod = lister.Get(aPod.Metadata.Name);
            pod.Should().Be(aPod);
        }
    }
}
