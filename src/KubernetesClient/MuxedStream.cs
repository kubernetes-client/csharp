using System;
using System.IO;

namespace k8s
{
    /// <summary>
    /// A <see cref="Stream"/> which reads/writes from a specific channel using a <see cref="StreamDemuxer" />.
    /// </summary>
    public class MuxedStream : Stream
    {
        private ByteBuffer inputBuffer;
        private byte? outputIndex;
        private StreamDemuxer muxer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuxedStream"/> class.
        /// </summary>
        /// <param name="muxer">
        /// The <see cref="StreamDemuxer"/> to use to read from/write to the underlying stream.
        /// </param>
        /// <param name="inputBuffer">
        /// The <see cref="inputBuffer"/> to read from.
        /// </param>
        /// <param name="outputIndex">
        /// The index of the channel to which to write.
        /// </param>
        public MuxedStream(StreamDemuxer muxer, ByteBuffer inputBuffer, byte? outputIndex)
        {
            this.inputBuffer = inputBuffer;
            this.outputIndex = outputIndex;

            if (this.inputBuffer == null && outputIndex == null)
            {
                throw new ArgumentException("You must specify at least inputBuffer or outputIndex");
            }

            if (outputIndex != null)
            {
                this.muxer = muxer ?? throw new ArgumentNullException(nameof(muxer));
            }
        }

        /// <inheritdoc/>
        public override bool CanRead => inputBuffer != null;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => outputIndex != null;

        /// <inheritdoc/>
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (outputIndex == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                muxer.Write(outputIndex.Value, buffer, offset, count).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (inputBuffer == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return inputBuffer.Read(buffer, offset, count);
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            // Whenever we call muxer.Write, a message is immediately sent over the wire, so we don't need/support flushing.
            // Implement flushing as a no-op operation as opposed to throwing a NotSupportedException.
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
