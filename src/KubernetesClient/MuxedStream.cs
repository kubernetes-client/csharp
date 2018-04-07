using System;
using System.IO;

namespace k8s
{
    public class MuxedStream : Stream
    {
        private ByteBuffer inputBuffer;
        private byte? outputIndex;
        private StreamDemuxer muxer;

        public MuxedStream(StreamDemuxer muxer, ByteBuffer inputBuffer, byte? outputIndex)
        {
            this.inputBuffer = inputBuffer;
            this.outputIndex = outputIndex;

            if (this.inputBuffer == null && outputIndex == null)
            {
                throw new ArgumentException("You must specify at least inputBuffer or outputIndex");
            }

            this.muxer = muxer ?? throw new ArgumentNullException(nameof(muxer));
        }

        public override bool CanRead => this.inputBuffer != null;

        public override bool CanSeek => false;

        public override bool CanWrite => this.outputIndex != null;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.outputIndex == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                this.muxer.Write(this.outputIndex.Value, buffer, offset, count).GetAwaiter().GetResult();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.inputBuffer == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return this.inputBuffer.Read(buffer, offset, count);
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
