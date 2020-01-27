using System;

namespace k8s.WorkQueue.RateLimiter
{
    public interface IRateLimiter<T>
    {
        TimeSpan When(T item);
        void Forget(T item);
        int NumRequeues(T item);
    }
}