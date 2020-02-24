using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Exceptions;
using k8s.Informer;
using k8s.Models;
using Xunit;
using k8s.Informer.Cache;
using k8s.Utils;
using Microsoft.Rest;
using NSubstitute;

namespace k8s.tests.Informer.Cache
{
    public class ReflectorTest
    {
        
        [Fact]
        public async Task TestReflectorRunOnce()
        {
            var pod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var cache = new Cache<V1Pod>();            
            var mockResourceVersion = "1000";
            var listerWatch = Substitute.For<IListerWatcher<V1Pod, V1PodList>>();
            listerWatch.List(Arg.Any<CallGeneratorParams>()).Returns(new HttpOperationResponse<V1PodList>()
            {
                Body = new V1PodList
                {
                    Metadata = new V1ListMeta
                    {
                        ResourceVersion = mockResourceVersion
                    }
                }
            });

            listerWatch.Watch(Arg.Any<CallGeneratorParams>()).Returns(new[] {Tuple.Create(WatchEventType.Added, pod)}.ToObservable());
            
            var reflector = new Reflector<V1Pod, V1PodList>(listerWatch, cache);
            await reflector.StartAsync(CancellationToken.None);
            await Task.Delay(10);
            var objs = cache.ToList();
            objs.Should().ContainSingle();
            objs[0].Metadata.Name.Should().Be(pod.Metadata.Name);
            objs[0].Metadata.NamespaceProperty.Should().Be(pod.Metadata.NamespaceProperty);
        }

        public async Task TestReflectorWatchConnectionCloseOnError()
        {
            var listerWatch = Substitute.For<IListerWatcher<V1Pod, V1PodList>>();
            listerWatch.List(Arg.Any<CallGeneratorParams>()).Returns(new HttpOperationResponse<V1PodList>()
            {
                Body = new V1PodList
                {
                    Metadata = new V1ListMeta()
                }
            });
            
            listerWatch.Watch(Arg.Any<CallGeneratorParams>())
                .Returns(Observable.Throw<Tuple<WatchEventType,V1Pod>>(new KubernetesClientException()));
            
            var reflector = new Reflector<V1Pod, V1PodList>(listerWatch, new Cache<V1Pod>());
            await Task.Delay(10);
            
        }
    }
}