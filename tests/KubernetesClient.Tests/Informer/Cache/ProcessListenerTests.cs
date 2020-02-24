using System;
using System.Threading;
using System.Threading.Tasks;
using k8s.Informer;
using k8s.Informer.Cache;
using k8s.Models;
using NSubstitute;
using Xunit;

namespace k8s.tests.Informer.Cache
{
    public class ProcessListenerTests
    {
        [Fact]
        public async Task TestNotificationHandling()
        {
            var pod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var resourceEventHandler = Substitute.For<IResourceEventHandler<V1Pod>>();
            var listener = new ProcessorListener<V1Pod>(resourceEventHandler, TimeSpan.Zero);
            await listener.Add(new ProcessorListener<V1Pod>.AddNotification(pod));
            await listener.Add(new ProcessorListener<V1Pod>.UpdateNotification(null,pod));
            await listener.Add(new ProcessorListener<V1Pod>.DeleteNotification(pod));
            await listener.StartAsync(CancellationToken.None);
            await Task.Delay(10);
            await resourceEventHandler.Received(1).OnAdd(pod);
            await resourceEventHandler.Received(1).OnUpdate(null, pod);
            await resourceEventHandler.Received(1).OnAdd(pod);

        }

        [Fact]
        public async Task TestMultipleNotificationsHandling()
        {
            var pod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };
            var resourceEventHandler = Substitute.For<IResourceEventHandler<V1Pod>>();
            var listener = new ProcessorListener<V1Pod>(resourceEventHandler, TimeSpan.Zero);
            for (int i = 0; i < 2000; i++)
            {
                await listener.Add(new ProcessorListener<V1Pod>.AddNotification(pod));
            }

            await listener.StartAsync(CancellationToken.None);
            await Task.Delay(500);
            await resourceEventHandler.Received(2000).OnAdd(pod);
        }
    }
}