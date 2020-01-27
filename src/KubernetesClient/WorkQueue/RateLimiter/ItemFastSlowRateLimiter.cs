using System;
using System.Collections.Concurrent;

namespace k8s.WorkQueue.RateLimiter
{
    public class ItemFastSlowRateLimiter<T> : IRateLimiter<T>
    {
        private readonly TimeSpan _fastDelay;
        private readonly TimeSpan _slowDelay;
        private readonly int _maxFastAttempts;
        readonly ConcurrentDictionary<T, int> _failures = new ConcurrentDictionary<T, int>();


        public ItemFastSlowRateLimiter(TimeSpan fastDelay, TimeSpan slowDelay, int maxFastAttempts) {
            _fastDelay = fastDelay;
            _slowDelay = slowDelay;
            _maxFastAttempts = maxFastAttempts;

        }
        public TimeSpan When(T item)
        {
            var attempts = _failures.AddOrUpdate(item, _ => 0, (_, existing) => existing + 1);
            if (attempts <= _maxFastAttempts) {
                return _fastDelay;
            }
            return _slowDelay;        }

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