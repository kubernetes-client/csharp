using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using k8s.Tests.Mock;
using Xunit;

namespace k8s.Tests
{
    public class StreamDemuxerTests
    {
        [Fact]
        public async Task SendDataRemoteCommand()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws))
            {
                var sentBuffer = new List<byte>();
                ws.MessageSent += (sender, args) => { sentBuffer.AddRange(args.Data.Buffer); };

                demuxer.Start();

                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);
                var b = GenerateRandomBuffer(100, 0xEF);
                stream.Write(b, 0, b.Length);

                // Send 100 bytes, expect 1 (channel index) + 100 (payload) = 101 bytes
                Assert.True(
                    await WaitForAsync(() => sentBuffer.Count == 101).ConfigureAwait(false),
                    $"Demuxer error: expect to send 101 bytes, but actually send {sentBuffer.Count} bytes.");
                Assert.True(sentBuffer[0] == channelIndex, "The first sent byte is not channel index!");
                Assert.True(sentBuffer[1] == 0xEF, "Incorrect payload!");
            }
        }

        [Fact]
        public async Task SendMultipleDataRemoteCommand()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws))
            {
                var sentBuffer = new List<byte>();
                ws.MessageSent += (sender, args) => { sentBuffer.AddRange(args.Data.Buffer); };

                demuxer.Start();

                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);
                var b = GenerateRandomBuffer(100, 0xEF);
                stream.Write(b, 0, b.Length);
                b = GenerateRandomBuffer(200, 0xAB);
                stream.Write(b, 0, b.Length);

                // Send 300 bytes in 2 messages, expect 1 (channel index) * 2 + 300 (payload) = 302 bytes
                Assert.True(
                    await WaitForAsync(() => sentBuffer.Count == 302).ConfigureAwait(false),
                    $"Demuxer error: expect to send 302 bytes, but actually send {sentBuffer.Count} bytes.");
                Assert.True(sentBuffer[0] == channelIndex, "The first sent byte is not channel index!");
                Assert.True(sentBuffer[1] == 0xEF, "The first part of payload incorrect!");
                Assert.True(sentBuffer[101] == channelIndex, "The second message first byte is not channel index!");
                Assert.True(sentBuffer[102] == 0xAB, "The second part of payload incorrect!");
            }
        }

        [Fact]
        public async Task ReceiveDataRemoteCommand()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws))
            {
                demuxer.Start();

                var receivedBuffer = new List<byte>();
                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);

                // Receive 600 bytes in 3 messages. Exclude 1 channel index byte per message, expect 597 bytes payload.
                var expectedCount = 597;

                var t = Task.Run(async () =>
                {
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex, 0xAA, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex, 0xAB, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex, 0xAC, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);

                    await WaitForAsync(() => receivedBuffer.Count == expectedCount).ConfigureAwait(false);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None).ConfigureAwait(false);
                });
                var buffer = new byte[50];
                while (true)
                {
                    var cRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    if (cRead == 0)
                    {
                        break;
                    }

                    for (var i = 0; i < cRead; i++)
                    {
                        receivedBuffer.Add(buffer[i]);
                    }
                }

                await t.ConfigureAwait(false);

                Assert.True(
                    receivedBuffer.Count == expectedCount,
                    $"Demuxer error: expect to receive {expectedCount} bytes, but actually got {receivedBuffer.Count} bytes.");
                Assert.True(receivedBuffer[0] == 0xAA, "The first payload incorrect!");
                Assert.True(receivedBuffer[98] == 0xAA, "The first payload incorrect!");
                Assert.True(receivedBuffer[99] == 0xAB, "The second payload incorrect!");
                Assert.True(receivedBuffer[297] == 0xAB, "The second payload incorrect!");
                Assert.True(receivedBuffer[298] == 0xAC, "The third payload incorrect!");
                Assert.True(receivedBuffer[596] == 0xAC, "The third payload incorrect!");
            }
        }

        [Fact]
        public async Task ReceiveDataPortForward()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws, StreamType.PortForward))
            {
                demuxer.Start();

                var receivedBuffer = new List<byte>();
                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);

                // Receive 600 bytes in 3 messages. Exclude 1 channel index byte per message, and 2 port bytes in the first message.
                // expect 600 - 3 - 2 = 595 bytes payload.
                var expectedCount = 595;

                var t = Task.Run(async () =>
                {
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex, 0xB1, true)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex, 0xB2, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex, 0xB3, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);

                    await WaitForAsync(() => receivedBuffer.Count == expectedCount).ConfigureAwait(false);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None).ConfigureAwait(false);
                });
                var buffer = new byte[50];
                while (true)
                {
                    var cRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    if (cRead == 0)
                    {
                        break;
                    }

                    for (var i = 0; i < cRead; i++)
                    {
                        receivedBuffer.Add(buffer[i]);
                    }
                }

                await t.ConfigureAwait(false);

                Assert.True(
                    receivedBuffer.Count == expectedCount,
                    $"Demuxer error: expect to receive {expectedCount} bytes, but actually got {receivedBuffer.Count} bytes.");
                Assert.True(receivedBuffer[0] == 0xB1, "The first payload incorrect!");
                Assert.True(receivedBuffer[96] == 0xB1, "The first payload incorrect!");
                Assert.True(receivedBuffer[97] == 0xB2, "The second payload incorrect!");
                Assert.True(receivedBuffer[295] == 0xB2, "The second payload incorrect!");
                Assert.True(receivedBuffer[296] == 0xB3, "The third payload incorrect!");
                Assert.True(receivedBuffer[594] == 0xB3, "The third payload incorrect!");
            }
        }

        [Fact]
        public async Task ReceiveDataPortForwardOneByteMessage()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws, StreamType.PortForward))
            {
                demuxer.Start();

                var receivedBuffer = new List<byte>();
                byte channelIndex = 12;
                var stream = demuxer.GetStream(channelIndex, channelIndex);

                // Receive 402 bytes in 3 buffers of 2 messages. Exclude 1 channel index byte per message, and 2 port bytes in the first message.
                // expect 402 - 1 x 2 - 2 = 398 bytes payload.
                var expectedCount = 398;

                var t = Task.Run(async () =>
                {
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(2, channelIndex, 0xC1, true)),
                        WebSocketMessageType.Binary, false).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex, 0xC2, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex, 0xC3, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);

                    await WaitForAsync(() => receivedBuffer.Count == expectedCount).ConfigureAwait(false);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None).ConfigureAwait(false);
                });
                var buffer = new byte[50];
                while (true)
                {
                    var cRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    if (cRead == 0)
                    {
                        break;
                    }

                    for (var i = 0; i < cRead; i++)
                    {
                        receivedBuffer.Add(buffer[i]);
                    }
                }

                await t.ConfigureAwait(false);

                Assert.True(
                    receivedBuffer.Count == expectedCount,
                    $"Demuxer error: expect to receive {expectedCount} bytes, but actually got {receivedBuffer.Count} bytes.");
                Assert.True(receivedBuffer[0] == 0xC2, "The first payload incorrect!");
                Assert.True(receivedBuffer[98] == 0xC2, "The first payload incorrect!");
                Assert.True(receivedBuffer[99] == 0xC3, "The second payload incorrect!");
                Assert.True(receivedBuffer[397] == 0xC3, "The second payload incorrect!");
            }
        }

        [Fact]
        public async Task ReceiveDataRemoteCommandMultipleStream()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws))
            {
                demuxer.Start();

                var receivedBuffer1 = new List<byte>();
                byte channelIndex1 = 1;
                var stream1 = demuxer.GetStream(channelIndex1, channelIndex1);
                var receivedBuffer2 = new List<byte>();
                byte channelIndex2 = 2;
                var stream2 = demuxer.GetStream(channelIndex2, channelIndex2);

                // stream 1: receive 100 + 300 = 400 bytes, exclude 1 channel index per message, expect 400 - 1 x 2 = 398 bytes.
                var expectedCount1 = 398;

                // stream 2: receive 200 bytes, exclude 1 channel index per message, expect 200 - 1 = 199 bytes.
                var expectedCount2 = 199;

                var t1 = Task.Run(async () =>
                {
                    // Simulate WebSocket received remote data to multiple streams
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex1, 0xD1, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex2, 0xD2, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex1, 0xD3, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);

                    await WaitForAsync(() => receivedBuffer1.Count == expectedCount1).ConfigureAwait(false);
                    await WaitForAsync(() => receivedBuffer2.Count == expectedCount2).ConfigureAwait(false);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None).ConfigureAwait(false);
                });
                var t2 = Task.Run(async () =>
                {
                    var buffer = new byte[50];
                    while (true)
                    {
                        var cRead = await stream1.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        if (cRead == 0)
                        {
                            break;
                        }

                        for (var i = 0; i < cRead; i++)
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
                        var cRead = await stream2.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        if (cRead == 0)
                        {
                            break;
                        }

                        for (var i = 0; i < cRead; i++)
                        {
                            receivedBuffer2.Add(buffer[i]);
                        }
                    }
                });
                await Task.WhenAll(t1, t2, t3).ConfigureAwait(false);

                Assert.True(
                    receivedBuffer1.Count == expectedCount1,
                    $"Demuxer error: expect to receive {expectedCount1} bytes, but actually got {receivedBuffer1.Count} bytes.");
                Assert.True(
                    receivedBuffer2.Count == expectedCount2,
                    $"Demuxer error: expect to receive {expectedCount2} bytes, but actually got {receivedBuffer2.Count} bytes.");
                Assert.True(receivedBuffer1[0] == 0xD1, "The first payload incorrect!");
                Assert.True(receivedBuffer1[98] == 0xD1, "The first payload incorrect!");
                Assert.True(receivedBuffer1[99] == 0xD3, "The second payload incorrect!");
                Assert.True(receivedBuffer1[397] == 0xD3, "The second payload incorrect!");
                Assert.True(receivedBuffer2[0] == 0xD2, "The first payload incorrect!");
                Assert.True(receivedBuffer2[198] == 0xD2, "The first payload incorrect!");
            }
        }

        [Fact]
        public async Task ReceiveDataPortForwardMultipleStream()
        {
            using (var ws = new MockWebSocket())
            using (var demuxer = new StreamDemuxer(ws, StreamType.PortForward))
            {
                demuxer.Start();

                var receivedBuffer1 = new List<byte>();
                byte channelIndex1 = 1;
                var stream1 = demuxer.GetStream(channelIndex1, channelIndex1);
                var receivedBuffer2 = new List<byte>();
                byte channelIndex2 = 2;
                var stream2 = demuxer.GetStream(channelIndex2, channelIndex2);

                // stream 1: receive 100 + 300 = 400 bytes, exclude 1 channel index per message, exclude port bytes in the first message,
                // expect 400 - 1 x 2 - 2 = 396 bytes.
                var expectedCount1 = 396;

                // stream 2: receive 200 bytes, exclude 1 channel index per message, exclude port bytes in the first message,
                // expect 200 - 1 - 2 = 197 bytes.
                var expectedCount2 = 197;

                var t1 = Task.Run(async () =>
                {
                    // Simulate WebSocket received remote data to multiple streams
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(100, channelIndex1, 0xE1, true)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(200, channelIndex2, 0xE2, true)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);
                    await ws.InvokeReceiveAsync(
                        new ArraySegment<byte>(GenerateRandomBuffer(300, channelIndex1, 0xE3, false)),
                        WebSocketMessageType.Binary, true).ConfigureAwait(false);

                    await WaitForAsync(() => receivedBuffer1.Count == expectedCount1).ConfigureAwait(false);
                    await WaitForAsync(() => receivedBuffer2.Count == expectedCount2).ConfigureAwait(false);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "normal", CancellationToken.None).ConfigureAwait(false);
                });
                var t2 = Task.Run(async () =>
                {
                    var buffer = new byte[50];
                    while (true)
                    {
                        var cRead = await stream1.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        if (cRead == 0)
                        {
                            break;
                        }

                        for (var i = 0; i < cRead; i++)
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
                        var cRead = await stream2.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                        if (cRead == 0)
                        {
                            break;
                        }

                        for (var i = 0; i < cRead; i++)
                        {
                            receivedBuffer2.Add(buffer[i]);
                        }
                    }
                });
                await Task.WhenAll(t1, t2, t3).ConfigureAwait(false);

                Assert.True(
                    receivedBuffer1.Count == expectedCount1,
                    $"Demuxer error: expect to receive {expectedCount1} bytes, but actually got {receivedBuffer1.Count} bytes.");
                Assert.True(
                    receivedBuffer2.Count == expectedCount2,
                    $"Demuxer error: expect to receive {expectedCount2} bytes, but actually got {receivedBuffer2.Count} bytes.");
                Assert.True(receivedBuffer1[0] == 0xE1, "The first payload incorrect!");
                Assert.True(receivedBuffer1[96] == 0xE1, "The first payload incorrect!");
                Assert.True(receivedBuffer1[97] == 0xE3, "The second payload incorrect!");
                Assert.True(receivedBuffer1[395] == 0xE3, "The second payload incorrect!");
                Assert.True(receivedBuffer2[0] == 0xE2, "The first payload incorrect!");
                Assert.True(receivedBuffer2[196] == 0xE2, "The first payload incorrect!");
            }
        }

        private static byte[] GenerateRandomBuffer(int length, byte channelIndex, byte content, bool portForward)
        {
            var buffer = GenerateRandomBuffer(length, content);
            buffer[0] = channelIndex;
            if (portForward)
            {
                if (length > 1)
                {
                    buffer[1] = 0xFF; // the first port bytes
                }

                if (length > 2)
                {
                    buffer[2] = 0xFF; // the 2nd port bytes
                }
            }

            return buffer;
        }

        private static byte[] GenerateRandomBuffer(int length, byte content)
        {
            var buffer = new byte[length];
            for (var i = 0; i < length; i++)
            {
                buffer[i] = content;
            }

            return buffer;
        }

        private async Task<bool> WaitForAsync(Func<bool> handler, float waitForSeconds = 1)
        {
            var w = Stopwatch.StartNew();
            try
            {
                do
                {
                    if (handler())
                    {
                        return true;
                    }

                    await Task.Delay(10).ConfigureAwait(false);
                }
                while (w.Elapsed.Duration().TotalSeconds < waitForSeconds);

                return false;
            }
            finally
            {
                w.Stop();
            }
        }
    }
}
