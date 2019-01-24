using System;
using System.Net.WebSockets;
#if NET452
using System.Net.Security;
#endif
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
        }

        public ClientWebSocketOptions Options => WebSocket.Options;

        public virtual WebSocketBuilder SetRequestHeader(string headerName, string headerValue)
        {
            this.WebSocket.Options.SetRequestHeader(headerName, headerValue);
            return this;
        }

        public virtual WebSocketBuilder AddClientCertificate(X509Certificate2 certificate)
        {
            this.WebSocket.Options.ClientCertificates.Add(certificate);
            return this;
        }

#if NET452
        public WebSocketBuilder SetServerCertificateValidationCallback(RemoteCertificateValidationCallback validationCallback)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += validationCallback;
            return this;
        }

        public void CleanupServerCertificateValidationCallback(RemoteCertificateValidationCallback validationCallback)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback -= validationCallback;
        }
#endif

#if NETCOREAPP2_1
        public WebSocketBuilder ExpectServerCertificate(X509Certificate2 serverCertificate)
        {
            Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                return Kubernetes.CertificateValidationCallBack(sender, serverCertificate, certificate, chain, sslPolicyErrors);
            };

            return this;
        }

        public WebSocketBuilder SkipServerCertificateValidation()
        {
            Options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            return this;
        }

#endif // NETCOREAPP2_1

        public virtual async Task<WebSocket> BuildAndConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await this.WebSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            return this.WebSocket;
        }
    }
}
