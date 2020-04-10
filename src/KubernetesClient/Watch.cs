using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using k8s.Models;
using Newtonsoft.Json;

namespace k8s
{
    #region Watch
    /// <summary>Watches one or more Kubernetes objects, reopening the watch and managing versions as necessary.</summary>
    public sealed class Watch<T> : IDisposable where T : IKubernetesObject, IMetadata<V1ObjectMeta>
    {
        /// <summary>Initializes a new watch from a <see cref="KubernetesRequest"/> that represents the item or list of items to watch.</summary>
        /// <param name="request">A <see cref="KubernetesRequest"/> that selects the item or list of items to watch</param>
        /// <param name="initialVersion">The resource version to resume from, or null to start from the current version</param>
        /// <param name="isListWatch">The value of the <see cref="IsListWatch"/> property, or null to use the default</param>
        /// <param name="exactRequest">If true, no modifications will be made to the <paramref name="request"/>. If false, common modifications
        /// will be made (although the original request will not be modified). See the remarks for details. The default is false.
        /// </param>
        /// <remarks>If <paramref name="exactRequest"/> is false, heuristics will be used to configure the request for watching as follows.
        /// If <see cref="IsListWatch"/> is true, the "allowWatchBookmarks" query-string parameter will be set to true. Otherwise, if the
        /// <see cref="KubernetesRequest.Name()"/> property is set but the <see cref="KubernetesRequest.Subresource()"/> property is not set,
        /// an attempt will be made to configure the query for watching a single item. If the query has no
        /// <see cref="KubernetesRequest.FieldSelector()"/> set, the name property will be replaced by a field selector that matches the name.
        /// Otherwise, the <see cref="KubernetesRequest.OldStyleWatch(bool)"/> property will be set to true to allow watching the named item.
        /// </remarks>
        public Watch(KubernetesRequest request, string initialVersion = null, bool? isListWatch = null, bool exactRequest = false)
        {
            this.req = request?.Clone() ?? throw new ArgumentNullException();
            IsListWatch = isListWatch ?? req.Name() == null;
            if (!exactRequest)
            {
                if (IsListWatch)
                {
                    req.SetQuery("allowWatchBookmarks", "true");
                }
                else if (req.Name() != null && req.Subresource() == null)
                {
                    if (req.FieldSelector() == null) req.OldStyleWatch(false).FieldSelector("metadata.name="+req.Name()).Name(null);
                    else req.OldStyleWatch(true);
                }
            }
            LastVersion = initialVersion;
        }

        /// <summary>Raised after the watch is closed.</summary>
        public event Action<Watch<T>> Closed;

        /// <summary>Raised when a non-bookmark watch event is received.</summary>
        public event Action<Watch<T>, WatchEventType, T> EventReceived;

        /// <summary>Raised when an error occurs, either an exception or a failure status.</summary>
        public event Action<Watch<T>, Exception, V1Status> Error;

        /// <summary>Raised after the initial set of item events has been sent after the watch is reset and opened. If the watch resumes
        /// from a known version, including when a resource version is passed to the constructor, it may not obtain an initial set of items
        /// and this event will not be raised.
        /// </summary>
        public event Action<Watch<T>> InitialListSent;

        /// <summary>Raised after the watch is opened.</summary>
        public event Action<Watch<T>> Opened;

        /// <summary>Raised after the watch is opened without a resource version or when events may have been lost.</summary>
        public event Action<Watch<T>> Reset;

        /// <summary>Gets or sets whether the watch should use list-watching semantics instead of item-watching semantics. This must
        /// be true when watching multiple items and should be false when watching a single item and must be false when the request does
        /// not return a list. By default, the property is initialized to true if the request <see cref="KubernetesRequest.Name()"/> is
        /// null, but if you're using a request with a null name that is nonetheless guaranteed to return only one item, you can improve
        /// efficiency by setting this to false.
        /// </summary>
        public bool IsListWatch { get; set; }

        /// <summary>Gets or sets the version of the most recently received resource, or null if no resource has been received or
        /// the version information is known to be out of date.
        /// </summary>
        public string LastVersion { get; private set; }

        /// <summary>Gets or sets how long the watch will wait between retries after an error. The default is 15 seconds.</summary>
        public TimeSpan OpenRetryTime { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>Stops the watch.</summary>
        public void Dispose() => cts?.Cancel();

        /// <summary>Runs the watch. To stop it, call <see cref="Dispose"/> or cancel the <see cref="CancellationToken"/>.</summary>
        /// <param name="shutdownToken">A <see cref="CancellationToken"/> that can be used to stop the watch</param>
        public Task Run(CancellationToken shutdownToken = default)
        {
            if (task != null) throw new InvalidOperationException();
            shutdownToken.ThrowIfCancellationRequested();
            cts = shutdownToken.CanBeCanceled ?
                CancellationTokenSource.CreateLinkedTokenSource(shutdownToken) : new CancellationTokenSource();
            task = Task.Run(Run);
            return task;
        }

        async Task Run()
        {
            while(!cts.IsCancellationRequested)
            {
                bool wasOpen = false; // used to determine whether we should raise OnClosed
                try
                {
                    // when watching multiple items when we don't know the starting version, we first have to retrieve the list and then
                    // watch for changes. if we just started watching, we'd get an initial mishmash of resource versions and wouldn't be
                    // able to know what the latest version is. when listing, the appropriate version is part of the list metadata
                    KubernetesList<T> list = null;
                    if (IsListWatch && LastVersion == null)
                    {
                        using (KubernetesResponse listResponse = await req.WatchVersion(null).ExecuteAsync(cts.Token).ConfigureAwait(false))
                        {
                            if (listResponse.IsError)
                            {
                                await ReportErrorAndWait(listResponse).ConfigureAwait(false);
                                continue;
                            }
                            list = await listResponse.GetBodyAsync<KubernetesList<T>>(failIfEmpty: true).ConfigureAwait(false);
                            LastVersion = list.Metadata?.ResourceVersion;
                        }
                    }

                    // now start the watch
                    using (KubernetesResponse response = await req.WatchVersion(LastVersion ?? "").ExecuteAsync(cts.Token).ConfigureAwait(false))
                    {
                        if (response.IsError) // if the watch request failed...
                        {
                            // if our resource version is too old, reset it. otherwise report the error
                            if (response.StatusCode == HttpStatusCode.Gone) LastVersion = null; // NOTE: in practice this seems to be reported as a watch event...
                            else await ReportErrorAndWait(response).ConfigureAwait(false);
                        }
                        else // if the response was successful, then the watch is open...
                        {
                            bool reset = LastVersion == null || list != null;
                            if (reset) Reset?.Invoke(this); // if we may have lost events, let the client know
                            Opened?.Invoke(this);
                            wasOpen = true; // since it was opened, we have to call OnClosed later
                            // if we started a new watch based on a list, send out the initial events from the list.
                            // otherwise, the initial event(s) will be sent from the watch
                            if (list != null)
                            {
                                if (EventReceived != null)
                                {
                                    // annoyingly, Kubernetes may exclude the API version and kind from items in lists. the list itself
                                    // only has the API version (of the list), and the kind is the kind of the list (e.g. PodList)
                                    string guessedApiVersion, guessedKind;
                                    if (!req.Scheme().TryGetVK(typeof(T), out guessedApiVersion, out guessedKind)) // so try to get them from the scheme
                                    {
                                        guessedApiVersion = list.ApiVersion; // if it's missing, assume the API version is the same as the list's
                                        // and for the kind, we'll use a heuristic that if the list kind is FooList then the item kind is Foo
                                        guessedKind = list.Kind != null && list.Kind.Length > 4 && list.Kind.EndsWith("List") ?
                                            list.Kind.Substring(0, list.Kind.Length-4) : null;
                                    }
                                    foreach (T item in list.Items)
                                    {
                                        if (item.ApiVersion == null) item.ApiVersion = guessedApiVersion;
                                        if (item.Kind == null) item.Kind = guessedKind;
                                        EventReceived(this, WatchEventType.Added, item);
                                    }
                                }
                                InitialListSent?.Invoke(this);
                            }
                            list = null; // the initial list may be large, so help out the garbage collector, maybe
                            using (var watchReader = new WatchReader<T>(response))
                            {
                                WatchEvent<T> e;
                                bool firstItem = true;
                                while((e = await watchReader.ReadAsync(cts.Token).ConfigureAwait(false)) != null)
                                {
                                    if (e.Type == WatchEventType.Error) // if an error occurred...
                                    {
                                        if (e.Error.Code.GetValueOrDefault() == (int)HttpStatusCode.Gone) // if our version is outdated...
                                        {
                                            LastVersion = null; // reset it
                                            break; // and restart the watch
                                        }
                                        else // otherwise, if the error was unknown...
                                        {
                                            Error?.Invoke(this, null, e.Error); // report and ignore it
                                        }
                                    }
                                    else // otherwise, no error occurred. update our version number and inform the client
                                    {
                                        LastVersion = e.Object.Metadata.ResourceVersion;
                                        if (e.Type != WatchEventType.Bookmark) // if it's a "real" event...
                                        {
                                            EventReceived?.Invoke(this, e.Type, e.Object); // let the user know
                                            // if we're not in list mode, then the first event completes the initial list
                                            if (firstItem && reset && !IsListWatch) InitialListSent?.Invoke(this);
                                            firstItem = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
                catch (EndOfStreamException) { } // treat a sudden stream closure the same way we treat closure in between events
                catch (Exception ex)
                {
                    Error?.Invoke(this, ex, null);
                    try { await Task.Delay(OpenRetryTime, cts.Token).ConfigureAwait(false); }
                    catch (OperationCanceledException) { }
                }
                finally
                {
                    if (wasOpen) Closed?.Invoke(this);
                }
            }
        }

        async Task ReportErrorAndWait(KubernetesResponse response)
        {
            Error?.Invoke(this, null, await response.GetStatusAsync().ConfigureAwait(false));
            await Task.Delay(OpenRetryTime, cts.Token).ConfigureAwait(false); // and wait before trying again
        }

        readonly KubernetesRequest req;
        CancellationTokenSource cts;
        Task task;
    }
    #endregion

    #region WatchEvent
    /// <summary>Represents an event from a watch stream.</summary>
    public sealed class WatchEvent<T>
    {
        /// <summary>Gets or sets the <see cref="WatchEventType"/>.</summary>
        public WatchEventType Type { get; set; }

        /// <summary>Gets or sets the error that occurred, if the <see cref="Type"/> is <see cref="WatchEventType.Error"/>.</summary>
        public V1Status Error { get; set; }

        /// <summary>Gets or sets object that the event applies to, if the <see cref="Type"/> is not <see cref="WatchEventType.Error"/>.</summary>
        public T Object { get; set; }
    }
    #endregion

    #region WatchReader
    /// <summary>Represents a stream of watch events.</summary>
    /// <typeparam name="T">The type of items to watch</typeparam>
    public sealed class WatchReader<T> : IDisposable
    {
        /// <summary>Initializes a new <see cref="WatchReader{T}"/> from a <see cref="KubernetesResponse"/> containing a watch stream.</summary>
        public WatchReader(KubernetesResponse response) : this((response ?? throw new ArgumentNullException(nameof(response))).Message) { }

        /// <summary>Initializes a new <see cref="WatchReader{T}"/> from a <see cref="HttpResponseMessage"/> containing a watch stream.</summary>
        public WatchReader(HttpResponseMessage response)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
            if (response.Content == null && !IsError) throw new ArgumentException("The response has no content.");
        }

        /// <summary>Initializes a new <see cref="WatchReader{T}"/> from a <see cref="Stream"/> containing a watch stream.</summary>
        public WatchReader(Stream stream) => CreateReader(stream ?? throw new ArgumentNullException(nameof(stream)));

        /// <summary>Indicates whether the request failed and so the <see cref="Response"/> does not contain a valid watch stream.</summary>
        public bool IsError => Response != null && (int)Response.StatusCode >= 400;

        /// <summary>Gets the underlying <see cref="HttpResponseMessage"/>, or null if the watch reader was initialized from a <see cref="Stream"/>.</summary>
        public HttpResponseMessage Response { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            streamReader?.Dispose();
            Response?.Dispose();
        }

        /// <summary>Reads the next <see cref="WatchEvent{T}"/> from the stream, or returns null if the stream was closed or
        /// <see cref="IsError"/> is true.
        /// </summary>
        public async Task<WatchEvent<T>> ReadAsync(CancellationToken cancelToken = default)
        {
            if (IsError) return null;
            cancelToken.ThrowIfCancellationRequested();
            if (reader == null) CreateReader(await Response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            try
            {
                streamReader.CancelToken = cancelToken;
#if !NET452
                if (!await reader.ReadAsync(cancelToken).ConfigureAwait(false)) return null; // wait for the next event
#else
                if (!reader.Read()) return null; // wait for the next event
#endif
            }
            catch (IOException ex) when (ex.InnerException is WebException wex && wex.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException("The watch read was canceled.", ex.InnerException, cancelToken);
            }

            // deserialize the watch event
            var e = new WatchEvent<T>();
            bool gotType = false, gotObject = false;
            while (true)
            {
#if !NET452
                if (!await reader.ReadAsync(cancelToken).ConfigureAwait(false)) throw EOFError(); // move to the next property, if any
#else
                if (!reader.Read()) throw EOFError(); // move to the next property, if any
#endif
                if (reader.TokenType != JsonToken.PropertyName) break;
                string name = (string)reader.Value;
#if !NET452
                if (!await reader.ReadAsync(cancelToken).ConfigureAwait(false)) throw EOFError(); // move to the property value
#else
                if (!reader.Read()) throw EOFError(); // move to the property value
#endif
                if (name == "type")
                {
                    e.Type = (WatchEventType)Enum.Parse(typeof(WatchEventType), (string)reader.Value, true);
                    gotType = true;
                }
                else if (name == "object")
                {
                    if (!gotType) throw new JsonSerializationException("Expected type property before object.");
                    if (e.Type != WatchEventType.Error) e.Object = serializer.Deserialize<T>(reader);
                    else e.Error = serializer.Deserialize<V1Status>(reader);
                    gotObject = true;
                }
                else
                {
#if !NET452
                    await reader.SkipAsync(cancelToken).ConfigureAwait(false); // move to the next property, if any
#else
                    reader.Skip(); // move to the next property, if any
#endif
                }
            }
            if (!gotObject) throw new JsonSerializationException("The stream does not appear to contain watch events.");
            return e;
        }

        #region CancelableStreamReader
        // HACK: annoyingly, the TextReader interface doesn't support cancellation of async reads outside .NET core. furthermore, Json.NET
        // doesn't support cancellation even in several versions of .NET core. so we have this hacky TextReader that attempts cancellation
        sealed class CancelableStreamReader : TextReader
        {
            public CancelableStreamReader(Stream stream) => this.stream = stream;

            // the CancellationToken used to cancel reads
            public CancellationToken CancelToken { get; set; }

            public override int Peek() => EnsureData() ? charBuffer[charRead] : -1;
            public override int Read() => EnsureData() ? charBuffer[charRead++] : -1;
            public override int Read(char[] buffer, int index, int count) => Read(buffer, index, count, false);

#if NETCOREAPP2_1
            public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken _) => ReadAsync(buffer, false);

            public override Task<int> ReadAsync(char[] buffer, int index, int count) =>
                ReadAsync(buffer.AsMemory(index, count), false).AsTask();

            public override ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken _) => ReadAsync(buffer, true);

            public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) =>
                ReadAsync(buffer.AsMemory(index, count), true).AsTask();
#else
            public override Task<int> ReadAsync(char[] buffer, int index, int count) => ReadAsync(buffer, index, count, false);
            public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) => ReadAsync(buffer, index, count, true);
#endif

            public override string ReadToEnd()
            {
                var sb = new System.Text.StringBuilder(4096);
                while(EnsureData())
                {
                    sb.Append(charBuffer, charRead, charWrite-charRead);
                    charRead = charWrite;
                }
                return sb.ToString();
            }

            public async override Task<string> ReadToEndAsync()
            {
                var sb = new System.Text.StringBuilder(4096);
                while(await EnsureDataAsync().ConfigureAwait(false))
                {
                    sb.Append(charBuffer, charRead, charWrite-charRead);
                    charRead = charWrite;
                }
                return sb.ToString();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                stream?.Dispose();
            }

            bool EnsureData()
            {
                if (charRead == charWrite)
                {
                    try { return EnsureDataAsync().GetAwaiter().GetResult(); }
                    catch (OperationCanceledException) { return false; } // non-async callers aren't expecting OperationCanceledException
                }
                return true;
            }

            async Task<bool> EnsureDataAsync()
            {
                if (charRead == charWrite)
                {
                    CancelToken.ThrowIfCancellationRequested();
                    charRead = charWrite = 0;
                    do
                    {
                        int bytesRead = await stream.ReadAsync(byteBuffer, 0, byteBuffer.Length, CancelToken).ConfigureAwait(false);
                        if (bytesRead == 0) return false;
                        charWrite = decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);
                    } while(charWrite == 0);
                }
                return true;
            }

            int Read(char[] buffer, int index, int count, bool block)
            {
                int start = index;
                while(count != 0 && EnsureData())
                {
                    int toRead = Math.Min(count, charWrite-charRead);
                    Array.Copy(charBuffer, charRead, buffer, index, toRead);
                    charRead += toRead;
                    index += toRead;
                    if (!block) break;
                    count -= toRead;
                }
                return index - start;
            }

#if NETCOREAPP2_1
            async ValueTask<int> ReadAsync(Memory<char> buffer, bool block)
            {
                int totalRead = 0;
                while(buffer.Length != 0 && (charRead != charWrite || await EnsureDataAsync().ConfigureAwait(false)))
                {
                    int toRead = Math.Min(buffer.Length, charWrite-charRead);
                    charBuffer.AsMemory(0, toRead).CopyTo(buffer);
                    totalRead += toRead;
                    charRead += toRead;
                    if (!block) break;
                    buffer = buffer.Slice(toRead);
                }
                return totalRead;
            }
#else
            async Task<int> ReadAsync(char[] buffer, int index, int length, bool block)
            {
                int start = index;
                while(length != 0 && (charRead != charWrite || await EnsureDataAsync().ConfigureAwait(false)))
                {
                    int toRead = Math.Min(length, charWrite-charRead);
                    Array.Copy(charBuffer, 0, buffer, index, toRead);
                    charRead += toRead;
                    index += toRead;
                    if (!block) break;
                    length -= toRead;
                }
                return index - start;
            }
#endif

            readonly Stream stream;
            readonly System.Text.Decoder decoder = System.Text.Encoding.UTF8.GetDecoder();
            readonly char[] charBuffer = new char[4096];
            readonly byte[] byteBuffer = new byte[4096];
            int charRead, charWrite;
        }
#endregion

        void CreateReader(Stream stream)
        {
            streamReader = new CancelableStreamReader(stream);
            reader = new JsonTextReader(streamReader);
            reader.SupportMultipleContent = true; // we'll be reading multiple objects out of the stream
            serializer = JsonSerializer.Create(Kubernetes.DefaultJsonSettings);
        }

        JsonTextReader reader;
        JsonSerializer serializer;
        CancelableStreamReader streamReader;

        static EndOfStreamException EOFError() => new EndOfStreamException("Unexpected end of the watch stream.");
    }
#endregion
}
