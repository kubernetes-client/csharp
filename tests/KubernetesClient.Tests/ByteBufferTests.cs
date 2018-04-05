using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    /// <summary>
    /// Tests the <see cref="ByteBuffer"/> class.
    /// </summary>
    public class ByteBufferTests
    {
        private readonly byte[] writeData = new byte[] { 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF };

        /// <summary>
        /// Tests a sequential read and write operation.
        /// </summary>
        [Fact]
        public void LinearReadWriteTest()
        {
            ByteBuffer buffer = new ByteBuffer(bufferSize: 0x10, maximumSize: 0x100);

            // There's no real guarantee that this will be the case because the ArrayPool does not guarantee
            // a specific buffer size. So let's assert this first to make sure the test fails should this
            // assumption not hold.
            Assert.Equal(0x10, buffer.Size);

            // Assert the initial values.
            Assert.Equal(0, buffer.AvailableReadableBytes);
            Assert.Equal(0x10, buffer.AvailableWritableBytes);
            Assert.Equal(0, buffer.ReadWaterMark);
            Assert.Equal(0, buffer.WriteWaterMark);

            // Write two bytes
            buffer.Write(this.writeData, 0, 2);

            Assert.Equal(2, buffer.AvailableReadableBytes);
            Assert.Equal(0x0E, buffer.AvailableWritableBytes);
            Assert.Equal(0, buffer.ReadWaterMark);
            Assert.Equal(2, buffer.WriteWaterMark);

            // Read two bytes, one byte at a time
            byte[] readData = new byte[0x10];

            var read = buffer.Read(readData, 0, 1);
            Assert.Equal(1, read);

            // Verify the result of the read operation.
            Assert.Equal(0xF0, readData[0]);
            Assert.Equal(0, readData[1]); // Make sure no additional data was read

            // Check the state of the buffer
            Assert.Equal(1, buffer.AvailableReadableBytes);
            Assert.Equal(0x0F, buffer.AvailableWritableBytes);
            Assert.Equal(1, buffer.ReadWaterMark);
            Assert.Equal(2, buffer.WriteWaterMark);

            // Read another byte
            read = buffer.Read(readData, 1, 1);
            Assert.Equal(1, read);

            // Verify the result of the read operation.
            Assert.Equal(0xF1, readData[1]);
            Assert.Equal(0, readData[2]); // Make sure no additional data was read

            // Check the state of the buffer
            Assert.Equal(0, buffer.AvailableReadableBytes);
            Assert.Equal(0x10, buffer.AvailableWritableBytes);
            Assert.Equal(2, buffer.ReadWaterMark);
            Assert.Equal(2, buffer.WriteWaterMark);
        }

        /// <summary>
        /// Tests reading a writing which crosses the boundary (end) of the circular buffer.
        /// </summary>
        [Fact]
        public void BoundaryReadWriteTest()
        {
            ByteBuffer buffer = new ByteBuffer(bufferSize: 0x10, maximumSize: 0x100);

            // There's no real guarantee that this will be the case because the ArrayPool does not guarantee
            // a specific buffer size. So let's assert this first to make sure the test fails should this
            // assumption not hold.
            Assert.Equal(0x10, buffer.Size);

            // Write out 0x0A bytes to the buffer, to increase the high water level for writing bytes
            buffer.Write(this.writeData, 0, 0x0A);

            // Assert the initial values.
            Assert.Equal(0x0A, buffer.AvailableReadableBytes);
            Assert.Equal(0x06, buffer.AvailableWritableBytes);
            Assert.Equal(0, buffer.ReadWaterMark);
            Assert.Equal(0x0A, buffer.WriteWaterMark);

            // Read 0x0A bytes, to increase the high water level for reading bytes
            byte[] readData = new byte[0x10];
            var read = buffer.Read(readData, 0, 0x0A);
            Assert.Equal(0x0A, read);

            Assert.Equal(0x00, buffer.AvailableReadableBytes);
            Assert.Equal(0x10, buffer.AvailableWritableBytes);
            Assert.Equal(0x0A, buffer.ReadWaterMark);
            Assert.Equal(0x0A, buffer.WriteWaterMark);

            // Write an additional 0x0A bytes, but now in reverse order. This will cause the data
            // to be wrapped.
            Array.Reverse(this.writeData);

            buffer.Write(this.writeData, 0, 0x0A);

            // Assert the resulting state of the buffer.
            Assert.Equal(0x0A, buffer.AvailableReadableBytes);
            Assert.Equal(0x06, buffer.AvailableWritableBytes);
            Assert.Equal(0x0A, buffer.ReadWaterMark);
            Assert.Equal(0x04, buffer.WriteWaterMark);

            // Read ten bytes, this will be a wrapped read
            read = buffer.Read(readData, 0, 0x0A);
            Assert.Equal(0x0A, read);

            // Verify the result of the read operation.
            Assert.Equal(0xFF, readData[0]);
            Assert.Equal(0xFE, readData[1]);
            Assert.Equal(0xFD, readData[2]);
            Assert.Equal(0xFC, readData[3]);
            Assert.Equal(0xFB, readData[4]);
            Assert.Equal(0xFA, readData[5]);
            Assert.Equal(0xF9, readData[6]);
            Assert.Equal(0xF8, readData[7]);
            Assert.Equal(0xF7, readData[8]);
            Assert.Equal(0xF6, readData[9]);
            Assert.Equal(0, readData[10]); // Make sure no additional data was read

            // Check the state of the buffer
            Assert.Equal(0, buffer.AvailableReadableBytes);
            Assert.Equal(0x10, buffer.AvailableWritableBytes);
            Assert.Equal(4, buffer.ReadWaterMark);
            Assert.Equal(4, buffer.WriteWaterMark);
        }

        /// <summary>
        /// Tests resizing of the <see cref="ByteBuffer"/> class.
        /// </summary>
        [Fact]
        public void ResizeWriteTest()
        {
            ByteBuffer buffer = new ByteBuffer(bufferSize: 0x10, maximumSize: 0x100);

            // There's no real guarantee that this will be the case because the ArrayPool does not guarantee
            // a specific buffer size. So let's assert this first to make sure the test fails should this
            // assumption not hold.
            Assert.Equal(0x10, buffer.Size);

            // Write out 0x0A bytes to the buffer, to increase the high water level for writing bytes
            buffer.Write(this.writeData, 0, 0x0A);

            byte[] readData = new byte[0x20];

            // Read these 0x0A bytes.
            var read = buffer.Read(readData, 0, 0x0A);
            Assert.Equal(0x0A, read);

            // Assert the initial state of the buffer
            Assert.Equal(0x00, buffer.AvailableReadableBytes);
            Assert.Equal(0x10, buffer.AvailableWritableBytes);
            Assert.Equal(0x0A, buffer.ReadWaterMark);
            Assert.Equal(0x0A, buffer.WriteWaterMark);

            // Write out 0x0A bytes to the buffer, this will cause the buffer to wrap
            buffer.Write(this.writeData, 0, 0x0A);

            Assert.Equal(0x0A, buffer.AvailableReadableBytes);
            Assert.Equal(0x06, buffer.AvailableWritableBytes);
            Assert.Equal(0x0A, buffer.ReadWaterMark);
            Assert.Equal(0x04, buffer.WriteWaterMark);

            // Write an additional 0x0A bytes, but now in reverse order. This will cause the buffer to be resized.
            Array.Reverse(this.writeData);

            buffer.Write(this.writeData, 0, 0x0A);

            // Make sure the buffer has been resized.
            Assert.Equal(0x20, buffer.Size);
            Assert.Equal(0x14, buffer.AvailableReadableBytes); // 2 * 0x0A = 0x14
            Assert.Equal(0x0C, buffer.AvailableWritableBytes); // 0x20 - 0x14 = 0x0C
            Assert.Equal(0x1A, buffer.ReadWaterMark);
            Assert.Equal(0x0E, buffer.WriteWaterMark);

            // Read data, and verify the read data
            read = buffer.Read(readData, 0, 0x14);
            Assert.Equal(0xF0, readData[0]);
            Assert.Equal(0xF1, readData[1]);
            Assert.Equal(0xF2, readData[2]);
            Assert.Equal(0xF3, readData[3]);
            Assert.Equal(0xF4, readData[4]);
            Assert.Equal(0xF5, readData[5]);
            Assert.Equal(0xF6, readData[6]);
            Assert.Equal(0xF7, readData[7]);
            Assert.Equal(0xF8, readData[8]);
            Assert.Equal(0xF9, readData[9]);

            Assert.Equal(0xFF, readData[10]);
            Assert.Equal(0xFE, readData[11]);
            Assert.Equal(0xFD, readData[12]);
            Assert.Equal(0xFC, readData[13]);
            Assert.Equal(0xFB, readData[14]);
            Assert.Equal(0xFA, readData[15]);
            Assert.Equal(0xF9, readData[16]);
            Assert.Equal(0xF8, readData[17]);
            Assert.Equal(0xF7, readData[18]);
            Assert.Equal(0xF6, readData[19]);
        }

        /// <summary>
        /// Tests a call to <see cref="ByteBuffer.Read(byte[], int, int)"/> which wants to read more data
        /// than is available.
        /// </summary>
        [Fact]
        public void ReadTooMuchDataTest()
        {
            var buffer = new ByteBuffer();

            var readData = new byte[0x10];

            // Read 0x010 bytes of data when only 0x06 are available
            buffer.Write(this.writeData, 0, 0x06);

            var read = buffer.Read(readData, 0, readData.Length);
            Assert.Equal(0x06, read);

            Assert.Equal(0xF0, readData[0]);
            Assert.Equal(0xF1, readData[1]);
            Assert.Equal(0xF2, readData[2]);
            Assert.Equal(0xF3, readData[3]);
            Assert.Equal(0xF4, readData[4]);
            Assert.Equal(0xF5, readData[5]);
            Assert.Equal(0x00, readData[6]);
        }

        /// <summary>
        /// Tests a call to <see cref="ByteBuffer.Read(byte[], int, int)"/> when no data is available; and makes
        /// sure the call blocks until data is available.
        /// </summary>
        [Fact]
        public async Task ReadBlocksUntilDataAvailableTest()
        {
            // Makes sure that the Read method does not return until data is available.
            var buffer = new ByteBuffer();
            var readData = new byte[0x10];
            var read = 0;

            // Kick off a read operation
            var readTask = Task.Run(() => read = buffer.Read(readData, 0, readData.Length));
            await Task.Delay(250);
            Assert.False(readTask.IsCompleted, "Read task completed before data was available.");

            // Write data to the buffer
            buffer.Write(this.writeData, 0, 0x03);

            await TaskAssert.Completed(readTask,
                timeout: TimeSpan.FromMilliseconds(1000),
                message: "Timed out waiting for read task to complete."
            );

            Assert.Equal(3, read);
            Assert.Equal(0xF0, readData[0]);
            Assert.Equal(0xF1, readData[1]);
            Assert.Equal(0xF2, readData[2]);
            Assert.Equal(0x00, readData[3]);
        }

        /// <summary>
        /// Tests reading until the end of the file.
        /// </summary>
        [Fact]
        public void ReadUntilEndOfFileTest()
        {
            ByteBuffer buffer = new ByteBuffer(bufferSize: 0x10, maximumSize: 0x100);

            // There's no real guarantee that this will be the case because the ArrayPool does not guarantee
            // a specific buffer size. So let's assert this first to make sure the test fails should this
            // assumption not hold.
            Assert.Equal(0x10, buffer.Size);

            buffer.Write(this.writeData, 0, 2);
            buffer.Write(this.writeData, 2, 2);
            buffer.WriteEnd();

            // Assert the initial state of the buffer
            Assert.Equal(0x04, buffer.AvailableReadableBytes);
            Assert.Equal(0x0C, buffer.AvailableWritableBytes);
            Assert.Equal(0x00, buffer.ReadWaterMark);
            Assert.Equal(0x04, buffer.WriteWaterMark);

            // Read the data on a chunk-by-chunk basis
            byte[] readData = new byte[0x03];
            var read = buffer.Read(readData, 0, 3);
            Assert.Equal(3, read);
            Assert.Equal(0xF0, readData[0]);
            Assert.Equal(0xF1, readData[1]);
            Assert.Equal(0xF2, readData[2]);

            read = buffer.Read(readData, 0, 3);
            Assert.Equal(1, read);
            Assert.Equal(0xF3, readData[0]);
        }

        /// <summary>
        /// Tests reading until the end of a file, piecemeal.
        /// </summary>
        [Fact]
        public void ReadUntilEndOfFileTest2()
        {
            ByteBuffer buffer = new ByteBuffer(bufferSize: 0x10, maximumSize: 0x100);

            // There's no real guarantee that this will be the case because the ArrayPool does not guarantee
            // a specific buffer size. So let's assert this first to make sure the test fails should this
            // assumption not hold.
            Assert.Equal(0x10, buffer.Size);

            buffer.Write(this.writeData, 0, 2);
            buffer.Write(this.writeData, 2, 2);
            buffer.WriteEnd();

            // Assert the initial state of the buffer
            Assert.Equal(0x04, buffer.AvailableReadableBytes);
            Assert.Equal(0x0C, buffer.AvailableWritableBytes);
            Assert.Equal(0x00, buffer.ReadWaterMark);
            Assert.Equal(0x04, buffer.WriteWaterMark);

            // Read the data at once
            byte[] readData = new byte[0x10];
            var read = buffer.Read(readData, 0, 0x10);
            Assert.Equal(4, read);
            Assert.Equal(0xF0, readData[0]);
            Assert.Equal(0xF1, readData[1]);
            Assert.Equal(0xF2, readData[2]);
            Assert.Equal(0xF3, readData[3]);
            Assert.Equal(0x00, readData[4]);

            read = buffer.Read(readData, 0, 0x10);
            Assert.Equal(0, read);
        }
    }
}
