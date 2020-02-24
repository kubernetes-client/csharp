using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Informer;
using k8s.Informer.Cache;
using k8s.Models;
using NSubstitute;
using Xunit;

namespace k8s.tests.Informer.Cache
{
    public class SharedProcessorTests
    {
        [Fact]
        public async Task ListenerAddition()
        {
            var sharedProcessor = new SharedProcessor<V1Pod>();

            var foo1 = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "foo1",
                    NamespaceProperty = "default"
                }
            };   
            var addNotification = new ProcessorListener<V1Pod>.AddNotification(foo1);
            var updateNotification = new ProcessorListener<V1Pod>.UpdateNotification(null,foo1);
            var deleteNotification = new ProcessorListener<V1Pod>.DeleteNotification(foo1);

            var expectAddHandler = new ExpectingNoticationHandler<V1Pod>(addNotification);
            var expectUpdateHandler = new ExpectingNoticationHandler<V1Pod>(updateNotification);
            var expectDeleteHandler = new ExpectingNoticationHandler<V1Pod>(deleteNotification);

            await sharedProcessor.AddAndStartListener(expectAddHandler);
            await sharedProcessor.AddAndStartListener(expectUpdateHandler);
            await sharedProcessor.AddAndStartListener(expectDeleteHandler);

            await sharedProcessor.Distribute(addNotification, false);
            await sharedProcessor.Distribute(updateNotification, false);
            await sharedProcessor.Distribute(deleteNotification, false);
            // sleep 1s for notification distribution completing
            //todo: figure out a better way of doing this
            await Task.Delay(100);

            expectAddHandler.Satisfied.Should().BeTrue();
            expectUpdateHandler.Satisfied.Should().BeTrue();
            expectDeleteHandler.Satisfied.Should().BeTrue();
        }
    }
    public class ExpectingNoticationHandler<ApiType> : ProcessorListener<ApiType> 
    {

        public ExpectingNoticationHandler(Notification expectingNotification) :
            base(Substitute.For<IResourceEventHandler<ApiType>>(), TimeSpan.Zero)
        {
            this.expectingNotification = expectingNotification;
        }

        private readonly Notification expectingNotification;
        public bool Satisfied { get; private set; }


        public override Task Add(Notification obj) 
        {
            base.Add(obj);
            if (!Satisfied) 
            {
                Satisfied = obj.Equals(expectingNotification);
            }

            return Task.CompletedTask;
        }
    }
}