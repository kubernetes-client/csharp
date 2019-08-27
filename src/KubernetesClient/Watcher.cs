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
        private readonly IAsyncLineReader _lineReader;
        private readonly Task _watcherLoop;

        /// <summary>
        /// Initializes a new instance of the <see cref="Watcher{T}"/> class.
        /// </summary>
        /// <param name="lineReader">
        /// A <see cref="ILineReader"/> from which to read the events.
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
        /// <param name="cancellationToken">
        /// A token that can be used to cancel the read
        /// </param>
        public Watcher(IAsyncLineReader lineReader, Action<WatchEventType, T> onEvent, Action<Exception> onError,
            Action onClosed = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _lineReader = lineReader;
            OnEvent += onEvent;
            OnError += onError;
            OnClosed += onClosed;

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _watcherLoop = this.WatcherLoop(_cts.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _cts.Cancel();
            _lineReader.Dispose();
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
                while ((line = await this._lineReader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
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
        /// <param name="cancellationToken">A token that can be used to cancel the watch</param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T>(this HttpOperationResponse response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
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
        /// <param name="cancellationToken">A token that can be used to cancel the read</param>
        /// <returns>a watch object</returns>
        public static Watcher<T> Watch<T>(this HttpOperationResponse<T> response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Watch((HttpOperationResponse)response, onEvent, onError, onClosed, cancellationToken);
        }

        /// <summary>
        /// watch the call to api server with watch=true asynchronously
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <param name="response">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">A token that can be used to cancel the watch</param>
        /// <returns>a watch object</returns>
        public static async Task WatchAsync<T>(this HttpOperationResponse response,
            CancellationToken cancellationToken,
            Func<CancellationToken, WatchEventType, T, Task> onEvent,
            Func<CancellationToken, Exception, Task> onError = null,
            Func<CancellationToken, Task> onClosed = null)
        {
            if (!(response.Response.Content is WatcherDelegatingHandler.LineSeparatedHttpContent content))
            {
                throw new KubernetesClientException("not a watchable request or failed response");
            }

            try
            {
                string line;
                // ReadLineAsync will return null when we've reached the end of the stream.
                while ((line = await content.StreamReader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
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
                            if (onError != null)
                            {
                                var exception = new KubernetesException(statusEvent.Object);
                                await onError(cancellationToken, exception).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            var @event = SafeJsonConvert.DeserializeObject<k8s.Watcher<T>.WatchEvent>(line);
                            if (onEvent != null)
                            {
                                await onEvent(cancellationToken, @event.Type, @event.Object).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // error if deserialized failed or onevent throws
                        if (onError != null)
                        {
                            await onError(cancellationToken, e).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // error when transport error, IOException ect
                if (onError != null)
                {
                    await onError(cancellationToken, e).ConfigureAwait(false);
                }
            }
            finally
            {
                if (onClosed != null)
                {
                    await onClosed(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// watch the call to api server with watch=true asynchronously
        /// </summary>
        /// <typeparam name="T">type of the event object</typeparam>
        /// <param name="response">the api response</param>
        /// <param name="onEvent">a callback when any event raised from api server</param>
        /// <param name="onError">a callbak when any exception was caught during watching</param>
        /// <param name="onClosed">
        /// The action to invoke when the server closes the connection.
        /// </param>
        /// <param name="cancellationToken">A token that can be used to cancel the watch</param>
        /// <returns>a watch object</returns>
        public static Task WatchAsync<T>(this HttpOperationResponse<T> response,
            CancellationToken cancellationToken,
            Func<CancellationToken, WatchEventType, T, Task> onEvent,
            Func<CancellationToken, Exception, Task> onError = null,
            Func<CancellationToken, Task> onClosed = null)
        {
            return WatchAsync((HttpOperationResponse)response, cancellationToken, onEvent, onError, onClosed);
        }
    }
}
