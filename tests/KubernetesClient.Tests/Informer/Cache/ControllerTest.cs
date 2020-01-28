using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Informer;
using k8s.Informer.Cache;
using k8s.Models;
using Microsoft.Rest;
using NSubstitute;
using NSubstitute.Core;
using Org.BouncyCastle.Utilities;
using Xunit;

namespace k8s.tests.Informer.Cache
{
    public class ControllerTest
    {
        private class MockRunOnceListerWatcher<TApi, TApiList> : IListerWatcher<TApi, TApiList>
        {
            private readonly TApiList _list;
            private readonly IEnumerable<Tuple<WatchEventType, TApi>> _events;

            
            private bool listExecuted = false;
            private bool watchExecuted = false;

            public MockRunOnceListerWatcher(TApiList list, params Tuple<WatchEventType, TApi>[] events) : this(list, events.AsEnumerable())
            {}
            public MockRunOnceListerWatcher(TApiList list, IEnumerable<Tuple<WatchEventType, TApi>> events)
            {
                _list = list;
                _events = events;
            }

            public Task<HttpOperationResponse<TApiList>> List(CallGeneratorParams param)
            {
                if (!listExecuted) 
                {
                    listExecuted = true;
                    return Task.FromResult(new HttpOperationResponse<TApiList>()
                    {
                        Body = _list,
                        Response = new HttpResponseMessage(HttpStatusCode.OK)
                    });
                }
                return new TaskCompletionSource<HttpOperationResponse<TApiList>>().Task; // never complete
            }

            public IObservable<Tuple<WatchEventType, TApi>> Watch(CallGeneratorParams param)
            {
                if (!watchExecuted) 
                {
                    watchExecuted = true;
                    return new MockObservable<Tuple<WatchEventType, TApi>>(_events.ToArray());
                }
                throw new InvalidOperationException();
            }
        }

        public class MockObservable<T> : IObservable<T>
        {
            private readonly T[] _events;

            public MockObservable(params T[] events)
            {
                _events = events;
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                foreach (var @event in _events)
                {
                    observer.OnNext(@event);
                    
                }
                observer.OnCompleted();
                return Disposable.Empty;
            }
        }
        [Fact]
        public async Task ControllerProcessDeltas()
        {
            int receivingDeltasCount = 0;
            var foo1 = new V1Pod
            {
                Metadata = new V1ObjectMeta()
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var foo2 = new V1Pod
            {
                Metadata = new V1ObjectMeta()
                {
                    Name = "foo2",
                    NamespaceProperty = "default"
                }
            };
            var foo3 = new V1Pod
            {
                Metadata = new V1ObjectMeta()
                {
                    Name = "foo3",
                    NamespaceProperty = "default"
                }
            };
            V1PodList podList =
                new V1PodList
                {
                    Metadata = new V1ListMeta(),
                    Items = new List<V1Pod>() {foo1, foo2, foo3}
                };
            var deltaFifo = new DeltaFifo<V1Pod>(Caches.DeletionHandlingMetaNamespaceKeyFunc, new Cache<V1Pod>());
            var watcher = new MockRunOnceListerWatcher<V1Pod, V1PodList>(podList, Tuple.Create(WatchEventType.Modified, foo3));
            var controller = new Controller<V1Pod, V1PodList>(deltaFifo, watcher, deltas => Task.FromResult(receivingDeltasCount++));
            await controller.StartAsync(CancellationToken.None);
            await Task.Delay(100);
            receivingDeltasCount.Should().Be(4);
            await controller.StopAsync(CancellationToken.None);
            
        }
    }
}