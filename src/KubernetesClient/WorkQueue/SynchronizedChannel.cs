using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace k8s.WorkQueue
{
    public class SynchronizedChannel<T>
    {
        public SynchronizedChannel()
        {
            Reader = new SynchronizedReader(this);
            Writer = new SynchronizedWriter(this);
        }

        public ChannelReader<T> Reader { get; }
        public ChannelWriter<T> Writer { get; } 
        private readonly Channel<T> _internalChannel = Channel.CreateUnbounded<T>();


        private readonly HashSet<T> _dirty = new HashSet<T>();
        private readonly HashSet<T> _processing = new HashSet<T>();
        private readonly object _syncLock = new object();

        public void Done(T item)
        {
            lock (_syncLock)
            {
                _processing.Remove(item);
                if (_dirty.Contains(item))
                    _internalChannel.Writer.WriteAsync(item);
            }
        }
        private class SynchronizedReader : ChannelReader<T>
        {
            private readonly SynchronizedChannel<T> _parent;

            public SynchronizedReader(SynchronizedChannel<T> parent)
            {
                _parent = parent;
            }

            public override bool TryRead(out T item)
            {
                lock (_parent._syncLock)
                {
                    var retVal = _parent._internalChannel.Reader.TryRead(out item);
                    _parent._processing.Add(item);
                    _parent._dirty.Remove(item);
                    return retVal;
                }
            }

            public override async ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = new CancellationToken())
                => await _parent.Reader.WaitToReadAsync(cancellationToken);
        }

        private class SynchronizedWriter : ChannelWriter<T>
        {
            private readonly SynchronizedChannel<T> _parent;

            public SynchronizedWriter(SynchronizedChannel<T> parent)
            {
                _parent = parent;
            }

            public override bool TryWrite(T item)
            {
                lock (_parent._syncLock)
                {

                    if (_parent.Reader.Completion.IsCompleted)
                        return false;
                    if (_parent._dirty.Contains(item))
                        return false;
                    _parent._dirty.Add(item);
                    if (_parent._processing.Contains(item))
                        return false;
                    return _parent._internalChannel.Writer.TryWrite(item);
                }
            }

            public override async ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = new CancellationToken())
                => await _parent._internalChannel.Writer.WaitToWriteAsync(cancellationToken);
        }
    }
}