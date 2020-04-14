using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using k8s.Informers.Cache;
using k8s.Tests.Utils;
using Xunit;

namespace k8s.Tests
{
    public class VersionPartitionedSharedCacheTests
    {
        private readonly VersionPartitionedSharedCache<int, TestResource, int> _sut;

        public VersionPartitionedSharedCacheTests()
        {
            _sut = new VersionPartitionedSharedCache<int, TestResource, int>(x => x.Key, x => x.Version);
        }

        [Fact]
        public void AddItem_WhenExistingItemInOtherPartitions_ReuseExistingValue()
        {
            var resourceV1 = new TestResource(1, 1);
            var otherResourceV1 = new TestResource(1, 1);
            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1.Add(resourceV1.Key, resourceV1);
            partition2.Add(resourceV1.Key, otherResourceV1);
            _sut.Items.Should().HaveCount(1);
            partition1.Should().HaveCount(1);
            partition2.Should().HaveCount(1);
            partition2[1].Should().BeSameAs(resourceV1);
        }

        [Fact]
        public void AddItem_WhenMultiplePartitions_OtherPartitionsNotAffected()
        {
            var resourceV1 = new TestResource(1, 1);
            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1.Add(resourceV1.Key, resourceV1);
            partition1.Should().HaveCount(1);
            partition1.Values.First().Should().BeSameAs(resourceV1);
            partition2.Should().BeEmpty();
        }
        [Fact]
        public void AddItem_WhenResourceExistsWithDifferentVersion_AddAsNew()
        {
            var resourceV1 = new TestResource(1, 1);
            var resourceV2 = new TestResource(1, 2);

            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1.Add(resourceV1.Key, resourceV1);
            partition2.Add(resourceV2.Key, resourceV2);

            _sut.Items.Should().HaveCount(2);
            partition1.Should().HaveCount(1);
            partition1.Should().Contain(KeyValuePair.Create(1, resourceV1));
            partition2.Should().HaveCount(1);
            partition2.Should().Contain(KeyValuePair.Create(1, resourceV2));
            partition1[1].Version.Should().NotBe(partition2[1].Version);
        }

        [Fact]
        public void RemoveItem_WhenResourceExistsWithSameVersionInOtherPartitions_RemoveWithoutAffectingOtherPartitions()
        {
            var resourceV1 = new TestResource(1, 1);

            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1.Add(resourceV1.Key, resourceV1);
            partition2.Add(resourceV1.Key, resourceV1);
            partition2.Remove(resourceV1.Key);

            _sut.Items.Should().HaveCount(1);
            partition1.Should().HaveCount(1);
            partition2.Should().BeEmpty();
        }


        [Fact]
        public void RemoveItem_WhenNoOtherPartitionsTrackingRemovedItem_RemovedFromSharedList()
        {
            var resourceV1 = new TestResource(1, 1);

            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1.Add(resourceV1.Key, resourceV1);
            partition2.Add(resourceV1.Key, resourceV1);
            partition1.Remove(resourceV1.Key);
            partition2.Remove(resourceV1.Key);

            partition1.Should().BeEmpty();
            partition2.Should().BeEmpty();
            _sut.Items.Should().BeEmpty();
        }

        [Fact]
        public void Clear_WhenResourceExistsWithSameVersionInOtherPartitions_RemoveWithoutAffectingOtherPartitions()
        {
            var resource1V1 = new TestResource(1, 1);
            var resource2V1 = new TestResource(2, 1);

            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1.Add(resource1V1.Key, resource1V1);
            partition2.Add(resource1V1.Key, resource1V1);
            partition2.Add(resource2V1.Key, resource2V1);
            partition2.Clear();

            _sut.Items.Should().HaveCount(1);
            partition1.Should().HaveCount(1);
            partition2.Should().BeEmpty();
        }


        [Fact]
        public void SetIndexer_WhenNotInSharedList_AddToSharedList()
        {
            var resourceV1 = new TestResource(1, 1);

            var partition1 = _sut.CreatePartition();
            partition1[resourceV1.Key] = resourceV1;

            partition1.Should().HaveCount(1);
            _sut.Items.Should().HaveCount(1);
        }
        [Fact]
        public void SetIndexer_WhenItemAlreadyExistsInShared_DontAddReuseExisting()
        {
            var resourceV1 = new TestResource(1, 1);
            var otherResourceV1 = new TestResource(1, 1);

            var partition1 = _sut.CreatePartition();
            var partition2 = _sut.CreatePartition();
            partition1[1] = resourceV1;
            partition2[1] = otherResourceV1;

            partition1.Should().HaveCount(1);
            partition2.Should().HaveCount(1);
            partition1.Values.First().Should().BeSameAs(resourceV1);
            partition2.Values.First().Should().BeSameAs(resourceV1);
        }

        [Fact]
        public void Add_WhenKeyNotMatchKeyInResource_Throws()
        {
            var resource1V1 = new TestResource(1, 1);

            var partition = _sut.CreatePartition();
            Action act1 = () => partition.Add(2, resource1V1);
            Action act2 = () => partition.Add(KeyValuePair.Create(2, resource1V1));
            Action act3 = () => partition[2] = resource1V1;

            act1.Should().Throw<InvalidOperationException>();
            act2.Should().Throw<InvalidOperationException>();
            act3.Should().Throw<InvalidOperationException>();
        }
    }
}
