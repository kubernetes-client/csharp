using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using k8s.Exceptions;
using k8s.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;

namespace k8s
{
    public enum WatchEventType
    {
        [EnumMember(Value = "ADDED")] Added,

        [EnumMember(Value = "MODIFIED")] Modified,

        [EnumMember(Value = "DELETED")] Deleted,

        [EnumMember(Value = "ERROR")] Error
    }

    public class Watcher<T> : IDisposable
    {
        /// <summary>
        /// indicate if the watch object is alive
        /// </summary>
        public bool Watching { get; private set; }

        private readonly CancellationTokenSource _cts;
        private readonly StreamReader _streamReader;
        private readonly Task _watcherLoop;

        /// <summary>
        /// Initializes a new instance of the <see cref="Watcher{T}"/> class.
        /// </summary>
        /// <param name="streamReader">
        /// A <see cref="StreamReader"/> from which to read the events.
        /// </param>
        /// <param name="onEvent">
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name="onError">
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        public Watcher(StreamReader streamReader, Action<WatchEventType, T> onEvent, Action<Exception> onError, Action onClosed = null)
        {
            _streamReader = streamReader;
            OnEvent += onEvent;
            OnError += onError;
            OnClosed += onClosed;

            _cts = new CancellationTokenSource();
            _watcherLoop = this.WatcherLoop(_cts.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _cts.Cancel();
            _streamReader.Dispose();
        }

        /// <summary>
        /// add/remove callbacks when any event raised from api server
        /// </summary>
        public event Action<WatchEventType, T> OnEvent;

        /// <summary>
        /// add/remove callbacks when any exception was caught during watching
        /// </summary>
        public event Action<Exception> OnError;

        /// <summary>
        /// The event which is raised when the server closes th econnection.
        /// </summary>
        public event Action OnClosed;

        public class WatchEvent
        {
            public WatchEventType Type { get; set; }

            public T Object { get; set; }
        }

        private async Task WatcherLoop(CancellationToken cancellationToken)
        {
            // Make sure we run async
            await Task.Yield();

            try
            {
                Watching = true;
                string line;

                // ReadLineAsync will return null when we've reached the end of the stream.
                while ((line = await this._streamReader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        var genericEvent = SafeJsonConvert.DeserializeObject<k8s.Watcher<KubernetesObject>.WatchEvent>(line);

                        if (genericEvent.Object.Kind == "Status")
                        {
                            var statusEvent = SafeJsonConvert.DeserializeObject<k8s.Watcher<V1Status>.WatchEvent>(line);
                            var exception = new KubernetesException(statusEvent.Object);
                            this.OnError?.Invoke(exception);
                        }
                        else
                        {
                            var @event = SafeJsonConvert.DeserializeObject<k8s.Watcher<T>.WatchEvent>(line);
                            this.OnEvent?.Invoke(@event.Type, @event.Object);
                        }
                    }
                    catch (Exception e)
                    {
                        // error if deserialized failed or onevent throws
                        OnError?.Invoke(e);
                    }
                }
            }
            catch (Exception e)
            {
                // error when transport error, IOException ect
                OnError?.Invoke(e);
            }
            finally
            {
                Watching = false;
                OnClosed?.Invoke();
            }
        }
    }

    public static class WatcherExt
    {
        /// <summary>
        /// create a watch object from a call to api server with watch=true
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <param name="response">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T>(this HttpOperationResponse response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            if (!(response.Response.Content is WatcherDelegatingHandler.LineSeparatedHttpContent content))
            {
                throw new KubernetesClientException("not a watchable request or failed response");
            }

            return new Watcher<T>(content.StreamReader, onEvent, onError, onClosed);
        }

        /// <summary>
        /// create a watch object from a call to api server with watch=true
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <param name="response">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T>(this HttpOperationResponse<T> response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return Watch((HttpOperationResponse)response, onEvent, onError, onClosed);
        }
    }
}
