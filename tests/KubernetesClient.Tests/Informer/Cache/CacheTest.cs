using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using FluentAssertions;
using k8s.Informer.Cache;
using k8s.Models;
using Org.BouncyCastle.Utilities;
using Xunit;

namespace k8s.tests.Informer.Cache
{
    public class CacheTest
    {
        private const string mockIndexName = "mock";

        private static List<string> MockIndexFunc(V1Pod obj)
        {
            return obj == null ? new List<string> {"null"} : new List<string> {typeof(V1Pod).FullName};
        }
        private static string MockKeyFunc(V1Pod obj)
        {
            return obj == null ? "null" : RuntimeHelpers.GetHashCode(obj).ToString();
        }

        private Cache<V1Pod> _cache = new Cache<V1Pod>(mockIndexName, MockIndexFunc, MockKeyFunc);
        public static IEnumerable<object[]> Data()
        {

            var typeName = typeof(V1Pod).FullName;
            yield return new object[]
            { 
                new V1Pod()
                {
                    Metadata = new V1ObjectMeta
                    {
                        Name = "foo",
                        NamespaceProperty = "default"
                    }
                }, 
                typeName
            };

            yield return new object[]
            {
                new V1Pod
                {
                    Metadata = new V1ObjectMeta
                    {
                        Name = "foo",
                        NamespaceProperty = null
                    }
                },
                typeName
            };

            yield return new object[]
            {
                new V1Pod()
                {
                    Metadata = new V1ObjectMeta
                    {
                        Name = null,
                        NamespaceProperty = "default"
                    }
                },
                typeName
            };
            yield return new object[] {null, "null"};

        }
        [Theory]
        [MemberData(nameof(Data))]
        public void CacheIndex(V1Pod obj, string ns)
        {
            _cache.Replace(new List<V1Pod> {obj}, "0");
            var index = MockIndexFunc(obj)[0];
            var key = MockKeyFunc(obj);
            ns.Should().Be(index);
            
            var indexedObjectList = _cache.ByIndex(mockIndexName, index);
            indexedObjectList.Should().BeEquivalentTo(obj);

            var indexedObjectlist2 = _cache.Index(mockIndexName, obj);
            indexedObjectlist2.Should().BeEquivalentTo(obj);

            var allExistingKeys = _cache.Keys;
            allExistingKeys.Should().BeEquivalentTo(key);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void CacheStore(V1Pod pod, string index)
        {
            if (pod == null)
                return;
            _cache.Replace(new List<V1Pod> {pod}, "0");
            _cache.Delete(pod);
            var indexedObjectList = _cache.ByIndex(mockIndexName, index);
            indexedObjectList.Should().BeEmpty();
            pod.Metadata.ClusterName.Should().BeNull();
            
            _cache.Add(pod);
            // replace cached object w/ null value
            var newClusterName = "test_cluster";
            pod.Metadata.ClusterName = newClusterName;
            _cache.Update(pod);

            _cache.Should().ContainSingle();
            pod.Metadata.ClusterName.Should().Be(newClusterName);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void MultiIndexFuncCacheStore(V1Pod obj, string index)
        {
            var testIndexFuncName = "test-idx-func";
            var podCache = new Cache<V1Pod>();
            podCache.AddIndexFunc(
                testIndexFuncName,
                pod => new List<string>{pod.Spec.NodeName});

            var testPod = new V1Pod
            {
                Metadata = new V1ObjectMeta()
                {
                    NamespaceProperty = "ns",
                    Name = "n"
                },
                Spec = new V1PodSpec()
                {
                    NodeName = "node1"
                }
            };
            podCache.Add(testPod);

            var namespaceIndexedPods = podCache.ByIndex(Caches.NamespaceIndex, "ns");
            namespaceIndexedPods.Should().ContainSingle();

            var nodeNameIndexedPods = podCache.ByIndex(testIndexFuncName, "node1");
            nodeNameIndexedPods.Should().ContainSingle();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void AddIndexers(V1Pod obj, string index)
        {
            var podCache = new Cache<V1Pod>();
            var nodeIndex = "node-index";
            var clusterIndex = "cluster-index";

            var indexers = new Dictionary<string, Func<V1Pod, List<string>>>();

            indexers[nodeIndex] = pod => new List<string> { pod.Spec.NodeName };
            indexers[clusterIndex] = pod => new List<string> { pod.Metadata.ClusterName };
            podCache.AddIndexers(indexers);

            var testPod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    NamespaceProperty = "ns",
                    Name = "n",
                    ClusterName = "cluster1"
                },
                Spec = new V1PodSpec
                {
                    NodeName = "node1"
                }
            };

            podCache.Add(testPod);

            var namespaceIndexedPods = podCache.ByIndex(Caches.NamespaceIndex, "ns");
            namespaceIndexedPods.Should().ContainSingle();

            var nodeNameIndexedPods = podCache.ByIndex(nodeIndex, "node1");
            nodeNameIndexedPods.Should().ContainSingle();

            var clusterNameIndexedPods = podCache.ByIndex(clusterIndex, "cluster1");
            clusterNameIndexedPods.Should().ContainSingle();
        }
    }

}