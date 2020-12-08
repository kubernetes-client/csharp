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
        private readonly AutoResetEvent dataAvailable = new AutoResetEvent(false);
        private readonly object lockObject = new object();

        private byte[] buffer;
        private int bytesWritten;
        private int bytesRead;

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
            buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            endOfFile = false;
        }

        /// <summary>
        /// Gets the current buffer size.
        /// </summary>
        public int Size
        {
            get { return buffer.Length; }
        }

        /// <summary>
        /// Gets the maximum allowed size of the buffer.
        /// </summary>
        public int MaximumSize
        {
            get { return maximumSize; }
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
                lock (lockObject)
                {
                    if (ReadWaterMark == WriteWaterMark)
                    {
                        return 0;
                    }
                    else if (ReadWaterMark < WriteWaterMark)
                    {
                        return WriteWaterMark - ReadWaterMark;
                    }
                    else
                    {
                        return

                            // Bytes available at the end of the array
                            buffer.Length - ReadWaterMark

                            // Bytes available at the start of the array
                            + WriteWaterMark;
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
                lock (lockObject)
                {
                    if (WriteWaterMark > ReadWaterMark)
                    {
                        return
                            /* Available bytes at the end of the buffer */
                            buffer.Length - WriteWaterMark
                            /* Available bytes at the start of the buffer */
                            + ReadWaterMark;
                    }
                    else if (WriteWaterMark == ReadWaterMark)
                    {
                        return buffer.Length;
                    }
                    else
                    {
                        return ReadWaterMark - WriteWaterMark;
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
            lock (lockObject)
            {
                // Does the data fit?
                // We must make sure that ReadWaterMark != WriteWaterMark; that would indicate 'all data has been read' instead
                // instead of 'all data must be read'
                if (AvailableWritableBytes <= length)
                {
                    // Grow the buffer
                    Grow(buffer.Length + length - AvailableWritableBytes + 1);
                }

                // Write the data; first the data that fits between the write watermark and the end of the buffer
                var availableBeforeWrapping = buffer.Length - WriteWaterMark;

                Array.Copy(data, offset, buffer, WriteWaterMark, Math.Min(availableBeforeWrapping, length));
                WriteWaterMark += Math.Min(availableBeforeWrapping, length);

                if (length > availableBeforeWrapping)
                {
                    Array.Copy(data, offset + availableBeforeWrapping, buffer, 0,
                        length - availableBeforeWrapping);
                    WriteWaterMark = length - availableBeforeWrapping;
                }

                bytesWritten += length;
                Debug.Assert(bytesRead + AvailableReadableBytes == bytesWritten);
            }

            dataAvailable.Set();
        }

        /// <summary>
        /// Stops writing data to the buffer, indicating that the end of file has been reached.
        /// </summary>
        public void WriteEnd()
        {
            lock (lockObject)
            {
                endOfFile = true;
                dataAvailable.Set();
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
            while (AvailableReadableBytes == 0 && !endOfFile)
            {
                dataAvailable.WaitOne();
            }

            var toRead = 0;

            lock (lockObject)
            {
                // Signal the end of file to the caller.
                if (AvailableReadableBytes == 0 && endOfFile)
                {
                    return 0;
                }

                toRead = Math.Min(AvailableReadableBytes, count);

                var availableBeforeWrapping = buffer.Length - ReadWaterMark;

                Array.Copy(buffer, ReadWaterMark, data, offset, Math.Min(availableBeforeWrapping, toRead));
                ReadWaterMark += Math.Min(availableBeforeWrapping, toRead);

                if (toRead > availableBeforeWrapping)
                {
                    Array.Copy(buffer, 0, data, offset + availableBeforeWrapping,
                        toRead - availableBeforeWrapping);
                    ReadWaterMark = toRead - availableBeforeWrapping;
                }

                bytesRead += toRead;
                Debug.Assert(bytesRead + AvailableReadableBytes == bytesWritten);
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
            if (size > maximumSize)
            {
                throw new OutOfMemoryException();
            }

            var newBuffer = ArrayPool<byte>.Shared.Rent(size);

            if (WriteWaterMark < ReadWaterMark)
            {
                // Copy the data at the start
                Array.Copy(buffer, 0, newBuffer, 0, WriteWaterMark);

                var trailingDataLength = buffer.Length - ReadWaterMark;
                Array.Copy(
                    buffer,
                    ReadWaterMark,
                    newBuffer,
                    newBuffer.Length - trailingDataLength,
                    trailingDataLength);

                ReadWaterMark += newBuffer.Length - buffer.Length;
            }
            else
            {
                // ... [Read WM] ... [Write WM] ... [newly available space]
                Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
            }

            ArrayPool<byte>.Shared.Return(buffer);
            buffer = newBuffer;

            Debug.Assert(bytesRead + AvailableReadableBytes == bytesWritten);
            OnResize?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    dataAvailable.Dispose();
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
