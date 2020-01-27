using System;

namespace k8s.WorkQueue.RateLimiter
{
    public class DefaultControllerRateLimiter<T> : IRateLimiter<T>
    {
        private IRateLimiter<T> _internalRateLimiter =
            new MaxOfRateLimiter<T>(
                new ItemExponentialFailureRateLimiter<T>(
                    TimeSpan.FromMilliseconds(5), TimeSpan.FromSeconds(1000)));

        public TimeSpan When(T item) => _internalRateLimiter.When(item);

        public void Forget(T item) => _internalRateLimiter.Forget(item);

        public int NumRequeues(T item) => _internalRateLimiter.NumRequeues(item);
    }
}