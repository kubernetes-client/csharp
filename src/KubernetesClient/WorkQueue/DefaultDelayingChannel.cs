using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace k8s.WorkQueue
{
    public  static class ChannelWriterExtensions
    {
        public static void AddAfter<T>(this ChannelWriter<T> channelWriter, T item, TimeSpan delay)
        {
            Task.Delay(delay).ContinueWith(x => channelWriter.TryWrite(item));
        }
    }
}