using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

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
        }

        public ClientWebSocketOptions Options => WebSocket.Options;

        public virtual WebSocketBuilder SetRequestHeader(string headerName, string headerValue)
        {
            WebSocket.Options.SetRequestHeader(headerName, headerValue);
            return this;
        }

        public virtual WebSocketBuilder AddClientCertificate(X509Certificate2 certificate)
        {
            WebSocket.Options.ClientCertificates.Add(certificate);
            return this;
        }

        public WebSocketBuilder ExpectServerCertificate(X509Certificate2Collection serverCertificate)
        {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            Options.RemoteCertificateValidationCallback
 = (sender, certificate, chain, sslPolicyErrors) =>
            {
                return Kubernetes.CertificateValidationCallBack(sender, serverCertificate, certificate, chain, sslPolicyErrors);
            };
#endif
            return this;
        }

        public WebSocketBuilder SkipServerCertificateValidation()
        {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            Options.RemoteCertificateValidationCallback
 = (sender, certificate, chain, sslPolicyErrors) => true;

#endif
            return this;
        }

        public virtual async Task<WebSocket> BuildAndConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await WebSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            return WebSocket;
        }
    }
}
