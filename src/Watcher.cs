using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using k8s.Exceptions;
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
        public bool Watching { get; private set; }

        private readonly CancellationTokenSource _cts;
        private readonly StreamReader _streamReader;

        public Watcher(StreamReader streamReader, Action<WatchEventType, T> onEvent, Action<Exception> onError)
        {
            _streamReader = streamReader;
            OnEvent += onEvent;
            OnError += onError;

            _cts = new CancellationTokenSource();

            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    Watching = true;

                    while (!streamReader.EndOfStream)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        var line = await streamReader.ReadLineAsync();

                        try
                        {
                            var @event = SafeJsonConvert.DeserializeObject<WatchEvent>(line);
                            OnEvent?.Invoke(@event.Type, @event.Object);
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
                }
            }, token);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _streamReader.Dispose();
        }

        public event Action<WatchEventType, T> OnEvent;
        public event Action<Exception> OnError;

        public class WatchEvent
        {
            public WatchEventType Type { get; set; }

            public T Object { get; set; }
        }
    }

    public static class WatcherExt
    {
        public static Watcher<T> Watch<T>(this HttpOperationResponse response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null)
        {
            if (!(response.Response.Content is WatcherDelegatingHandler.LineSeparatedHttpContent content))
            {
                throw new KubernetesClientException("not a watchable request or failed response");
            }

            return new Watcher<T>(content.StreamReader, onEvent, onError);
        }

        public static Watcher<T> Watch<T>(this HttpOperationResponse<T> response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null)
        {
            return Watch((HttpOperationResponse) response, onEvent, onError);
        }
    }
}