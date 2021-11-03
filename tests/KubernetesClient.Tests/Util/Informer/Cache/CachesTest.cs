using System;
using System.Linq;
using FluentAssertions;
using k8s.Models;
using Xunit;
using k8s.Util.Informer.Cache;

namespace k8s.Tests.Util.Informer.Cache
{
    public class CachesTest
    {
        [Fact(DisplayName = "Check for default DeletedFinalStateUnknown")]
        public void CheckDefaultDeletedFinalStateUnknown()
        {
            var aPod = Helpers.CreatePods(1).First();
            Caches.DeletionHandlingMetaNamespaceKeyFunc(aPod).Should().Be($"{aPod.Metadata.NamespaceProperty}/{aPod.Metadata.Name}");
        }

        [Fact(DisplayName = "Check for obj DeletedFinalStateUnknown")]
        public void CheckObjDeletedFinalStateUnknown()
        {
            var aPod = Helpers.CreatePods(1).First();
            var key = "a-key";
            var deletedPod = new DeletedFinalStateUnknown<V1Pod>(key, aPod);

            var returnKey = Caches.DeletionHandlingMetaNamespaceKeyFunc(deletedPod);

            // returnKey.Should().Be(key);
        }

        [Fact(DisplayName = "Get default namespace key null")]
        public void GetDefaultNamespaceKeyNull()
        {
            Assert.Throws<ArgumentNullException>(() => { Caches.MetaNamespaceKeyFunc(null); });
        }

        [Fact(DisplayName = "Get default namespace key success")]
        public void GetDefaultNamespaceKeySuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            Caches.MetaNamespaceKeyFunc(aPod).Should().Be($"{aPod.Metadata.NamespaceProperty}/{aPod.Metadata.Name}");
        }

        [Fact(DisplayName = "Get default namespace index null")]
        public void GetDefaultNamespaceIndexNull()
        {
            Assert.Throws<ArgumentNullException>(() => { Caches.MetaNamespaceIndexFunc<V1Pod>(null); });
        }

        [Fact(DisplayName = "Get default namespace index success")]
        public void GetDefaultNamespaceIndexSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var indexes = Caches.MetaNamespaceIndexFunc(aPod);

            indexes.Should().NotBeNull();
            indexes.Should().Contain(aPod.Metadata.NamespaceProperty);
        }
    }
}
