using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Informer;
using k8s.Informer.Cache;
using k8s.Models;
using NSubstitute;
using Org.BouncyCastle.Utilities;
using Xunit;

namespace k8s.tests.Informer.Cache
{
    public class DeltaFifoTests
    {
        [Fact]
        public async Task DeltaFIFOBasic()
        {
            var receivingDeltas = new LinkedList<DeltaFifo<V1Pod>.ObjectDelta>();
            var foo1 = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var cache = new Cache<V1Pod>();
            var deltaFIFO = new DeltaFifo<V1Pod>(Caches.DeletionHandlingMetaNamespaceKeyFunc, cache);


            DeltaFifo<V1Pod>.ObjectDelta receivingDelta;
            // basic add operation
            deltaFIFO.Add(foo1);
            cache.Add(foo1);
            var deltaList = await deltaFIFO.ReadAsync();
            receivingDeltas.AddFirst(deltaList.First.Value);
            receivingDelta = receivingDeltas.First.Value;
            receivingDeltas.RemoveFirst();
            receivingDelta.Object.Should().Be(foo1);
            receivingDelta.DeltaType.Should().Be(DeltaType.Added);


            // basic update operation
            deltaFIFO.Update(foo1);
            cache.Update(foo1);
            deltaList = await deltaFIFO.ReadAsync();
            receivingDeltas.AddFirst(deltaList.First.Value);
            receivingDelta = receivingDeltas.First.Value;
            receivingDeltas.RemoveFirst();
            receivingDelta.Object.Should().Be(foo1);
            receivingDelta.DeltaType.Should().Be(DeltaType.Updated);

            // basic delete operation
            deltaFIFO.Delete(foo1);
            cache.Delete(foo1);
            deltaList = await deltaFIFO.ReadAsync();
            receivingDeltas.AddFirst(deltaList.First.Value);
            receivingDelta = receivingDeltas.First.Value;
            receivingDeltas.RemoveFirst();
            receivingDelta.Object.Should().Be(foo1);
            receivingDelta.DeltaType.Should().Be(DeltaType.Deleted);

            // basic sync operation
            deltaFIFO.Replace(new List<V1Pod> {foo1}, "0");
            cache.Replace(new List<V1Pod> {foo1}, "0");
            deltaList = await deltaFIFO.ReadAsync();
            receivingDeltas.AddFirst(deltaList.First.Value);
            receivingDelta = receivingDeltas.First.Value;
            receivingDeltas.RemoveFirst();
            receivingDelta.Object.Should().Be(foo1);
            receivingDelta.DeltaType.Should().Be(DeltaType.Replaced);
        }

        [Fact]
        public void DeltaFIFODedup()
        {
            var foo1 = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var cache = new Cache<V1Pod>();
            var deltaFIFO = new DeltaFifo<V1Pod>(Caches.DeletionHandlingMetaNamespaceKeyFunc, cache);
            LinkedList<DeltaFifo<V1Pod>.ObjectDelta> deltas;
            //
            // // add-delete dedup
            deltaFIFO.Add(foo1);
            deltaFIFO.Delete(foo1);
            deltas = deltaFIFO.Items[Caches.DeletionHandlingMetaNamespaceKeyFunc(foo1)];
            deltas.Last.Value.DeltaType.Should().Be(DeltaType.Deleted);
            deltas.Last.Value.Object.Should().Be(foo1);
            deltas.First.Value.DeltaType.Should().Be(DeltaType.Added);
            deltas.First.Value.Object.Should().Be(foo1);
            deltas.Should().HaveCount(2);
            deltaFIFO.Items.Remove(Caches.DeletionHandlingMetaNamespaceKeyFunc(foo1));


            // // add-delete-delete dedup
            deltaFIFO.Add(foo1);
            deltaFIFO.Delete(foo1);
            deltaFIFO.Delete(foo1);
            deltas = deltaFIFO.Items[Caches.DeletionHandlingMetaNamespaceKeyFunc(foo1)];
            deltas.Last.Value.DeltaType.Should().Be(DeltaType.Deleted);
            deltas.Last.Value.Object.Should().Be(foo1);
            deltas.First.Value.DeltaType.Should().Be(DeltaType.Added);
            deltas.First.Value.Object.Should().Be(foo1);
            deltas.Should().HaveCount(2);
            deltaFIFO.Items.Remove(Caches.DeletionHandlingMetaNamespaceKeyFunc(foo1));

        }

        [Fact]
        public void DeltaFIFOResync()
        {
            var foo1 = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var cache = new Cache<V1Pod>();
            var deltaFIFO = new DeltaFifo<V1Pod>(Caches.DeletionHandlingMetaNamespaceKeyFunc, cache);

            // sync after add
            cache.Add(foo1);
            deltaFIFO.Resync();

            var deltas = deltaFIFO.Items[Caches.DeletionHandlingMetaNamespaceKeyFunc(foo1)];
            deltas.Should().ContainSingle();
            deltas.Last.Value.Object.Should().Be(foo1);
            deltas.Last.Value.DeltaType.Should().Be(DeltaType.Sync);
        }

        [Fact]
        public async Task DeltaFIFOReplaceWithDeleteDeltaIn()
        {
            var oldPod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var newPod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo2",
                    NamespaceProperty = "default"
                }
            };
            // var cache = new Cache<V1Pod>();
            var cache = Substitute.For<IStore<V1Pod>>();
            cache[Caches.DeletionHandlingMetaNamespaceKeyFunc(oldPod)].Returns(oldPod);
            cache.Keys.Returns(new List<string> {Caches.DeletionHandlingMetaNamespaceKeyFunc(oldPod)});
            var deltaFIFO = new DeltaFifo<V1Pod>(Caches.DeletionHandlingMetaNamespaceKeyFunc, cache);

            deltaFIFO.Delete(oldPod);
            deltaFIFO.Replace(new List<V1Pod>() {newPod}, "0");

            var deltas = await deltaFIFO.ReadAsync();
            deltas.First.Value.DeltaType.Should().Be(DeltaType.Deleted);
            deltas.First.Value.Object.Should().Be(oldPod);

            deltas = await deltaFIFO.ReadAsync();
            deltas.First.Value.DeltaType.Should().Be(DeltaType.Replaced);
            deltas.First.Value.Object.Should().Be(newPod);
        }
    }
}