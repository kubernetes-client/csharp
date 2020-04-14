using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using k8s.Informers;
using k8s.Informers.FaultTolerance;
using k8s.Informers.Notifications;
using k8s.Models;
using k8s.Tests.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class KubernetesResourceInformerTests : IDisposable
    {
        private readonly ITestOutputHelper _testOutput;
        private WireMockServer _server;
        private Kubernetes _kubernetes;
        private ILogger _log;

        public KubernetesResourceInformerTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
            _log = new XunitLogger<SharedInformerTests>(testOutput);
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings() { Converters = new[] { new StringEnumConverter() }, Formatting = Formatting.None };
            _server = WireMockServer.Start();
            _kubernetes = new Kubernetes(new KubernetesClientConfiguration { Host = _server.Urls.First() });
        }

        [Fact]
        public async Task List()
        {
            _server.Given(Request.Create().WithParam("watch", MatchBehaviour.RejectOnMatch, "true").UsingGet())
                .RespondWith(Response.Create().WithBodyAsJson(TestData.ListPodsTwoItems));
            var sut = new KubernetesInformer<V1Pod>(_kubernetes);

            var result = await sut.GetResource(ResourceStreamType.List).ToList().TimeoutIfNotDebugging(); ;

            result.Should().HaveCount(2);
            result[0].EventFlags.Should().HaveFlag(EventTypeFlags.ResetStart);
            result[1].EventFlags.Should().HaveFlag(EventTypeFlags.ResetEnd);
            result[0].Value.Should().BeEquivalentTo(TestData.ListPodsTwoItems.Items[0]);
            result[1].Value.Should().BeEquivalentTo(TestData.ListPodsTwoItems.Items[1]);
        }
        [Fact]
        public async Task Watch()
        {
            _server.Given(Request.Create().WithParam("watch", MatchBehaviour.AcceptOnMatch, "true").UsingGet())
                .RespondWith(Response.Create().WithBodyAsJson(TestData.TestPod1ResourceVersion2.ToWatchEvent(WatchEventType.Added)));
            var sut = new KubernetesInformer<V1Pod>(_kubernetes, null, () => false);

            var result = await sut.GetResource(ResourceStreamType.Watch).ToList().TimeoutIfNotDebugging();

            result.Should().HaveCount(1);
            result[0].EventFlags.Should().HaveFlag(EventTypeFlags.Add);
            result[0].Value.Should().BeEquivalentTo(TestData.TestPod1ResourceVersion2);
        }

        [Fact]
        public async Task ListWatch()
        {

            _server.Given(Request.Create().UsingGet()).AtPriority(100)
                .RespondWith(Response.Create().WithBodyAsJson(TestData.ListPodsTwoItems));
            _server.Given(Request.Create().WithParam("watch", MatchBehaviour.AcceptOnMatch, "true").UsingGet())
                .RespondWith(Response.Create().WithBodyAsJson(TestData.TestPod1ResourceVersion2.ToWatchEvent(WatchEventType.Modified)));

            var sut = new KubernetesInformer<V1Pod>(_kubernetes, new RetryPolicy((e, i) => false, i => TimeSpan.Zero), () => false);

            var result = await sut.GetResource(ResourceStreamType.ListWatch).ToList().TimeoutIfNotDebugging();
            result.Should().HaveCount(3);
            result[0].EventFlags.Should().HaveFlag(EventTypeFlags.ResetStart);
            result[0].Value.Should().BeEquivalentTo(TestData.TestPod1ResourceVersion1);
            result[1].EventFlags.Should().HaveFlag(EventTypeFlags.ResetEnd);
            result[1].Value.Should().BeEquivalentTo(TestData.TestPod2ResourceVersion1);
            result[2].EventFlags.Should().HaveFlag(EventTypeFlags.Modify);
            result[2].Value.Should().BeEquivalentTo(TestData.TestPod1ResourceVersion2);
        }

        [Fact]
        public async Task WatchWithRetryPolicy_WhenApiCallThrowsTransient_ShouldRetry()
        {
            var kubernetes = Substitute.For<IKubernetes>();
            kubernetes.ListWithHttpMessagesAsync<V1Pod>().ThrowsForAnyArgs(info => new HttpRequestException());
            var sut = new KubernetesInformer<V1Pod>(kubernetes, new RetryPolicy((e, i) => i < 2, i => TimeSpan.Zero), () => false);
            Func<Task> act = async () => await sut.GetResource(ResourceStreamType.ListWatch).ToList().TimeoutIfNotDebugging();
            act.Should().Throw<HttpRequestException>();
            await kubernetes.ReceivedWithAnyArgs(2).ListWithHttpMessagesAsync<V1Pod>();
            await kubernetes.Received().ListWithHttpMessagesAsync<V1Pod>(cancellationToken: Arg.Any<CancellationToken>());
        }
        [Fact]
        public void Watch_InterruptedWatchAndGoneResourceVersion_ShouldReList()
        {
            var kubernetes = Substitute.For<IKubernetes>();

            kubernetes.ListWithHttpMessagesAsync<V1Pod>(cancellationToken: Arg.Any<CancellationToken>())
                .Returns(
                    _ => TestData.ListPodEmpty.ToHttpOperationResponse<V1PodList, V1Pod>(),
                    _ => throw new TestCompleteException());
            kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    cancellationToken: Arg.Any<CancellationToken>(),
                    watch: true,
                    allowWatchBookmarks: true,
                    resourceVersion: TestData.ListPodEmpty.Metadata.ResourceVersion)
                .Returns(TestData.TestPod1ResourceVersion1.ToWatchEvent(WatchEventType.Added).ToHttpOperationResponse());
            kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    cancellationToken: Arg.Any<CancellationToken>(),
                    watch: true,
                    allowWatchBookmarks: true,
                    resourceVersion: TestData.TestPod1ResourceVersion1.Metadata.ResourceVersion)
                .Returns(new HttpOperationResponse<KubernetesList<V1Pod>>() { Response = new HttpResponseMessage() { StatusCode = HttpStatusCode.Gone } });

            var sut = new KubernetesInformer<V1Pod>(kubernetes, RetryPolicy.None, () => true);
            Func<Task> act = async () => await sut.GetResource(ResourceStreamType.ListWatch).ToList().TimeoutIfNotDebugging();
            act.Should().Throw<TestCompleteException>();

            Received.InOrder(() =>
            {
                // initial list
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(cancellationToken: Arg.Any<CancellationToken>());

                // watch after list
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    cancellationToken: Arg.Any<CancellationToken>(),
                    watch: true,
                    allowWatchBookmarks: true,
                    resourceVersion: TestData.ListPodEmpty.Metadata.ResourceVersion);

                // resume watch with same resource version - server responded with gone
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    cancellationToken: Arg.Any<CancellationToken>(),
                    watch: true,
                    allowWatchBookmarks: true,
                    resourceVersion: TestData.TestPod1ResourceVersion1.Metadata.ResourceVersion);
                // restart the whole thing with list without version
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(cancellationToken: Arg.Any<CancellationToken>());
            });
        }
        [Fact]
        public void Watch_BookmarkInterrupted_ShouldRewatchWithBookmarkResourceVersion()
        {
            var kubernetes = Substitute.For<IKubernetes>();

            kubernetes.ListWithHttpMessagesAsync<V1Pod>(cancellationToken: Arg.Any<CancellationToken>())
                .Returns(_ => TestData.ListPodEmpty.ToHttpOperationResponse<V1PodList, V1Pod>());
            kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    watch: true,
                    resourceVersion: TestData.ListPodEmpty.Metadata.ResourceVersion,
                    allowWatchBookmarks: true,
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(
                _ => new V1Pod()
                {
                    Kind = V1Pod.KubeKind,
                    ApiVersion = V1Pod.KubeApiVersion,
                    Metadata = new V1ObjectMeta()
                    {
                        ResourceVersion = TestData.ListPodOneItem.Metadata.ResourceVersion
                    }
                }
                    .ToWatchEvent(WatchEventType.Bookmark)
                    .ToHttpOperationResponse());
            kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    watch: true,
                    allowWatchBookmarks: true,
                    resourceVersion: TestData.ListPodOneItem.Metadata.ResourceVersion,
                    cancellationToken: Arg.Any<CancellationToken>())
                .Throws<TestCompleteException>();

            var sut = new KubernetesInformer<V1Pod>(kubernetes, RetryPolicy.None, () => true);
            Func<Task> act = async () => await sut.GetResource(ResourceStreamType.ListWatch).ToList().TimeoutIfNotDebugging();

            act.Should().Throw<TestCompleteException>();
            Received.InOrder(() =>
            {
                // initial list
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(cancellationToken: Arg.Any<CancellationToken>());
                // watch after list with same version as returned by list - receive bookmark with new version
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    resourceVersion: TestData.ListPodEmpty.Metadata.ResourceVersion,
                    cancellationToken: Arg.Any<CancellationToken>(),
                    watch: true,
                    allowWatchBookmarks: true);
                // resume watch with bookmark version
                kubernetes.ListWithHttpMessagesAsync<V1Pod>(
                    resourceVersion: TestData.ListPodOneItem.Metadata.ResourceVersion,
                    cancellationToken: Arg.Any<CancellationToken>(),
                    watch: true,
                    allowWatchBookmarks: true);
            });
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
