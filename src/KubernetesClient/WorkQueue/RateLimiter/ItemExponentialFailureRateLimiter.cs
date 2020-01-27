using System;
using System.Collections.Concurrent;
using System.Threading;

namespace k8s.WorkQueue.RateLimiter
{
    public class ItemExponentialFailureRateLimiter<T> : IRateLimiter<T>
    {
        private TimeSpan _baseDelay;
        private TimeSpan _maxDelay;
        ConcurrentDictionary<T, int> _failures = new ConcurrentDictionary<T, int>();

        public ItemExponentialFailureRateLimiter(TimeSpan baseDelay, TimeSpan maxDelay)
        {
            _baseDelay = baseDelay;
            _maxDelay = maxDelay;
        }

        public TimeSpan When(T item)
        {
            var attempts = _failures.AddOrUpdate(item, _ => 0, (_, existing) => existing + 1);
            var d = (long)_maxDelay.TotalMilliseconds >> attempts;
            return d > _baseDelay.TotalMilliseconds ? Multiply(_baseDelay, 1 << attempts) : _maxDelay;
        }

        private TimeSpan Multiply(TimeSpan timeSpan, int multiplier) => TimeSpan.FromTicks(timeSpan.Ticks * multiplier);

        public void Forget(T item)
        {
            _failures.TryRemove(item, out _);
        }

        public int NumRequeues(T item)
        {
            _failures.TryGetValue(item, out var failures);
            return failures;
        }
    }
}