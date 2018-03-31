using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using k8s.Exceptions;
using k8s.Models;
using k8s.Tests.Mock;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class WatchTests
        : TestBase
    {
        private static readonly string MockAddedEventStreamLine = BuildWatchEventStreamLine(WatchEventType.Added);
        private static readonly string MockDeletedStreamLine = BuildWatchEventStreamLine(WatchEventType.Deleted);
        private static readonly string MockModifiedStreamLine = BuildWatchEventStreamLine(WatchEventType.Modified);
        private static readonly string MockErrorStreamLine = BuildWatchEventStreamLine(WatchEventType.Error);
        private static readonly string MockBadStreamLine = "bad json";

        public WatchTests(ITestOutputHelper testOutput) : base(testOutput)
        {
        }

        private static string BuildWatchEventStreamLine(WatchEventType eventType)
        {
            var corev1PodList = JsonConvert.DeserializeObject<V1PodList>(MockKubeApiServer.MockPodResponse);
            return JsonConvert.SerializeObject(new Watcher<V1Pod>.WatchEvent
            {
                Type = eventType,
                Object = corev1PodList.Items.First()
            }, new StringEnumConverter());
        }

        private static async Task WriteStreamLine(HttpContext httpContext, string reponseLine)
        {
            const string crlf = "\r\n";
            await httpContext.Response.WriteAsync(reponseLine.Replace(crlf, ""));
            await httpContext.Response.WriteAsync(crlf);
            await httpContext.Response.Body.FlushAsync();
        }

        [Fact]
        public void CannotWatch()
        {
            using (var server = new MockKubeApiServer(testOutput: TestOutput))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                // did not pass watch param
                {
                    var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default").Result;
                    Assert.ThrowsAny<KubernetesClientException>(() =>
                    {
                        listTask.Watch<V1Pod>((type, item) => { });
                    });
                }

                // server did not response line by line
                {
                    Assert.ThrowsAny<Exception>(() =>
                    {
                        var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;

                        // this line did not throw
                        // listTask.Watch<Corev1Pod>((type, item) => { });
                    });
                }
            }
        }

        [Fact]
        public void SuriveBadLine()
        {
            using (CountdownEvent eventsReceived = new CountdownEvent(4 /* first line of response is eaten by WatcherDelegatingHandler */))
            using (var server = new MockKubeApiServer(TestOutput, async httpContext =>
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
                httpContext.Response.ContentLength = null;

                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await WriteStreamLine(httpContext, MockBadStreamLine);
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await WriteStreamLine(httpContext, MockBadStreamLine);
                await WriteStreamLine(httpContext, MockModifiedStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await Task.Delay(TimeSpan.FromDays(1));
                return false;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;

                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        Log.LogInformation("Watcher received '{EventType}' event.", type);

                        events.Add(type);
                        eventsReceived.Signal();
                    },
                    error =>
                    {
                        Log.LogInformation("Watcher received '{ErrorType}' error.", error.GetType().FullName);

                        errors += 1;
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                Assert.True(
                    eventsReceived.Wait(TimeSpan.FromMilliseconds(3000)),
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Modified, events);

                Assert.Equal(2, errors);

                Assert.True(watcher.Watching);

                // prevent from server down exception trigger
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }
        }

        [Fact]
        public void DisposeWatch()
        {
            using (var eventsReceived = new CountdownEvent(1))
            using (var server = new MockKubeApiServer(TestOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);

                for (;;)
                {
                    await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                }
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;


                var events = new HashSet<WatchEventType>();

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) => {
                        events.Add(type);
                        eventsReceived.Signal();
                    }
                );

                // wait at least an event
                Assert.True(
                    eventsReceived.Wait(TimeSpan.FromSeconds(10)),
                    "Timed out waiting for events."
                );

                Assert.NotEmpty(events);
                Assert.True(watcher.Watching);

                watcher.Dispose();

                events.Clear();

                // make sure wait event called
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                Assert.Empty(events);
                Assert.False(watcher.Watching);

            }
        }

        [Fact]
        public void WatchAllEvents()
        {
            using (CountdownEvent eventsReceived = new CountdownEvent(4 /* first line of response is eaten by WatcherDelegatingHandler */))
            using (var server = new MockKubeApiServer(TestOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await WriteStreamLine(httpContext, MockDeletedStreamLine);
                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await WriteStreamLine(httpContext, MockErrorStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await Task.Delay(TimeSpan.FromDays(1));
                return false;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;


                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        Log.LogInformation("Watcher received '{EventType}' event.", type);

                        events.Add(type);
                        eventsReceived.Signal();
                    },
                    error =>
                    {
                        Log.LogInformation("Watcher received '{ErrorType}' error.", error.GetType().FullName);

                        errors += 1;
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                Assert.True(
                    eventsReceived.Wait(TimeSpan.FromMilliseconds(3000)),
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Deleted, events);
                Assert.Contains(WatchEventType.Modified, events);
                Assert.Contains(WatchEventType.Error, events);


                Assert.Equal(0, errors);

                Assert.True(watcher.Watching);
            }
        }

        [Fact]
        public void WatchServerDisconnect()
        {
            Exception exceptionCatched = null;
            using (var exceptionReceived = new AutoResetEvent(false))
            using (var waitForException = new AutoResetEvent(false))
            using (var server = new MockKubeApiServer(TestOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                waitForException.WaitOne();
                throw new IOException("server down");
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;

                waitForException.Set();
                Watcher<V1Pod> watcher;
                watcher = listTask.Watch<V1Pod>(
                    (type, item) => { },
                    e => {
                        exceptionCatched = e;
                        exceptionReceived.Set();
                    });

                // wait server down
                Assert.True(
                    exceptionReceived.WaitOne(TimeSpan.FromSeconds(10)),
                    "Timed out waiting for exception"
                );

                Assert.False(watcher.Watching);
                Assert.IsType<IOException>(exceptionCatched);
            }
        }

        private class DummyHandler : DelegatingHandler
        {
            internal bool Called { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Called = true;
                return base.SendAsync(request, cancellationToken);
            }
        }

        [Fact]
        public void TestWatchWithHandlers()
        {
            using (CountdownEvent eventsReceived = new CountdownEvent(1))
            using (var server = new MockKubeApiServer(TestOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await Task.Delay(TimeSpan.FromDays(1));
                return false;
            }))
            {
                var handler1 = new DummyHandler();
                var handler2 = new DummyHandler();

                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                }, handler1, handler2);

                Assert.False(handler1.Called);
                Assert.False(handler2.Called);

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;

                var events = new HashSet<WatchEventType>();

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) => {
                        events.Add(type);
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                Assert.True(
                     eventsReceived.Wait(TimeSpan.FromMilliseconds(10000)),
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);

                Assert.True(handler1.Called);
                Assert.True(handler2.Called);
            }
        }
    }
}
