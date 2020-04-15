using Nito.AsyncEx;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.Tests.Mock
{
    public class MockWebSocket : WebSocket
    {
        private WebSocketCloseStatus? closeStatus = null;
        private string closeStatusDescription;
        private WebSocketState state;
        private string subProtocol;
        private ConcurrentQueue<MessageData> receiveBuffers = new ConcurrentQueue<MessageData>();
        private AsyncAutoResetEvent receiveEvent = new AsyncAutoResetEvent(false);

        public MockWebSocket(string subProtocol = null)
        {
            this.subProtocol = subProtocol;
        }

        public void SetState(WebSocketState state)
        {
            this.state = state;
        }

        public EventHandler<MessageDataEventArgs> MessageSent;

        public Task InvokeReceiveAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage)
        {
            this.receiveBuffers.Enqueue(new MessageData()
            {
                Buffer = buffer,
                MessageType = messageType,
                EndOfMessage = endOfMessage
            });
            this.receiveEvent.Set();
            return Task.CompletedTask;
        }

        #region WebSocket overrides
        public override WebSocketCloseStatus? CloseStatus => this.closeStatus;

        public override string CloseStatusDescription => this.closeStatusDescription;

        public override WebSocketState State => this.state;

        public override string SubProtocol => this.subProtocol;

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            this.closeStatus = closeStatus;
            this.closeStatusDescription = statusDescription;
            this.receiveBuffers.Enqueue(new MessageData()
            {
                Buffer = new ArraySegment<byte>(new byte[] { }),
                EndOfMessage = true,
                MessageType = WebSocketMessageType.Close
            });
            this.receiveEvent.Set();
            return Task.CompletedTask;
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            this.receiveBuffers.Clear();
            this.receiveEvent.Set();
        }

        public override async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            if (this.receiveBuffers.Count == 0)
            {
                await this.receiveEvent.WaitAsync(cancellationToken).ConfigureAwait(false);
            }

            int bytesReceived = 0;
            bool endOfMessage = true;
            WebSocketMessageType messageType = WebSocketMessageType.Close;

            MessageData received = null;
            if (this.receiveBuffers.TryPeek(out received))
            {
                messageType = received.MessageType;
                if (received.Buffer.Count <= buffer.Count)
                {
                    this.receiveBuffers.TryDequeue(out received);
                    received.Buffer.CopyTo(buffer);
                    bytesReceived = received.Buffer.Count;
                    endOfMessage = received.EndOfMessage;
                }
                else
                {
                    received.Buffer.Slice(0, buffer.Count).CopyTo(buffer);
                    bytesReceived = buffer.Count;
                    endOfMessage = false;
                    received.Buffer = received.Buffer.Slice(buffer.Count);
                }
            }

            return new WebSocketReceiveResult(bytesReceived, messageType, endOfMessage);
        }

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            this.MessageSent?.Invoke(this, new MessageDataEventArgs()
            {
                Data = new MessageData()
                {
                    Buffer = buffer,
                    MessageType = messageType,
                    EndOfMessage = endOfMessage
                }
            });
            return Task.CompletedTask;
        }
        #endregion

        public class MessageData
        {
            public ArraySegment<byte> Buffer { get; set; }
            public WebSocketMessageType MessageType { get; set; }
            public bool EndOfMessage { get; set; }
        }

        public class MessageDataEventArgs : EventArgs
        {
            public MessageData Data { get; set; }
        }
    }
}
