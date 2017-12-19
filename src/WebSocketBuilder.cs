using System;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// The <see cref="WebSocketBuilder"/> creates a new <see cref="WebSocket"/> object which connects to a remote WebSocket.
    /// </summary>
    /// <remarks>
    /// By default, this uses the .NET <see cref="ClientWebSocket"/> class, but you can inherit from this class and change it to
    /// use any class which inherits from <see cref="WebSocket"/>, should you want to use a third party framework or mock the requests.
    /// </remarks>
    public class WebSocketBuilder
    {
        protected ClientWebSocket WebSocket { get; private set; } = new ClientWebSocket();

        public WebSocketBuilder()
        {
            this.WebSocket = new ClientWebSocket();
        }

        public virtual WebSocketBuilder SetRequestHeader(string headerName, string headerValue)
        {
            this.WebSocket.Options.SetRequestHeader(headerName, headerValue);
            return this;
        }

        public virtual WebSocketBuilder AddClientCertificate(X509Certificate certificate)
        {
            this.WebSocket.Options.ClientCertificates.Add(certificate);
            return this;
        }

        public virtual async Task<WebSocket> BuildAndConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await this.WebSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            return this.WebSocket;
        }
    }
}
