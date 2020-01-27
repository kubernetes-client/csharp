using System.Threading.Channels;

namespace k8s.WorkQueue
{
    public interface IRateLimitingChannel<T> : IChannel<T>
    {
        void AddRateLimited(T item);
        void Forget(T item);
        int NumRequests(T item);
    }

    public interface IChannel<T>
    {
        ChannelReader<T> Reader { get; }
        ChannelWriter<T> Writer { get; }
    }
}