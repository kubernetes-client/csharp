using k8s.WorkQueue.RateLimiter;

namespace k8s.WorkQueue
{
    public class DefaultRateLimitingChannel<T> : SynchronizedChannel<T>, IRateLimitingChannel<T>
    {
        private readonly IRateLimiter<T> _rateLimiter;

        public DefaultRateLimitingChannel() : this(new DefaultControllerRateLimiter<T>())
        {
        }

        public DefaultRateLimitingChannel(IRateLimiter<T> rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        public void AddRateLimited(T item) => Writer.AddAfter(item, _rateLimiter.When(item));

        public void Forget(T item) => _rateLimiter.Forget(item);

        public int NumRequests(T item) => _rateLimiter.NumRequeues(item);
    }
}