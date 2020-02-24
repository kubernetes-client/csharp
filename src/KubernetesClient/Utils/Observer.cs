using System;

namespace k8s.Utils
{
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onComplete;
        
        public Observer(Action<T> onNext, Action<Exception> onError = null, Action onComplete = null)
        {
            _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            _onError = onError ?? OnErrorNoOp;
            _onComplete = onComplete ?? OnCompleteNoOp;
        }

        private static void OnCompleteNoOp()
        {
            
        }
        private static void OnErrorNoOp(Exception exception)
        {
        }

        public void OnCompleted() => _onComplete();

        public void OnError(Exception error) => _onError(error);

        public void OnNext(T value) => _onNext(value);
    }
}