#if NETCOREAPP2_1

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace k8s
{
    /// <summary>
    /// The <see cref="WebSocketBuilder"/> creates a new <see cref="WebSocket"/> object which connects to a remote WebSocket.
    /// </summary>
    public sealed class WebSocketBuilder
    {
        public KubernetesWebSocketOptions Options { get; } = new KubernetesWebSocketOptions();

        public WebSocketBuilder()
        {
        }

        public WebSocketBuilder SetRequestHeader(string headerName, string headerValue)
        {
            Options.RequestHeaders[headerName] = headerValue;

            return this;
        }

        public WebSocketBuilder AddClientCertificate(X509Certificate2 certificate)
        {
            Options.ClientCertificates.Add(certificate);

            return this;
        }

        public WebSocketBuilder ExpectServerCertificate(X509Certificate2 serverCertificate)
        {
            Options.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors != SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    return false;
                }

                try
                {
                    using (X509Chain certificateChain = new X509Chain())
                    {
                        certificateChain.ChainPolicy.ExtraStore.Add(serverCertificate);
                        certificateChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                        certificateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                        return certificateChain.Build(
                            (X509Certificate2)certificate
                        );
                    }
                }
                catch (Exception chainException)
                {
                    Debug.WriteLine(chainException);

                    return false;
                }
            };

            return this;
        }

        public WebSocketBuilder SkipServerCertificateValidation()
        {
            Options.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            return this;
        }

        public async Task<WebSocket> BuildAndConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            return await CoreFX.K8sWebSocket.ConnectAsync(uri, Options, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Options for connecting to Kubernetes web sockets.
    /// </summary>
    public class KubernetesWebSocketOptions
    {
        /// <summary>
        ///     The default size (in bytes) for WebSocket send / receive buffers.
        /// </summary>
        public static readonly int DefaultBufferSize = 2048;

        /// <summary>
        ///     Create new <see cref="KubernetesWebSocketOptions"/>.
        /// </summary>
        public KubernetesWebSocketOptions()
        {
        }

        /// <summary>
        ///     The requested size (in bytes) of the WebSocket send buffer.
        /// </summary>
        public int SendBufferSize { get; set; } = 2048;

        /// <summary>
        ///     The requested size (in bytes) of the WebSocket receive buffer.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 2048;

        /// <summary>
        ///     Custom request headers (if any).
        /// </summary>
        public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Requested sub-protocols (if any).
        /// </summary>
        public List<string> RequestedSubProtocols { get; } = new List<string>();

        /// <summary>
        ///     Client certificates (if any) to use for authentication.
        /// </summary>
        public List<X509Certificate2> ClientCertificates = new List<X509Certificate2>();

        /// <summary>
        ///     An optional delegate to use for authenticating the remote server certificate.
        /// </summary>
        public RemoteCertificateValidationCallback ServerCertificateCustomValidationCallback { get; set; }

        /// <summary>
        ///     An <see cref="SslProtocols"/> value representing the SSL protocols that the client supports.
        /// </summary>
        public SslProtocols EnabledSslProtocols { get; set; } = SslProtocols.Tls;

        /// <summary>
        ///     The WebSocket keep-alive interval.
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(5);
    }
}

#endif // NETCOREAPP2_1
