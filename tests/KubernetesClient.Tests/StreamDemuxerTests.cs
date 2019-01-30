using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using k8s.tests.Mock;
using Xunit;
using Xunit.Abstractions;

namespace k8s.tests
{
    public class StreamDemuxerTests
    {
        private readonly ITestOutputHelper testOutput;

        public StreamDemuxerTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public async Task SendDataRemoteCommand()
        {
            using (MockWebSocket ws = new MockWebSocket())
            {
                List<byte> sentBuffer = new List<byte>();
                ws.MessageSent += (sender, args) =>
                {
                    sentBuffer.AddRange(args.Data.Buffer);
                };

                StreamDemuxer demuxer = new StreamDemuxer(ws);
                Task.Run(() => demuxer.Start());

                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);
                var b = GenerateRandomBuffer(100, 10);
                stream.Write(b, 0, b.Length);

                // Send 100 bytes, expect 1 (channel index) + 100 (payload) = 101 bytes
                Assert.True(await WaitForAsync(() => sentBuffer.Count == 101), $"Demuxer error: expect to send 101 bytes, but actually send {sentBuffer.Count} bytes.");
                Assert.True(sentBuffer[0] == channelIndex, "The first sent byte is not channel index!");
            }
        }

        [Fact]
        public async Task SendMultipleDataRemoteCommand()
        {
            using (MockWebSocket ws = new MockWebSocket())
            {
                List<byte> sentBuffer = new List<byte>();
                ws.MessageSent += (sender, args) =>
                {
                    sentBuffer.AddRange(args.Data.Buffer);
                };

                StreamDemuxer demuxer = new StreamDemuxer(ws);
                Task.Run(() => demuxer.Start());

                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);
                var b = GenerateRandomBuffer(100, 10);
                stream.Write(b, 0, b.Length);
                b = GenerateRandomBuffer(200, 10);
                stream.Write(b, 0, b.Length);

                // Send 100 bytes, expect 1 (channel index) * 2 + 300 (payload) = 302 bytes
                Assert.True(await WaitForAsync(() => sentBuffer.Count == 302), $"Demuxer error: expect to send 302 bytes, but actually send {sentBuffer.Count} bytes.");
                Assert.True(sentBuffer[0] == channelIndex, "The first sent byte is not channel index!");
            }
        }

        [Fact]
        public async Task ReceiveDataRemoteCommand()
        {
            using (MockWebSocket ws = new MockWebSocket())
            {
                StreamDemuxer demuxer = new StreamDemuxer(ws);
                Task.Run(() => demuxer.Start());

                List<byte> receivedBuffer = new List<byte>();
                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);

                var t = Task.Run(async () =>
                {
                    // Simulate WebSocket received remote data
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex)), WebSocketMessageType.Binary, true);

                    await WaitForAsync(() => receivedBuffer.Count == 597);  // Receive 600 bytes in 3 messages, expect 597 bytes payload.
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None);
                });
                var buffer = new byte[50];
                while (true)
                {
                    var cRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (cRead == 0)
                    {
                        break;
                    }
                    for (int i = 0; i < cRead; i++)
                    {
                        receivedBuffer.Add(buffer[i]);
                    }
                }
                await t;

                // Receive 600 bytes in 3 messages, each with 1 channel index, expect payload 597 bytes
                Assert.True(receivedBuffer.Count == 597, $"Demuxer error: expect to receive 597 bytes, but actually got {receivedBuffer.Count} bytes.");
                Assert.True(receivedBuffer[0] == channelIndex, "The first sent byte is not channel index!");
            }
        }


        [Fact]
        public async Task ReceiveDataPortForward()
        {
            using (MockWebSocket ws = new MockWebSocket())
            {
                StreamDemuxer demuxer = new StreamDemuxer(ws, StreamType.PortForward);
                Task.Run(() => demuxer.Start());

                List<byte> receivedBuffer = new List<byte>();
                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);

                var t = Task.Run(async () =>
                {
                    // Simulate WebSocket received remote data
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex)), WebSocketMessageType.Binary, true);

                    await WaitForAsync(() => receivedBuffer.Count == 595);  // Receive 600 bytes in 3 messages, expect 595 bytes payload.
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None);
                });
                var buffer = new byte[50];
                while (true)
                {
                    var cRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (cRead == 0)
                    {
                        break;
                    }
                    for (int i = 0; i < cRead; i++)
                    {
                        receivedBuffer.Add(buffer[i]);
                    }
                }
                await t;

                // Receive 600 bytes in 3 messages, first one with 2 port bytes, and each with 1 channel index, expect payload 595 bytes
                Assert.True(receivedBuffer.Count == 595, $"Demuxer error: expect to receive 595 bytes, but actually got {receivedBuffer.Count} bytes.");
                Assert.True(receivedBuffer[0] == channelIndex, "The first sent byte is not channel index!");
            }
        }

        [Fact]
        public async Task ReceiveDataRemoteCommandMultipleStream()
        {
            using (MockWebSocket ws = new MockWebSocket())
            {
                StreamDemuxer demuxer = new StreamDemuxer(ws);
                Task.Run(() => demuxer.Start());

                List<byte> receivedBuffer1 = new List<byte>();
                byte channelIndex1 = 1;
                var stream1 = demuxer.GetStream(channelIndex1, channelIndex1);
                List<byte> receivedBuffer2 = new List<byte>();
                byte channelIndex2 = 2;
                var stream2 = demuxer.GetStream(channelIndex2, channelIndex2);

                var t1 = Task.Run(async () =>
                {
                    // Simulate WebSocket received remote data to multiple streams
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex1)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex2)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex1)), WebSocketMessageType.Binary, true);

                    await WaitForAsync(() => receivedBuffer1.Count == 398); // Receive 400 bytes in 2 messages, expect 398 bytes payload.
                    await WaitForAsync(() => receivedBuffer2.Count == 199); // Receive 200 bytes in 1 messages, expect 199 bytes payload.
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None);
                });
                var t2 = Task.Run(async () =>
                {
                    var buffer = new byte[50];
                    while (true)
                    {
                        var cRead = await stream1.ReadAsync(buffer, 0, buffer.Length);
                        if (cRead == 0)
                        {
                            break;
                        }
                        for (int i = 0; i < cRead; i++)
                        {
                            receivedBuffer1.Add(buffer[i]);
                        }
                    }
                });
                var t3 = Task.Run(async () =>
                {
                    var buffer = new byte[50];
                    while (true)
                    {
                        var cRead = await stream2.ReadAsync(buffer, 0, buffer.Length);
                        if (cRead == 0)
                        {
                            break;
                        }
                        for (int i = 0; i < cRead; i++)
                        {
                            receivedBuffer2.Add(buffer[i]);
                        }
                    }
                });
                await Task.WhenAll(t1, t2, t3);

                // Receive 600 bytes in 3 messages, each with 1 channel index, expect payload 597 bytes
                Assert.True(receivedBuffer1.Count == 398, $"Demuxer error: expect to receive 398 bytes, but actually got {receivedBuffer1.Count} bytes.");
                Assert.True(receivedBuffer2.Count == 199, $"Demuxer error: expect to receive 199 bytes, but actually got {receivedBuffer2.Count} bytes.");
            }
        }


        [Fact]
        public async Task ReceiveDataPortForwardMultipleStream()
        {
            using (MockWebSocket ws = new MockWebSocket())
            {
                StreamDemuxer demuxer = new StreamDemuxer(ws, StreamType.PortForward);
                Task.Run(() => demuxer.Start());

                List<byte> receivedBuffer1 = new List<byte>();
                byte channelIndex1 = 1;
                var stream1 = demuxer.GetStream(channelIndex1, channelIndex1);
                List<byte> receivedBuffer2 = new List<byte>();
                byte channelIndex2 = 2;
                var stream2 = demuxer.GetStream(channelIndex2, channelIndex2);

                var t1 = Task.Run(async () =>
                {
                    // Simulate WebSocket received remote data to multiple streams
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex1)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex2)), WebSocketMessageType.Binary, true);
                    await ws.InvokeReceiveAsync(new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex1)), WebSocketMessageType.Binary, true);

                    await WaitForAsync(() => receivedBuffer1.Count == 396); // Receive 400 bytes in 2 messages, expect 396 bytes payload.
                    await WaitForAsync(() => receivedBuffer2.Count == 197); // Receive 200 bytes in 1 messages, expect 197 bytes payload.
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None);
                });
                var t2 = Task.Run(async () =>
                {
                    var buffer = new byte[50];
                    while (true)
                    {
                        var cRead = await stream1.ReadAsync(buffer, 0, buffer.Length);
                        if (cRead == 0)
                        {
                            break;
                        }
                        for (int i = 0; i < cRead; i++)
                        {
                            receivedBuffer1.Add(buffer[i]);
                        }
                    }
                });
                var t3 = Task.Run(async () =>
                {
                    var buffer = new byte[50];
                    while (true)
                    {
                        var cRead = await stream2.ReadAsync(buffer, 0, buffer.Length);
                        if (cRead == 0)
                        {
                            break;
                        }
                        for (int i = 0; i < cRead; i++)
                        {
                            receivedBuffer2.Add(buffer[i]);
                        }
                    }
                });
                await Task.WhenAll(t1, t2, t3);

                // Receive 400 bytes in 2 messages, first one with 2 port bytes, each with 1 channel index, expect payload 396 bytes
                Assert.True(receivedBuffer1.Count == 396, $"Demuxer error: expect to receive 396 bytes, but actually got {receivedBuffer1.Count} bytes.");
                // Receive 200 bytes in 1 messages, first one with 2 port bytes, each with 1 channel index, expect payload 197 bytes
                Assert.True(receivedBuffer2.Count == 197, $"Demuxer error: expect to receive 197 bytes, but actually got {receivedBuffer2.Count} bytes.");
            }
        }


        private static byte[] GenerateRandomBuffer(int length, byte content)
        {
            var buffer = new byte[length];
            for(int i = 0; i < length; i++)
            {
                buffer[i] = content;
            }
            return buffer;
        }

        private async Task<bool> WaitForAsync(Func<bool> handler, float waitForSeconds = 1)
        {
            Stopwatch w = Stopwatch.StartNew();
            try
            {
                do
                {
                    if (handler())
                    {
                        return true;
                    }
                    await Task.Delay(10);
                } while (w.Elapsed.Duration().TotalSeconds < waitForSeconds);
                return false;
            }
            finally
            {
                w.Stop();
            }
        }
    }
}
