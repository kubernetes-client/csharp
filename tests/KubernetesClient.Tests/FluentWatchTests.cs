using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using k8s.Tests.Mock;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace k8s.Tests
{
    public class FluentWatchTests
    {
        [Fact]
        public async Task TestWatchReader()
        {
            Assert.True(new WatchReader<V1Pod>(new KubernetesResponse(new HttpResponseMessage(HttpStatusCode.BadRequest))).IsError);

            for (int i = 0; i < 2; i++) // run twice, once using a Stream and again using an HttpResponseMessage
            {
                using (var stream = new PipeStream())
                {
                    var respMsg = i == 0 ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) } : null;
                    var events = new BlockingCollection<WatchEvent<V1Pod>>();
                    var cts = new CancellationTokenSource();
                    using (var wr = i == 0 ? new WatchReader<V1Pod>(new KubernetesResponse(respMsg)) : new WatchReader<V1Pod>(stream))
                    {
                        Assert.False(wr.IsError);

                        Task task = Task.Run(async () => // start a background task to read items from the watch
                        {
                            WatchEvent<V1Pod> we;
                            try
                            {
                                while ((we = await wr.ReadAsync(cts.Token)) != null) events.Add(we);
                            }
                            catch (OperationCanceledException) { }
                            events.CompleteAdding();
                        });

                        void writeEvent(WatchEventType t, object o) =>
                            stream.QueueData(Encoding.UTF8.GetBytes(
                                new JObject(new JProperty("type", t.ToString()), new JProperty("object", JObject.FromObject(o))).ToString()));

                        WatchEvent<V1Pod> e;
                        Assert.False(events.TryTake(out e, 100));

                        var pod = new V1Pod() { ApiVersion = "v1", Kind = "Pod" };
                        pod.Metadata = new V1ObjectMeta() { Name = "pod", NamespaceProperty = "ns", ResourceVersion = "1", Uid = "id" };
                        pod.Spec = new V1PodSpec();
                        writeEvent(WatchEventType.Added, pod);
                        pod.SetLabel("a", "b");
                        pod.Metadata.ResourceVersion = "2";
                        writeEvent(WatchEventType.Modified, pod);
                        pod.Metadata.ResourceVersion = "3";
                        pod.Spec = null;
                        writeEvent(WatchEventType.Bookmark, pod);
                        writeEvent(WatchEventType.Deleted, pod);
                        writeEvent(WatchEventType.Error, new V1Status() { Status = "Failure", Code = 410, Reason = "Gone" });

                        Assert.True(events.TryTake(out e, 1000));
                        Assert.Equal(WatchEventType.Added, e.Type);
                        Assert.Equal("1", e.Object.ResourceVersion());
                        Assert.True(events.TryTake(out e, 1000));
                        Assert.Equal(WatchEventType.Modified, e.Type);
                        Assert.Equal("2", e.Object.ResourceVersion());
                        Assert.Equal("b", e.Object.GetLabel("a"));
                        Assert.True(events.TryTake(out e, 1000));
                        Assert.Equal(WatchEventType.Bookmark, e.Type);
                        Assert.Equal("3", e.Object.ResourceVersion());
                        Assert.True(events.TryTake(out e, 1000));
                        Assert.Equal(WatchEventType.Deleted, e.Type);
                        Assert.Equal("3", e.Object.ResourceVersion());
                        Assert.True(events.TryTake(out e, 1000));
                        Assert.Null(e.Object);
                        Assert.Equal("Failure", e.Error.Status);
                        Assert.Equal(410, e.Error.Code.Value);

                        cts.Cancel();
                        await task;
                        Assert.False(events.TryTake(out e));
                    }
                }
            }
        }

        [Fact]
        public async Task TestSingleItemWatch()
        {
            Pipe pipe = null;
            void writeEvent(WatchEventType t, object o) =>
                pipe.Write(Encoding.UTF8.GetBytes(
                    new JObject(new JProperty("type", t.ToString()), new JProperty("object", JObject.FromObject(o))).ToString()));

            var pipeResetEvent = new AutoResetEvent(false);
            PipeStream stream = null;
            var handler = new MockHttpHandler(_ =>
            {
                pipe = new Pipe();
                pipeResetEvent.Set();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream = new PipeStream(pipe, true)) };
            });

            var c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost" }, new HttpClient(handler));
            Assert.True(c.Request<V1Pod>("ns").ToWatch<V1Pod>().IsListWatch);
            Assert.False(c.Request<V1Pod>("ns", "name").ToWatch<V1Pod>().IsListWatch);

            using (var w = new Watch<V1Pod>(c.Request<V1Pod>("ns", "name")) { OpenRetryTime = TimeSpan.FromSeconds(0.1) })
            {
                var events = new BlockingCollection<WatchEvent<V1Pod>>();
                V1Status error = null;
                int closeCount = 0, initialListCount = 0, openCount = 0, resetCount = 0;
                w.Closed += _ => closeCount++;
                w.Error += (_, ex, err) => error = err;
                w.EventReceived += (_, t, o) => events.Add(new WatchEvent<V1Pod>() { Type = t, Object = o });
                w.InitialList += _ => initialListCount++;
                w.Opened += _ => openCount++;
                w.Reset += _ => resetCount++;

                var cts = new CancellationTokenSource();
                Task runTask = w.Run(cts.Token);
                Assert.Null(w.LastVersion);
                Assert.True(pipeResetEvent.WaitOne(5000)); // wait for the initial connection
                Assert.Equal(0, initialListCount);

                // add some events and check that we receive them all
                var pod = c.New<V1Pod>("ns", "name");
                (pod.Metadata.ResourceVersion, pod.Metadata.Uid) = ("1", "id");
                writeEvent(WatchEventType.Added, pod);
                Assert.True(events.TryTake(out WatchEvent<V1Pod> we, 5000));
                Assert.Equal("ns", we.Object.Namespace());
                Assert.Equal("name", we.Object.Name());
                Assert.Equal("1", we.Object.ResourceVersion());
                Assert.Equal("id", we.Object.Uid());
                Assert.False(events.TryTake(out we, 100));

                pod.SetLabel("x", "y");
                pod.Metadata.ResourceVersion = "2";
                writeEvent(WatchEventType.Modified, pod);
                pod.Metadata.ResourceVersion = "3";
                writeEvent(WatchEventType.Bookmark, pod);
                pod.Metadata.ResourceVersion = "4";
                writeEvent(WatchEventType.Deleted, pod);

                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Modified, we.Type);
                Assert.Equal("2", we.Object.ResourceVersion());

                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Deleted, we.Type); // the bookmark should not be emitted
                Assert.Equal("4", we.Object.ResourceVersion());

                Assert.False(events.TryTake(out we, 100));
                Assert.Equal("4", w.LastVersion);
                Assert.Equal(0, closeCount);
                Assert.Equal(1, initialListCount);
                Assert.Equal(1, openCount);
                Assert.Equal(1, resetCount);

                // now issue a 410 Gone error and see that it's processed correctly
                writeEvent(WatchEventType.Error, new V1Status() { Status = "Failure", Code = 410 });
                Assert.True(pipeResetEvent.WaitOne(5000)); // wait for the next open call
                Assert.Null(w.LastVersion);
                writeEvent(WatchEventType.Added, pod);
                Assert.True(events.TryTake(out we, 1000));
                Assert.Equal(WatchEventType.Added, we.Type);
                Assert.Equal("4", we.Object.ResourceVersion());
                Assert.False(events.TryTake(out we, 100));
                Assert.Equal(1, closeCount);
                Assert.Equal(2, initialListCount);
                Assert.Equal(2, openCount);
                Assert.Equal(2, resetCount);
                Assert.Equal("4", w.LastVersion);
                Assert.Null(error); // 410 Gone errors are "normal" errors and aren't reported

                // now issue a 500 Internal Server Error and see that it does get reported
                writeEvent(WatchEventType.Error, new V1Status() { Status = "Failure", Code = 500 });
                writeEvent(WatchEventType.Modified, pod);
                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Modified, we.Type);
                Assert.Equal("4", we.Object.ResourceVersion());
                Assert.False(events.TryTake(out we, 100));
                Assert.Equal(1, closeCount); // it should not cause a reconnection
                Assert.Equal(2, initialListCount);
                Assert.Equal(2, openCount);
                Assert.Equal(2, resetCount);
                Assert.Equal("4", w.LastVersion);
                Assert.NotNull(error);
                Assert.Equal(500, error.Code.Value);

                // now close the connection and ensure it gets reopened
                error = null;
                stream.Close();
                Assert.True(pipeResetEvent.WaitOne(5000)); // wait for the reconnection
                Assert.Equal("4", w.LastVersion);
                writeEvent(WatchEventType.Modified, pod);
                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Modified, we.Type);
                Assert.Equal("4", we.Object.ResourceVersion());
                Assert.False(events.TryTake(out we, 100));
                Assert.Equal(2, closeCount);
                Assert.Equal(2, initialListCount);
                Assert.Equal(3, openCount);
                Assert.Equal(2, resetCount); // the version should not be reset
                Assert.Null(error);

                // test graceful shutdown
                cts.Cancel();
                await runTask;
            }
        }

        [Fact]
        public async Task TestListWatch()
        {
            Pipe pipe = null;
            void writeEvent(WatchEventType t, object o) =>
                pipe.Write(Encoding.UTF8.GetBytes(
                    new JObject(new JProperty("type", t.ToString()), new JProperty("object", JObject.FromObject(o))).ToString()));

            var pipeResetEvent = new AutoResetEvent(false);
            PipeStream stream = null;
            var pods = new V1PodList() { Items = new List<V1Pod>(), Metadata = new V1ListMeta() };
            var handler = new MockHttpHandler(req =>
            {
                if (req.RequestUri.Query.Contains("watch=1")) // if it's a watch request...
                {
                    pipe = new Pipe();
                    pipeResetEvent.Set();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream = new PipeStream(pipe, true)) };
                }
                else // otherwise, it's a request for the initial list
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(pods)) };
                }
            });

            var c = new Kubernetes(new KubernetesClientConfiguration() { Host = "http://localhost" }, new HttpClient(handler));

            var pod1 = c.New<V1Pod>("ns", "name1");
            (pod1.Metadata.ResourceVersion, pod1.Metadata.Uid) = ("10", "id1");
            var pod2 = c.New<V1Pod>("ns", "name2");
            (pod2.Metadata.ResourceVersion, pod2.Metadata.Uid) = ("1", "id2");
            pods.Items.Add(pod1);
            pods.Items.Add(pod2);
            pods.Metadata.ResourceVersion = "10";

            using (var w = new Watch<V1Pod>(c.Request<V1Pod>("ns")) { OpenRetryTime = TimeSpan.FromSeconds(0.1) })
            {
                var events = new BlockingCollection<WatchEvent<V1Pod>>();
                V1Status error = null;
                int closeCount = 0, openCount = 0, initialListCount = 0, resetCount = 0;
                w.Closed += _ => closeCount++;
                w.Error += (_, ex, err) => error = err;
                w.EventReceived += (_, t, o) => events.Add(new WatchEvent<V1Pod>() { Type = t, Object = o });
                w.InitialList += _ => initialListCount++;
                w.Opened += _ => openCount++;
                w.Reset += _ => resetCount++;

                var cts = new CancellationTokenSource();
                Task runTask = w.Run(cts.Token);
                Assert.True(pipeResetEvent.WaitOne(5000)); // wait for the initial connection

                // check that we received the initial events from the list
                Assert.True(events.TryTake(out WatchEvent<V1Pod> we, 5000));
                Assert.Equal("ns", we.Object.Namespace());
                Assert.Equal("name1", we.Object.Name());
                Assert.Equal("10", we.Object.ResourceVersion());
                Assert.Equal("id1", we.Object.Uid());
                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal("ns", we.Object.Namespace());
                Assert.Equal("name2", we.Object.Name());
                Assert.Equal("1", we.Object.ResourceVersion());
                Assert.Equal("id2", we.Object.Uid());
                Assert.Equal("10", w.LastVersion);
                Assert.False(events.TryTake(out we, 100));

                pod2.SetLabel("x", "y");
                pod2.Metadata.ResourceVersion = "11";
                writeEvent(WatchEventType.Modified, pod2);
                pod2.Metadata.ResourceVersion = "12";
                writeEvent(WatchEventType.Bookmark, pod2);
                pod2.Metadata.ResourceVersion = "13";
                writeEvent(WatchEventType.Deleted, pod2);

                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Modified, we.Type);
                Assert.Equal("11", we.Object.ResourceVersion());

                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Deleted, we.Type); // the bookmark should not be emitted
                Assert.Equal("13", we.Object.ResourceVersion());

                Assert.False(events.TryTake(out we, 100));
                Assert.Equal("13", w.LastVersion);
                Assert.Equal(0, closeCount);
                Assert.Equal(1, initialListCount);
                Assert.Equal(1, openCount);
                Assert.Equal(1, resetCount);

                // now issue a 410 Gone error and see that it's processed correctly
                pods.Metadata.ResourceVersion = "13";
                writeEvent(WatchEventType.Error, new V1Status() { Status = "Failure", Code = 410 });
                Assert.True(pipeResetEvent.WaitOne(5000)); // wait for the next open call
                Assert.True(events.TryTake(out we, 1000)); // we should get events from the initial list
                Assert.Equal(WatchEventType.Added, we.Type);
                Assert.Equal("10", we.Object.ResourceVersion());
                Assert.True(events.TryTake(out we, 1000));
                Assert.Equal(WatchEventType.Added, we.Type);
                Assert.Equal("13", we.Object.ResourceVersion());
                Assert.False(events.TryTake(out we, 100));
                Assert.Equal(1, closeCount);
                Assert.Equal(2, initialListCount);
                Assert.Equal(2, openCount);
                Assert.Equal(2, resetCount);
                Assert.Equal("13", w.LastVersion);
                Assert.Null(error); // 410 Gone errors are "normal" errors and aren't reported

                // now issue a 500 Internal Server Error and see that it does get reported
                writeEvent(WatchEventType.Error, new V1Status() { Status = "Failure", Code = 500 });
                pod1.Metadata.ResourceVersion = "14";
                writeEvent(WatchEventType.Modified, pod1);
                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Modified, we.Type); // we should not get events from the initial list
                Assert.Equal("14", we.Object.ResourceVersion());
                Assert.False(events.TryTake(out we, 100));
                Assert.Equal(1, closeCount); // it should not cause a reconnection
                Assert.Equal(2, initialListCount);
                Assert.Equal(2, openCount);
                Assert.Equal(2, resetCount);
                Assert.Equal("14", w.LastVersion);
                Assert.NotNull(error);
                Assert.Equal(500, error.Code.Value);

                // now close the connection and ensure it gets reopened
                error = null;
                stream.Close();
                Assert.True(pipeResetEvent.WaitOne(5000)); // wait for the reconnection
                Assert.Equal("14", w.LastVersion);
                pod1.Metadata.ResourceVersion = "15";
                writeEvent(WatchEventType.Modified, pod1);
                Assert.True(events.TryTake(out we, 5000));
                Assert.Equal(WatchEventType.Modified, we.Type);
                Assert.Equal("15", we.Object.ResourceVersion());
                Assert.Equal("15", w.LastVersion);
                Assert.False(events.TryTake(out we, 100)); // we should not get events from the initial list
                Assert.Equal(2, closeCount);
                Assert.Equal(2, initialListCount);
                Assert.Equal(3, openCount);
                Assert.Equal(2, resetCount); // the version should not be reset
                Assert.Null(error);

                // test graceful shutdown
                cts.Cancel();
                await runTask;
            }
        }
    }
}
