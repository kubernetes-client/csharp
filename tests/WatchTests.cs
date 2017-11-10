using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using k8s.Exceptions;
using k8s.Models;
using k8s.Tests.Mock;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;

namespace k8s.Tests
{
    public class WatchTests
    {
        private static readonly string MockAddedEventStreamLine = BuildWatchEventStreamLine(WatchEventType.Added);
        private static readonly string MockDeletedStreamLine = BuildWatchEventStreamLine(WatchEventType.Deleted);
        private static readonly string MockModifiedStreamLine = BuildWatchEventStreamLine(WatchEventType.Modified);
        private static readonly string MockErrorStreamLine = BuildWatchEventStreamLine(WatchEventType.Error);
        private static readonly string MockBadStreamLine = "bad json";

        private static string BuildWatchEventStreamLine(WatchEventType eventType)
        {
            var corev1PodList = JsonConvert.DeserializeObject<Corev1PodList>(MockKubeApiServer.MockPodResponse);
            return JsonConvert.SerializeObject(new Watcher<Corev1Pod>.WatchEvent
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
            using (var server = new MockKubeApiServer())
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
                        listTask.Watch<Corev1Pod>((type, item) => { });
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
            using (var server = new MockKubeApiServer(async httpContext =>
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
                httpContext.Response.ContentLength = null;

                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockBadStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockBadStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

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

                var watcher = listTask.Watch<Corev1Pod>(
                    (type, item) => { events.Add(type); },
                    e => { errors += 1; }
                );

                // wait server yields all events
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));

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
            using (var server = new MockKubeApiServer(async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                for (;;)
                {
                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                    
                }
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;


                var events = new HashSet<WatchEventType>();

                var watcher = listTask.Watch<Corev1Pod>(
                    (type, item) => { events.Add(type); }
                );

                // wait at least an event
                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                Assert.NotEmpty(events);
                Assert.True(watcher.Watching);

                watcher.Dispose();

                events.Clear();

                // make sure wait event called
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                Assert.Empty(events);
                Assert.False(watcher.Watching);
                
            }
        }

        [Fact]
        public void WatchAllEvents()
        {
            using (var server = new MockKubeApiServer(async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockAddedEventStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockDeletedStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockModifiedStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

                await WriteStreamLine(httpContext, MockErrorStreamLine);
                await Task.Delay(TimeSpan.FromMilliseconds(100));

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

                var watcher = listTask.Watch<Corev1Pod>(
                    (type, item) => { events.Add(type); },
                    e => { errors += 1; }
                );

                // wait server yields all events
                Thread.Sleep(TimeSpan.FromMilliseconds(750));

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
            Watcher<Corev1Pod> watcher;
            Exception exceptionCatched = null;

            using (var server = new MockKubeApiServer(async httpContext =>
            {
                await WriteStreamLine(httpContext, MockKubeApiServer.MockPodResponse);

                // make sure watch success
                await Task.Delay(TimeSpan.FromMilliseconds(200));

                throw new IOException("server down");
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default", watch: true).Result;

                watcher = listTask.Watch<Corev1Pod>(
                    (type, item) => { },
                    e => { exceptionCatched = e; });
            }

            // wait server down
            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            Assert.False(watcher.Watching);
            Assert.IsType<IOException>(exceptionCatched);
        }
    }
}
