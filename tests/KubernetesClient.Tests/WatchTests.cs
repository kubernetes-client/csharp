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
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class WatchTests
    {
        private static readonly string MockAddedEventStreamLine = BuildWatchEventStreamLine(WatchEventType.Added);
        private static readonly string MockDeletedStreamLine = BuildWatchEventStreamLine(WatchEventType.Deleted);
        private static readonly string MockModifiedStreamLine = BuildWatchEventStreamLine(WatchEventType.Modified);
        private static readonly string MockErrorStreamLine = BuildWatchEventStreamLine(WatchEventType.Error);
        private static readonly string MockBadStreamLine = "bad json";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(150);

        private readonly ITestOutputHelper testOutput;

        public WatchTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
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
        public async Task CannotWatch()
        {
            using (var server = new MockKubeApiServer(testOutput: testOutput))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                // did not pass watch param
                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default");
                Assert.ThrowsAny<KubernetesClientException>(() =>
                {
                    listTask.Watch<V1Pod>((type, item) => { });
                });

                // server did not response line by line
                await Assert.ThrowsAnyAsync<Exception>(() =>
                {
                    return client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                    // this line did not throw
                    // listTask.Watch<Corev1Pod>((type, item) => { });
                });
            }
        }

        [Fact]
        public async Task SuriveBadLine()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(4 /* first line of response is eaten by WatcherDelegatingHandler */);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();

            using (var server =
                new MockKubeApiServer(
                    testOutput,
                    async httpContext =>
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        httpContext.Response.ContentLength = null;

                        await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                        await WriteStreamLine(httpContext, MockBadStreamLine);
                        await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                        await WriteStreamLine(httpContext, MockBadStreamLine);
                        await WriteStreamLine(httpContext, MockModifiedStreamLine);

                        // make server alive, cannot set to int.max as of it would block response
                        await serverShutdown.WaitAsync();
                        return false;
                    }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        testOutput.WriteLine($"Watcher received '{type}' event.");

                        events.Add(type);
                        eventsReceived.Signal();
                    },
                    error =>
                    {
                        testOutput.WriteLine($"Watcher received '{error.GetType().FullName}' error.");

                        errors += 1;
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Modified, events);

                Assert.Equal(2, errors);

                Assert.True(watcher.Watching);

                // Let the server know it can initiate a shut down.
                serverShutdown.Set();
            }
        }

        [Fact]
        public async Task DisposeWatch()
        {
            var eventsReceived = new AsyncCountdownEvent(1);
            bool serverRunning = true;

            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);

                while (serverRunning)
                {
                    await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                }

                return true;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                var events = new HashSet<WatchEventType>();

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        events.Add(type);
                        eventsReceived.Signal();
                    }
                );

                // wait at least an event
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));
                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for events."
                );

                Assert.NotEmpty(events);
                Assert.True(watcher.Watching);

                watcher.Dispose();

                events.Clear();

                // Let the server disconnect
                serverRunning = false;

                var timeout = Task.Delay(TestTimeout);

                while(!timeout.IsCompleted && watcher.Watching)
                {
                    await Task.Yield();
                }

                Assert.Empty(events);
                Assert.False(watcher.Watching);
            }
        }

        [Fact]
        public async Task WatchAllEvents()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(4 /* first line of response is eaten by WatcherDelegatingHandler */);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();

            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await WriteStreamLine(httpContext, MockDeletedStreamLine);
                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await WriteStreamLine(httpContext, MockErrorStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await serverShutdown.WaitAsync();
                return false;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        testOutput.WriteLine($"Watcher received '{type}' event.");

                        events.Add(type);
                        eventsReceived.Signal();
                    },
                    error =>
                    {
                        testOutput.WriteLine($"Watcher received '{error.GetType().FullName}' error.");

                        errors += 1;
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Deleted, events);
                Assert.Contains(WatchEventType.Modified, events);
                Assert.Contains(WatchEventType.Error, events);

                Assert.Equal(0, errors);

                Assert.True(watcher.Watching);

                serverShutdown.Set();
            }
        }

        [Fact]
        public async Task WatchEventsWithTimeout()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(4 /* first line of response is eaten by WatcherDelegatingHandler */);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();

            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await Task.Delay(TimeSpan.FromSeconds(120)); // The default timeout is 100 seconds
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await WriteStreamLine(httpContext, MockDeletedStreamLine);
                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await WriteStreamLine(httpContext, MockErrorStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await serverShutdown.WaitAsync();
                return false;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        testOutput.WriteLine($"Watcher received '{type}' event.");

                        events.Add(type);
                        eventsReceived.Signal();
                    },
                    error =>
                    {
                        testOutput.WriteLine($"Watcher received '{error.GetType().FullName}' error.");

                        errors += 1;
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Deleted, events);
                Assert.Contains(WatchEventType.Modified, events);
                Assert.Contains(WatchEventType.Error, events);

                Assert.Equal(0, errors);

                Assert.True(watcher.Watching);

                serverShutdown.Set();
            }
        }

        [Fact]
        public async Task WatchServerDisconnect()
        {
            Exception exceptionCatched = null;
            var exceptionReceived = new AsyncManualResetEvent(false);
            var waitForException = new AsyncManualResetEvent(false);
            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await waitForException.WaitAsync();
                throw new IOException("server down");
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                waitForException.Set();
                Watcher<V1Pod> watcher;
                watcher = listTask.Watch<V1Pod>(
                    (type, item) => { },
                    e =>
                    {
                        exceptionCatched = e;
                        exceptionReceived.Set();
                    });

                // wait server down
                await Task.WhenAny(exceptionReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    exceptionReceived.IsSet,
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
        public async Task TestWatchWithHandlers()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(1);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();

            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await serverShutdown.WaitAsync();
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

                var listTask = await client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true);

                var events = new HashSet<WatchEventType>();

                var watcher = listTask.Watch<V1Pod>(
                    (type, item) =>
                    {
                        events.Add(type);
                        eventsReceived.Signal();
                    }
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                     eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);

                Assert.True(handler1.Called);
                Assert.True(handler2.Called);

                serverShutdown.Set();
            }
        }

        [Fact]
        public async Task DirectWatchAllEvents()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(4);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();

            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await WriteStreamLine(httpContext, MockDeletedStreamLine);
                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await WriteStreamLine(httpContext, MockErrorStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await serverShutdown.WaitAsync();
                return false;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = await client.WatchNamespacedPodAsync(
                    name: "myPod",
                    @namespace: "default",
                    onEvent:
                        (type, item) =>
                        {
                            testOutput.WriteLine($"Watcher received '{type}' event.");

                            events.Add(type);
                            eventsReceived.Signal();
                        },
                    onError:
                        error =>
                        {
                            testOutput.WriteLine($"Watcher received '{error.GetType().FullName}' error.");

                            errors += 1;
                            eventsReceived.Signal();
                        }
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Deleted, events);
                Assert.Contains(WatchEventType.Modified, events);
                Assert.Contains(WatchEventType.Error, events);

                Assert.Equal(0, errors);

                Assert.True(watcher.Watching);

                serverShutdown.Set();
            }
        }

        [Fact (Skip = "https://github.com/kubernetes-client/csharp/issues/165")]
        public async Task DirectWatchEventsWithTimeout()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(4);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();

            using (var server = new MockKubeApiServer(testOutput, async httpContext =>
            {
                await Task.Delay(TimeSpan.FromSeconds(120)); // The default timeout is 100 seconds
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await WriteStreamLine(httpContext, MockDeletedStreamLine);
                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await WriteStreamLine(httpContext, MockErrorStreamLine);

                // make server alive, cannot set to int.max as of it would block response
                await serverShutdown.WaitAsync();
                return false;
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var events = new HashSet<WatchEventType>();
                var errors = 0;

                var watcher = await client.WatchNamespacedPodAsync(
                    name: "myPod",
                    @namespace: "default",
                    onEvent:
                        (type, item) =>
                        {
                            testOutput.WriteLine($"Watcher received '{type}' event.");

                            events.Add(type);
                            eventsReceived.Signal();
                        },
                    onError:
                        error =>
                        {
                            testOutput.WriteLine($"Watcher received '{error.GetType().FullName}' error.");

                            errors += 1;
                            eventsReceived.Signal();
                        }
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Deleted, events);
                Assert.Contains(WatchEventType.Modified, events);
                Assert.Contains(WatchEventType.Error, events);

                Assert.Equal(0, errors);

                Assert.True(watcher.Watching);

                serverShutdown.Set();
            }
        }

    }
}
