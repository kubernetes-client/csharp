using System;
using System.Collections.Generic;

namespace k8s.WorkQueue.RateLimiter
{
    public class MaxOfRateLimiter<T> : IRateLimiter<T>
    {
        private readonly ICollection<IRateLimiter<T>> _rateLimiters;
        public MaxOfRateLimiter(params IRateLimiter<T>[] rateLimiters) {
            _rateLimiters = rateLimiters;
        }

        public TimeSpan When(T item)
        {
            var max = TimeSpan.Zero;
            foreach (var limiter in _rateLimiters) {
                var current = limiter.When(item);
                if (current.CompareTo(max) > 0) 
                {
                    max = current;
                }
            }

            return max;
        }

        public void Forget(T item)
        {
            foreach (var rateLimiter in _rateLimiters)
            {
                rateLimiter.Forget(item);
            }
        }

        public int NumRequeues(T item)
        {
            int max = 0;
            foreach (var limiter in _rateLimiters) 
            {
                int current = limiter.NumRequeues(item);
                if (current > max) {
                    max = current;
                }
            }

            return max;        }
    }
}