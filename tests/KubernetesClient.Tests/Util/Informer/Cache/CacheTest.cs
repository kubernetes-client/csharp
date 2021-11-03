using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using k8s.Models;
using k8s.Util.Informer.Cache;
using Xunit;

namespace k8s.Tests.Util.Informer.Cache
{
    public class CacheTest
    {
        [Fact(DisplayName = "Create default cache success")]
        private void CreateCacheSuccess()
        {
            var cache = new Cache<V1Node>();
            cache.Should().NotBeNull();
            cache.GetIndexers().ContainsKey(Caches.NamespaceIndex).Should().BeTrue();
        }

        [Fact(DisplayName = "Add cache item success")]
        private void AddCacheItemSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();

            cache.Add(aPod);

            cache.Get(aPod).Equals(aPod).Should().BeTrue();
        }

        [Fact(DisplayName = "Update cache item success")]
        private void UpdateCacheItemSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();

            var cache = new Cache<V1Pod>();

            cache.Add(aPod);
            aPod.Kind = "another-kind";
            cache.Update(aPod);

            cache.Get(aPod).Kind.Equals(aPod.Kind).Should().BeTrue();
        }

        [Fact(DisplayName = "Delete cache item success")]
        private void DeleteCacheItemSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();

            var cache = new Cache<V1Pod>();

            cache.Add(aPod);
            cache.Delete(aPod);

            // Todo: check indices for removed item
            cache.Get(aPod).Should().BeNull();
        }

        [Fact(DisplayName = "Replace cache items success")]
        private void ReplaceCacheItemsSuccess()
        {
            var pods = Helpers.CreatePods(3);
            var aPod = pods.First();
            var anotherPod = pods.Skip(1).First();
            var yetAnotherPod = pods.Skip(2).First();

            var cache = new Cache<V1Pod>();

            cache.Add(aPod);
            cache.Replace(new[] { anotherPod, yetAnotherPod });

            // Todo: check indices for replaced items
            cache.Get(anotherPod).Should().NotBeNull();
            cache.Get(yetAnotherPod).Should().NotBeNull();
        }

        [Fact(DisplayName = "List item keys success")]
        public void ListItemKeysSuccess()
        {
            var pods = Helpers.CreatePods(3);
            var aPod = pods.First();
            var anotherPod = pods.Skip(1).First();
            var cache = new Cache<V1Pod>();

            cache.Add(aPod);
            cache.Add(anotherPod);

            var keys = cache.ListKeys();

            keys.Should().Contain($"{aPod.Metadata.NamespaceProperty}/{aPod.Metadata.Name}");
            keys.Should().Contain($"{anotherPod.Metadata.NamespaceProperty}/{anotherPod.Metadata.Name}");
        }

        [Fact(DisplayName = "Get item doesn't exist")]
        public void GetItemNotExist()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();

            var item = cache.Get(aPod);
            item.Should().BeNull();
        }

        [Fact(DisplayName = "Get item success")]
        public void GetItemSuccess()
        {
            var aPod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();

            cache.Add(aPod);
            var item = cache.Get(aPod);
            item.Equals(aPod).Should().BeTrue();
        }

        [Fact(DisplayName = "List items success")]
        public void ListItemSuccess()
        {
            var pods = Helpers.CreatePods(3);
            var aPod = pods.First();
            var anotherPod = pods.Skip(1).First();
            var yetAnotherPod = pods.Skip(2).First();

            var cache = new Cache<V1Pod>();

            cache.Add(aPod);
            cache.Add(anotherPod);
            cache.Add(yetAnotherPod);

            var items = cache.List();
            items.Should().HaveCount(3);
            items.Should().Contain(aPod);
            items.Should().Contain(anotherPod);
            items.Should().Contain(yetAnotherPod);
        }

        [Fact(DisplayName = "Get item by key success")]
        public void GetItemByKeySuccess()
        {
            var pod = Helpers.CreatePods(1).First();
            var cache = new Cache<V1Pod>();

            cache.Add(pod);
            var item = cache.GetByKey($"{pod.Metadata.NamespaceProperty}/{pod.Metadata.Name}");
            item.Should().NotBeNull();
        }

        [Fact(DisplayName = "Index items no index")]
        public void IndexItemsNoIndex()
        {
            var pod = Helpers.CreatePods(1).First();

            var cache = new Cache<V1Pod>();

            cache.Add(pod);

            Assert.Throws<ArgumentException>(() => { cache.Index("asdf", pod); });
        }

        [Fact(DisplayName = "Index items success")]
        public void IndexItemsSuccess()
        {
            var pod = Helpers.CreatePods(1).First();

            var cache = new Cache<V1Pod>();

            cache.Add(pod);

            var items = cache.Index("namespace", pod);

            items.Should().Contain(pod);
        }

        [Fact(DisplayName = "Get index keys no index")]
        public void GetIndexKeysNoIndex()
        {
            var cache = new Cache<V1Pod>();

            Assert.Throws<ArgumentException>(() => { cache.IndexKeys("a", "b"); });
        }

        [Fact(DisplayName = "Get index keys no indice item")]
        public void GetIndexKeysNoIndiceItem()
        {
            var cache = new Cache<V1Pod>();

            Assert.Throws<KeyNotFoundException>(() => { cache.IndexKeys("namespace", "b"); });
        }

        [Fact(DisplayName = "Get index keys success")]
        public void GetIndexKeysSuccess()
        {
            var pod = Helpers.CreatePods(1).First();

            var cache = new Cache<V1Pod>();

            cache.Add(pod);
            var keys = cache.IndexKeys("namespace", pod.Metadata.NamespaceProperty);

            keys.Should().NotBeNull();
            keys.Should().Contain(Caches.MetaNamespaceKeyFunc(pod));
        }

        [Fact(DisplayName = "List by index no index")]
        public void ListByIndexNoIndex()
        {
            var cache = new Cache<V1Pod>();

            Assert.Throws<ArgumentException>(() => { cache.ByIndex("a", "b"); });
        }

        [Fact(DisplayName = "List by index no indice item")]
        public void ListByIndexNoIndiceItem()
        {
            var cache = new Cache<V1Pod>();

            Assert.Throws<KeyNotFoundException>(() => { cache.ByIndex("namespace", "b"); });
        }

        [Fact(DisplayName = "List by index success")]
        public void ListByIndexSuccess()
        {
            var pod = Helpers.CreatePods(1).First();

            var cache = new Cache<V1Pod>();

            cache.Add(pod);
            var items = cache.ByIndex("namespace", pod.Metadata.NamespaceProperty);

            items.Should().Contain(pod);
        }

        /* Add Indexers */
        [Fact(DisplayName = "Add null indexers")]
        public void AddNullIndexers()
        {
            var cache = new Cache<V1Pod>();
            Assert.Throws<ArgumentNullException>(() => { cache.AddIndexers(null); });
        }

        [Fact(DisplayName = "Add indexers with conflict")]
        public void AddIndexersConflict()
        {
            var cache = new Cache<V1Pod>();
            Dictionary<string, Func<V1Pod, List<string>>> initialIndexers = new Dictionary<string, Func<V1Pod, List<string>>>()
            {
                { "1", pod => new List<string>() },
                { "2", pod => new List<string>() },
            };
            Dictionary<string, Func<V1Pod, List<string>>> conflictIndexers = new Dictionary<string, Func<V1Pod, List<string>>>()
            {
                { "1", pod => new List<string>() },
            };

            cache.AddIndexers(initialIndexers);
            Assert.Throws<ArgumentException>(() => { cache.AddIndexers(conflictIndexers); });
        }

        [Fact(DisplayName = "Add indexers success")]
        public void AddIndexersSuccess()
        {
            var cache = new Cache<V1Pod>();
            Dictionary<string, Func<V1Pod, List<string>>> indexers = new Dictionary<string, Func<V1Pod, List<string>>>()
            {
                { "2", pod => new List<string>() { pod.Name() } },
                { "3", pod => new List<string>() { pod.Name() } },
            };

            cache.AddIndexers(indexers);

            var savedIndexers = cache.GetIndexers();
            savedIndexers.Should().HaveCount(indexers.Count + 1); // blank cache constructor will add a default index
            savedIndexers.Should().Contain(indexers);

            // Todo: check indicies collection for new indexname keys
        }

        /* Add Index Function */
        [Fact(DisplayName = "Add index function success")]
        public void AddIndexFuncSuccess()
        {
            var cache = new Cache<V1Pod>();
            cache.AddIndexFunc("1", pod => new List<string>() { pod.Name() });

            var savedIndexers = cache.GetIndexers();
            savedIndexers.Should().HaveCount(2);

            // Todo: check indicies collection for new indexname keys
        }

        /* Get Key Function */
        [Fact(DisplayName = "Get default key function success")]
        public void GetDefaultKeyFuncSuccess()
        {
            var pod = new V1Pod()
            {
                Metadata = new V1ObjectMeta()
                {
                    Name = "a-name",
                    NamespaceProperty = "the-namespace",
                },
            };
            var cache = new Cache<V1Pod>();
            var defaultReturnValue = Caches.DeletionHandlingMetaNamespaceKeyFunc<V1Pod>(pod);

            var funcReturnValue = cache.KeyFunc(pod);

            Assert.True(defaultReturnValue.Equals(funcReturnValue));
        }

        /* Set Key Function */
        [Fact(DisplayName = "Set key function success")]
        public void SetKeyFuncSuccess()
        {
            var aPod = new V1Pod()
            {
                Kind = "some-kind",
                Metadata = new V1ObjectMeta()
                {
                    Name = "a-name",
                    NamespaceProperty = "the-namespace",
                },
            };
            var cache = new Cache<V1Pod>();
            var newFunc = new Func<IKubernetesObject<V1ObjectMeta>, string>((pod) => pod.Kind);
            var defaultReturnValue = newFunc(aPod);

            cache.SetKeyFunc(newFunc);

            var funcReturnValue = cache.KeyFunc(aPod);

            Assert.True(defaultReturnValue.Equals(funcReturnValue));
        }
    }
}
