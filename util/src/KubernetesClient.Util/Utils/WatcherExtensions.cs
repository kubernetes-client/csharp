using System;

namespace k8s.Util.Utils
{
    public static class WatcherExtensions
    {
        public static IObservable<Tuple<WatchEventType, T>> AsObservable<T>(this Watcher<T> watcher)
        {
            return new WatchObservable<T>(watcher);
        }

        private class WatchObservable<T> : IObservable<Tuple<WatchEventType, T>>
        {
            private readonly Watcher<T> _watcher;

            public WatchObservable(Watcher<T> watcher)
            {
                _watcher = watcher;
            }

            private class Disposable : IDisposable
            {
                private readonly Action _dispose;

                public Disposable(Action dispose)
                {
                    _dispose = dispose;
                }

                public void Dispose()
                {
                    _dispose();
                }
            }

            public IDisposable Subscribe(IObserver<Tuple<WatchEventType, T>> observer)
            {
                void OnEvent(WatchEventType type, T obj) => observer.OnNext(Tuple.Create(type, obj));

                _watcher.OnEvent += OnEvent;
                _watcher.OnError += observer.OnError;
                _watcher.OnClosed += observer.OnCompleted;

                var subscriptionLifeline = new Disposable(() =>
                {
                    _watcher.OnEvent -= OnEvent;
                    _watcher.OnError -= observer.OnError;
                    _watcher.OnClosed -= observer.OnCompleted;
                });
                return subscriptionLifeline;
            }
        }
    }
}
