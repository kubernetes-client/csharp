using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using k8s.Enums;
using k8s.Models;
using Microsoft.Rest.Serialization;

namespace k8s
{
    public class Watcher<T> : IDisposable
    {
        /// <summary>
        /// indicate if the watch object is alive
        /// </summary>
        public bool Watching { get; private set; }

        private readonly CancellationTokenSource _cts;
        private readonly Func<Task<TextReader>> _streamReaderCreator;

        private TextReader _streamReader;
        private bool disposedValue;
        private readonly Task _watcherLoop;

        /// <summary>
        /// Initializes a new instance of the <see cref="Watcher{T}"/> class.
        /// </summary>
        /// <param name="streamReaderCreator">
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
        public Watcher(Func<Task<StreamReader>> streamReaderCreator, Action<WatchEventType, T> onEvent,
            Action<Exception> onError, Action onClosed = null)
            : this(
                async () => (TextReader)await streamReaderCreator().ConfigureAwait(false),
                onEvent, onError, onClosed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Watcher{T}"/> class.
        /// </summary>
        /// <param name="streamReaderCreator">
        /// A <see cref="TextReader"/> from which to read the events.
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
        public Watcher(Func<Task<TextReader>> streamReaderCreator, Action<WatchEventType, T> onEvent,
            Action<Exception> onError, Action onClosed = null)
        {
            _streamReaderCreator = streamReaderCreator;
            OnEvent += onEvent;
            OnError += onError;
            OnClosed += onClosed;

            _cts = new CancellationTokenSource();
            _watcherLoop = Task.Run(async () => await WatcherLoop(_cts.Token).ConfigureAwait(false));
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
            try
            {
                Watching = true;
                string line;
                _streamReader = await _streamReaderCreator().ConfigureAwait(false);

                // ReadLineAsync will return null when we've reached the end of the stream.
                while ((line = await _streamReader.ReadLineAsync().ConfigureAwait(false)) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        var genericEvent =
                            SafeJsonConvert.DeserializeObject<Watcher<KubernetesObject>.WatchEvent>(line);

                        if (genericEvent.Object.Kind == "Status")
                        {
                            var statusEvent = SafeJsonConvert.DeserializeObject<Watcher<V1Status>.WatchEvent>(line);
                            var exception = new KubernetesException(statusEvent.Object);
                            OnError?.Invoke(exception);
                        }
                        else
                        {
                            var @event = SafeJsonConvert.DeserializeObject<WatchEvent>(line);
                            OnEvent?.Invoke(@event.Type, @event.Object);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    _streamReader?.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Watcher()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
