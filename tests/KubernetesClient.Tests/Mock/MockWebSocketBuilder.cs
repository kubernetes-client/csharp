#if !NETCOREAPP2_1

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace k8s.tests.Mock
{
    public class MockWebSocketBuilder : WebSocketBuilder
    {
        public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>();

        public Collection<X509Certificate2> Certificates { get; } = new Collection<X509Certificate2>();

        public Uri Uri { get; private set; }

        public WebSocket PublicWebSocket => this.WebSocket;

        public override WebSocketBuilder AddClientCertificate(X509Certificate2 certificate)
        {
            this.Certificates.Add(certificate);
            return this;
        }

        public override Task<WebSocket> BuildAndConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            this.Uri = uri;

            return Task.FromResult(this.PublicWebSocket);
        }

        public override WebSocketBuilder SetRequestHeader(string headerName, string headerValue)
        {
            this.RequestHeaders.Add(headerName, headerValue);
            return this;
        }
    }
}

#endif // !NETCOREAPP2_1
