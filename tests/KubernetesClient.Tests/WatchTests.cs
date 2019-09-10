using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    class MockLineStreamReader : IAsyncLineReader
    {
        private readonly List<string> _lines;
        private readonly int _blockAtLine;
        private readonly int _exceptionAtLine;
        private int _pos;

        public MockLineStreamReader(
            IEnumerable<string> lines,
            int blockAtLine = -1,
            int exceptionAtLine = -1)
        {
            _blockAtLine = blockAtLine;
            _exceptionAtLine = exceptionAtLine;
            _lines = lines.ToList();
        }

        public async Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            if (_exceptionAtLine == _pos)
            {
                throw new Exception("io failure");
            }
            if (_blockAtLine == _pos)
            {
                try
                {
                    await Task.Delay(-1, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
            }
            return _pos < _lines.Count ? _lines[_pos++] : null;
        }

        public void Dispose()
        {
        }
    }

    class MockOtherHttpContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            throw new NotImplementedException();

        protected override bool TryComputeLength(out long length) =>
            throw new NotImplementedException();
    }

    class MockLineSeparatedHttpContent : HttpContent, ILineSeparatedHttpContent
    {
        public IAsyncLineReader StreamReader { get; private set; }

        public MockLineSeparatedHttpContent(IAsyncLineReader streamReader)
        {
            StreamReader = streamReader;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            Task.CompletedTask;

        protected override bool TryComputeLength(out long length) =>
            throw new NotImplementedException();
    }

    public class WatchTests
    {
        private static readonly string MockAddedEventStreamLine = BuildWatchEventStreamLine(WatchEventType.Added);
        private static readonly string MockDeletedStreamLine = BuildWatchEventStreamLine(WatchEventType.Deleted);
        private static readonly string MockModifiedStreamLine = BuildWatchEventStreamLine(WatchEventType.Modified);
        private static readonly string MockErrorStreamLine = BuildWatchEventStreamLine(WatchEventType.Error);
        private static readonly string MockBadStreamLine = "bad json";
        private static readonly string MockStatusLine = JsonConvert.SerializeObject(
            new Watcher<V1Status>.WatchEvent
            {
                Type = WatchEventType.Error,
                Object = new V1Status(kind: "Status", reason: "test status")
            }, new StringEnumConverter());
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
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(5);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();
            AsyncManualResetEvent connectionClosed = new AsyncManualResetEvent();

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
                    },
                    onClosed: connectionClosed.Set
                );

                // wait server yields all events
                await Task.WhenAny(eventsReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    eventsReceived.CurrentCount == 0,
                    "Timed out waiting for all events / errors to be received."
                );

                Assert.Contains(WatchEventType.Added, events);
                Assert.Contains(WatchEventType.Modified, events);

                Assert.Equal(3, errors);

                Assert.True(watcher.Watching);

                // Let the server know it can initiate a shut down.
                serverShutdown.Set();

                await Task.WhenAny(connectionClosed.WaitAsync(), Task.Delay(TestTimeout));
                Assert.True(connectionClosed.IsSet);
            }
        }

        [Fact]
        public async Task DisposeWatch()
        {
            var connectionClosed = new AsyncManualResetEvent();
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
                    },
                    onClosed: connectionClosed.Set
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

                while (!timeout.IsCompleted && watcher.Watching)
                {
                    await Task.Yield();
                }

                Assert.Empty(events);
                Assert.False(watcher.Watching);
                Assert.True(connectionClosed.IsSet);
            }
        }

        [Fact]
        public async Task WatchAllEvents()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(4 /* first line of response is eaten by WatcherDelegatingHandler */);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();
            var waitForClosed = new AsyncManualResetEvent(false);

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
                    },
                    onClosed: waitForClosed.Set
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

                await Task.WhenAny(waitForClosed.WaitAsync(), Task.Delay(TestTimeout));
                Assert.True(waitForClosed.IsSet);
                Assert.False(watcher.Watching);
            }
        }

        [Fact]
        public async Task WatchEventsWithTimeout()
        {
            AsyncCountdownEvent eventsReceived = new AsyncCountdownEvent(5);
            AsyncManualResetEvent serverShutdown = new AsyncManualResetEvent();
            AsyncManualResetEvent connectionClosed = new AsyncManualResetEvent();

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
                    },
                    onClosed: connectionClosed.Set
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

                Assert.Equal(1, errors);

                Assert.True(watcher.Watching);

                serverShutdown.Set();

                await Task.WhenAny(connectionClosed.WaitAsync(), Task.Delay(TestTimeout));
                Assert.True(connectionClosed.IsSet);
            }
        }

        [Fact]
        public async Task WatchServerDisconnect()
        {
            Exception exceptionCatched = null;
            var exceptionReceived = new AsyncManualResetEvent(false);
            var waitForException = new AsyncManualResetEvent(false);
            var waitForClosed = new AsyncManualResetEvent(false);

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
                    onEvent: (type, item) => { },
                    onError: e =>
                    {
                        exceptionCatched = e;
                        exceptionReceived.Set();
                    },
                    onClosed: waitForClosed.Set);

                // wait server down
                await Task.WhenAny(exceptionReceived.WaitAsync(), Task.Delay(TestTimeout));

                Assert.True(
                    exceptionReceived.IsSet,
                    "Timed out waiting for exception"
                );

                await Task.WhenAny(waitForClosed.WaitAsync(), Task.Delay(TestTimeout));
                Assert.True(waitForClosed.IsSet);
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
            AsyncManualResetEvent connectionClosed = new AsyncManualResetEvent();

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
                        },
                    onClosed: connectionClosed.Set
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

                await Task.WhenAny(connectionClosed.WaitAsync(), Task.Delay(TestTimeout));
                Assert.True(connectionClosed.IsSet);
            }
        }

        [Fact(Skip = "Integration Test")]
        public async Task WatcherIntegrationTest()
        {
            var kubernetesConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeconfigPath: @"C:\Users\frede\Source\Repos\cloud\minikube.config");
            var kubernetes = new Kubernetes(kubernetesConfig);

            var job = await kubernetes.CreateNamespacedJobAsync(
                new V1Job()
                {
                    ApiVersion = "batch/v1",
                    Kind = V1Job.KubeKind,
                    Metadata = new V1ObjectMeta()
                    {
                        Name = nameof(WatcherIntegrationTest).ToLowerInvariant()
                    },
                    Spec = new V1JobSpec()
                    {

                        Template = new V1PodTemplateSpec()
                        {
                            Spec = new V1PodSpec()
                            {
                                Containers = new List<V1Container>()
                                {
                                    new V1Container()
                                    {
                                        Image = "ubuntu/xenial",
                                        Name = "runner",
                                        Command = new List<string>()
                                        {
                                            "/bin/bash",
                                            "-c",
                                            "--"
                                        },
                                        Args = new List<string>()
                                        {
                                            "trap : TERM INT; sleep infinity & wait"
                                        }
                                    }
                                },
                                RestartPolicy = "Never"
                            },
                        }
                    }
                },
                "default");

            Collection<Tuple<WatchEventType, V1Job>> events = new Collection<Tuple<WatchEventType, V1Job>>();

            AsyncManualResetEvent started = new AsyncManualResetEvent();
            AsyncManualResetEvent connectionClosed = new AsyncManualResetEvent();

            var watcher = await kubernetes.WatchNamespacedJobAsync(
                job.Metadata.Name,
                job.Metadata.NamespaceProperty,
                resourceVersion: job.Metadata.ResourceVersion,
                timeoutSeconds: 30,
                onEvent:
                (type, source) =>
                {
                    Debug.WriteLine($"Watcher 1: {type}, {source}");
                    events.Add(new Tuple<WatchEventType, V1Job>(type, source));
                    job = source;
                    started.Set();
                },
                onClosed: connectionClosed.Set).ConfigureAwait(false);

            await started.WaitAsync();

            await Task.WhenAny(connectionClosed.WaitAsync(), Task.Delay(TimeSpan.FromMinutes(3)));
            Assert.True(connectionClosed.IsSet);

            await kubernetes.DeleteNamespacedJobAsync(
                job.Metadata.Name,
                job.Metadata.NamespaceProperty,
                new V1DeleteOptions());
        }

        [Fact(Skip = "https://github.com/kubernetes-client/csharp/issues/165")]
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

        [Fact]
        public void WatchFailsWithIncorrectContentType()
        {
            var ct = CancellationToken.None;
            var response = new HttpOperationResponse<V1Pod>
            {
                Response = new HttpResponseMessage
                {
                    Content = new MockOtherHttpContent()
                }
            };
            Assert.ThrowsAsync<KubernetesClientException>(
                async () => await response.WatchAsync(
                    (ct_, evt, pod) => Task.CompletedTask,
                    cancellationToken: ct));
        }

        private HttpOperationResponse<V1Pod> MakeResponse(MockLineStreamReader reader)
        {
            var content = new MockLineSeparatedHttpContent(reader);
            var response = new HttpOperationResponse<V1Pod>
            {
                Response = new HttpResponseMessage
                {
                    Content = content
                }
            };
            return response;
        }

        [Fact]
        public async Task WatchCanBeCancelled()
        {
            var reader = new MockLineStreamReader(new string[0], 0);
            var response = MakeResponse(reader);
            var cts = new CancellationTokenSource();
            var task = response.WatchAsync((ct, evt, pod) => Task.CompletedTask,
                cancellationToken: cts.Token);
            cts.Cancel();
            await task;
        }

        [Fact]
        public async Task WatchCanBeCancelledWithoutLSRReturningNull()
        {
            var reader = new MockLineStreamReader(new string[]
            {
                MockAddedEventStreamLine
            }, 0);
            var response = MakeResponse(reader);
            var cts = new CancellationTokenSource();
            var task = response.WatchAsync((ct, evt, pod) => Task.CompletedTask,
                cancellationToken: cts.Token);
            cts.Cancel();
            await task;
        }

        [Fact]
        public async Task WatchStopsOnIoException()
        {
            var reader = new MockLineStreamReader(new string[0], -1, 0);
            var response = MakeResponse(reader);
            var cts = new CancellationTokenSource();

            var events = new List<WatchEventType>();
            var exceptions = new List<Exception>();
            var onCloseCalled = false;

            var task = response.WatchAsync(
                (ct, evt, pod) => Task.CompletedTask,
                (ct, ex) =>
                {
                    exceptions.Add(ex);
                    return Task.CompletedTask;
                },
                (ct) =>
                {
                    onCloseCalled = true;
                    return Task.CompletedTask;
                }, CancellationToken.None);
            await task;

            Assert.True(onCloseCalled);
            Assert.Single(exceptions);
            Assert.Equal("io failure", exceptions[0].Message);
        }


        [Fact]
        public async Task WatchHandlesAllTypes()
        {
            var reader = new MockLineStreamReader(new[]
            {
                MockAddedEventStreamLine,
                MockBadStreamLine,
                MockModifiedStreamLine,
                MockBadStreamLine,
                MockDeletedStreamLine,
                MockBadStreamLine,
                MockErrorStreamLine,
                MockStatusLine
            });
            var response = MakeResponse(reader);

            var ct = CancellationToken.None;
            var events = new List<WatchEventType>();
            var exceptions = new List<Exception>();
            var onCloseCalled = false;
            await response.WatchAsync(
                (ct_, evt, pod) =>
                {
                    events.Add(evt);
                    return Task.CompletedTask;
                },
                (ct_, ex) =>
                {
                    exceptions.Add(ex);
                    return Task.CompletedTask;
                },
                (ct_) =>
                {
                    onCloseCalled = true;
                    return Task.CompletedTask;
                }, ct);

            Assert.True(onCloseCalled);
            Assert.Equal(new WatchEventType[]
            {
                WatchEventType.Added,
                WatchEventType.Modified,
                WatchEventType.Deleted,
                WatchEventType.Error
            }, events);
            Assert.Equal(4, exceptions.Count);
            Assert.IsType<JsonReaderException>(exceptions[0]);
            Assert.IsType<JsonReaderException>(exceptions[1]);
            Assert.IsType<JsonReaderException>(exceptions[2]);
            Assert.IsType<KubernetesException>(exceptions[3]);
            var k8se = (KubernetesException)exceptions[3];
            Assert.Equal("test status", k8se.Status.Reason);
        }
    }
}
