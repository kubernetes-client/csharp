using System;
using FluentAssertions;
using k8s.Models;
using Xunit;
using k8s.Informer.Cache;

namespace k8s.tests.Informer.Cache
{
    public class CachesTest
    {
        [Fact]
        public void DefaultNamespaceNameKey()
        {
            var testName = "test-name";
            var testNamespace = "test-namespace";
            var pod = new V1Pod()
            {
                Metadata = new V1ObjectMeta
                {
                    Name = testName,
                    NamespaceProperty = testNamespace
                }
            };
            Caches.MetaNamespaceKeyFunc(pod).Should().Be($"{testNamespace}/{testName}");
        }
        [Fact]
        public void DefaultNamespaceIndex() 
        {
            var testName = "test-name";
            var testNamespace = "test-namespace";
            var pod = new V1Pod()
            {
                Metadata = new V1ObjectMeta
                {
                    Name = testName,
                    NamespaceProperty = testNamespace
                }
            };
            var indices = Caches.MetaNamespaceIndexFunc(pod);

            indices[0].Should().Be(pod.Metadata.NamespaceProperty);
        }
    }
}