using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;

namespace k8s
{
    // There may be already an async implementation that we can use:
    // https://github.com/StephenCleary/AsyncEx/wiki/AsyncProducerConsumerQueue
    // However, they focus on individual objects and may not be a good choice for use with fixed-with byte buffers

    /// <summary>
    /// Represents a bounded buffer. A dedicated thread can send bytes to this buffer (the producer); while another thread can
    /// read bytes from this buffer (the consumer).
    /// </summary>
    /// <remarks>
    /// This is a producer-consumer problem (or bounded-buffer problem), see https://en.wikipedia.org/wiki/Producer%E2%80%93consumer_problem
    /// </remarks>
    public class ByteBuffer : IDisposable
    {
        private const int DefaultBufferSize = 4 * 1024; // 4 KB
        private const int DefaultMaximumSize = 40 * 1024 * 1024; // 40 MB

        private readonly int maximumSize;
        private readonly AutoResetEvent dataAvailable = new AutoResetEvent(initialState: false);
        private readonly object lockObject = new object();

        private byte[] buffer;
        private int bytesWritten = 0;
        private int bytesRead = 0;

        /// <summary>
        /// Used by a writer to indicate the end of the file. When set, the reader will be notified that no
        /// more data is available.
        /// </summary>
        private bool endOfFile;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteBuffer"/> class using the default buffer size and limit.
        /// </summary>
        public ByteBuffer()
            : this(DefaultBufferSize, DefaultMaximumSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteBuffer"/> class.
        /// </summary>
        /// <param name="bufferSize">
        /// The initial buffer size.
        /// </param>
        /// <param name="maximumSize">
        /// The maximum buffer size.
        /// </param>
        public ByteBuffer(int bufferSize, int maximumSize)
        {
            this.maximumSize = maximumSize;
            this.buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            this.endOfFile = false;
        }

        /// <summary>
        /// Gets the current buffer size.
        /// </summary>
        public int Size
        {
            get { return this.buffer.Length; }
        }

        /// <summary>
        /// Gets the maximum allowed size of the buffer.
        /// </summary>
        public int MaximumSize
        {
            get { return this.maximumSize; }
        }

        /// <summary>
        /// Gets the offset from which the next byte will be read. Increased every time a caller reads data.
        /// </summary>
        public int ReadWaterMark { get; private set; }

        /// <summary>
        /// Gets the offset to which the next byte will be written. Increased every time a caller writes data.
        /// </summary>
        public int WriteWaterMark { get; private set; }

        /// <summary>
        /// Gets the amount of bytes availble for reading.
        /// </summary>
        public int AvailableReadableBytes
        {
            get
            {
                lock (this.lockObject)
                {
                    if (this.ReadWaterMark == this.WriteWaterMark)
                    {
                        return 0;
                    }
                    else if (this.ReadWaterMark < this.WriteWaterMark)
                    {
                        return this.WriteWaterMark - this.ReadWaterMark;
                    }
                    else
                    {
                        return

                            // Bytes available at the end of the array
                            this.buffer.Length - this.ReadWaterMark

                            // Bytes available at the start of the array
                            + this.WriteWaterMark;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the amount of bytes available for writing.
        /// </summary>
        public int AvailableWritableBytes
        {
            get
            {
                lock (this.lockObject)
                {
                    if (this.WriteWaterMark > this.ReadWaterMark)
                    {
                        return
                            /* Available bytes at the end of the buffer */
                            this.buffer.Length - this.WriteWaterMark
                            /* Available bytes at the start of the buffer */
                            + this.ReadWaterMark;
                    }
                    else if (this.WriteWaterMark == this.ReadWaterMark)
                    {
                        return this.buffer.Length;
                    }
                    else
                    {
                        return this.ReadWaterMark - this.WriteWaterMark;
                    }
                }
            }
        }

        /// <summary>
        /// Writes bytes to the buffer.
        /// </summary>
        /// <param name="data">
        /// The source byte array from which to read the bytes.
        /// </param>
        /// <param name="offset">
        /// The offset of the first byte to copy.
        /// </param>
        /// <param name="length">
        /// The amount of bytes to copy.
        /// </param>
        public void Write(byte[] data, int offset, int length)
        {
            lock (this.lockObject)
            {
                // Does the data fit?
                // We must make sure that ReadWaterMark != WriteWaterMark; that would indicate 'all data has been read' instead
                // instead of 'all data must be read'
                if (this.AvailableWritableBytes <= length)
                {
                    // Grow the buffer
                    this.Grow(this.buffer.Length + length - this.AvailableWritableBytes + 1);
                }

                // Write the data; first the data that fits between the write watermark and the end of the buffer
                int availableBeforeWrapping = this.buffer.Length - this.WriteWaterMark;

                Array.Copy(data, offset, this.buffer, this.WriteWaterMark, Math.Min(availableBeforeWrapping, length));
                this.WriteWaterMark += Math.Min(availableBeforeWrapping, length);

                if (length > availableBeforeWrapping)
                {
                    Array.Copy(data, offset + availableBeforeWrapping, this.buffer, 0,
                        length - availableBeforeWrapping);
                    this.WriteWaterMark = length - availableBeforeWrapping;
                }

                this.bytesWritten += length;
                Debug.Assert(this.bytesRead + this.AvailableReadableBytes == this.bytesWritten);
            }

            this.dataAvailable.Set();
        }

        /// <summary>
        /// Stops writing data to the buffer, indicating that the end of file has been reached.
        /// </summary>
        public void WriteEnd()
        {
            lock (this.lockObject)
            {
                this.endOfFile = true;
                this.dataAvailable.Set();
            }
        }

        /// <summary>
        /// Reads bytes from the buffer.
        /// </summary>
        /// <param name="data">
        /// The byte array into which to read the data.
        /// </param>
        /// <param name="offset">
        /// The offset at which to start writing the bytes.
        /// </param>
        /// <param name="count">
        /// The amount of bytes to read.
        /// </param>
        /// <returns>
        /// The total number of bytes read.
        /// </returns>
        public int Read(byte[] data, int offset, int count)
        {
            while (this.AvailableReadableBytes == 0 && !this.endOfFile)
            {
                this.dataAvailable.WaitOne();
            }

            int toRead = 0;

            lock (this.lockObject)
            {
                // Signal the end of file to the caller.
                if (this.AvailableReadableBytes == 0 && this.endOfFile)
                {
                    return 0;
                }

                toRead = Math.Min(this.AvailableReadableBytes, count);

                int availableBeforeWrapping = this.buffer.Length - this.ReadWaterMark;

                Array.Copy(this.buffer, this.ReadWaterMark, data, offset, Math.Min(availableBeforeWrapping, toRead));
                this.ReadWaterMark += Math.Min(availableBeforeWrapping, toRead);

                if (toRead > availableBeforeWrapping)
                {
                    Array.Copy(this.buffer, 0, data, offset + availableBeforeWrapping,
                        toRead - availableBeforeWrapping);
                    this.ReadWaterMark = toRead - availableBeforeWrapping;
                }

                this.bytesRead += toRead;
                Debug.Assert(this.bytesRead + this.AvailableReadableBytes == this.bytesWritten);
            }

            return toRead;
        }

        /// <summary>
        /// The event which is raised when the buffer is resized.
        /// </summary>
        public event EventHandler OnResize;

        /// <summary>
        /// Increases the buffer size. Any call to this method must be protected with a lock.
        /// </summary>
        /// <param name="size">
        /// The new buffer size.
        /// </param>
        private void Grow(int size)
        {
            if (size > this.maximumSize)
            {
                throw new OutOfMemoryException();
            }

            var newBuffer = ArrayPool<byte>.Shared.Rent(size);

            if (this.WriteWaterMark < this.ReadWaterMark)
            {
                // Copy the data at the start
                Array.Copy(this.buffer, 0, newBuffer, 0, this.WriteWaterMark);

                int trailingDataLength = this.buffer.Length - this.ReadWaterMark;
                Array.Copy(this.buffer,
                    sourceIndex: this.ReadWaterMark,
                    destinationArray: newBuffer,
                    destinationIndex: newBuffer.Length - trailingDataLength,
                    length: trailingDataLength);

                this.ReadWaterMark += newBuffer.Length - this.buffer.Length;
            }
            else
            {
                // ... [Read WM] ... [Write WM] ... [newly available space]
                Array.Copy(this.buffer, 0, newBuffer, 0, this.buffer.Length);
            }

            ArrayPool<byte>.Shared.Return(this.buffer);
            this.buffer = newBuffer;

            Debug.Assert(this.bytesRead + this.AvailableReadableBytes == this.bytesWritten);
            this.OnResize?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ArrayPool<byte>.Shared.Return(this.buffer);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ByteBuffer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
