using k8s.Tests.Logging;
using k8s.Tests.Mock.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    /// <summary>
    ///     The base class for Kubernetes WebSocket test suites.
    /// </summary>
    public abstract class WebSocketTestBase : IDisposable
    {
        /// <summary>
        ///     The next server port to use.
        /// </summary>
        private static int nextPort = 13255;
        private bool disposedValue;
        private readonly ITestOutputHelper testOutput;

        /// <summary>
        ///     Create a new <see cref="WebSocketTestBase"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        protected WebSocketTestBase(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;

            int port = Interlocked.Increment(ref nextPort);

            // Useful to diagnose test timeouts.
            TestCancellation.Register(
                () => testOutput.WriteLine("Test-level cancellation token has been canceled."));

            ServerBaseAddress = new Uri($"http://localhost:{port}");
            WebSocketBaseAddress = new Uri($"ws://localhost:{port}");

            Host = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(ConfigureTestServerServices)
                .ConfigureLogging(ConfigureTestServerLogging)
                .UseUrls(ServerBaseAddress.AbsoluteUri)
                .Build();
        }

        /// <summary>
        ///     The test server's base address (http://).
        /// </summary>
        protected Uri ServerBaseAddress { get; }

        /// <summary>
        ///     The test server's base WebSockets address (ws://).
        /// </summary>
        protected Uri WebSocketBaseAddress { get; }

        /// <summary>
        ///     The test server's web host.
        /// </summary>
        protected IWebHost Host { get; }

        /// <summary>
        ///     Test adapter for accepting web sockets.
        /// </summary>
        protected WebSocketTestAdapter WebSocketTestAdapter { get; } = new WebSocketTestAdapter();

        /// <summary>
        ///     The source for cancellation tokens used by the test.
        /// </summary>
        protected CancellationTokenSource CancellationSource { get; } = new CancellationTokenSource();

        /// <summary>
        ///     A <see cref="System.Threading.CancellationToken"/> that can be used to cancel asynchronous operations.
        /// </summary>
        /// <seealso cref="CancellationSource"/>
        protected CancellationToken TestCancellation => CancellationSource.Token;

        /// <summary>
        ///     Configure services for the test server.
        /// </summary>
        /// <param name="services">
        ///     The service collection to configure.
        /// </param>
        protected virtual void ConfigureTestServerServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Inject WebSocketTestData.
            services.AddSingleton(WebSocketTestAdapter);
        }

        /// <summary>
        ///     Configure logging for the test server.
        /// </summary>
        /// <param name="services">
        ///     The logger factory to configure.
        /// </param>
        protected virtual void ConfigureTestServerLogging(ILoggingBuilder logging)
        {
            if (logging == null)
            {
                throw new ArgumentNullException(nameof(logging));
            }

            logging.ClearProviders(); // Don't log to console.
            logging.AddTestOutput(this.testOutput, LogLevel.Information);
        }

        /// <summary>
        ///     Create a Kubernetes client that uses the test server.
        /// </summary>
        /// <param name="credentials">
        ///     Optional <see cref="ServiceClientCredentials"/> to use for authentication (defaults to anonymous, i.e. no credentials).
        /// </param>
        /// <returns>
        ///     The configured client.
        /// </returns>
        protected virtual Kubernetes CreateTestClient(ServiceClientCredentials credentials = null)
        {
            return new Kubernetes(credentials ?? AnonymousClientCredentials.Instance) { BaseUri = ServerBaseAddress };
        }

        /// <summary>
        ///     Asynchronously disconnect client and server WebSockets using the standard handshake.
        /// </summary>
        /// <param name="clientSocket">
        ///     The client-side <see cref="WebSocket"/>.
        /// </param>
        /// <param name="serverSocket">
        ///     The server-side <see cref="WebSocket"/>.
        /// </param>
        /// <param name="closeStatus">
        ///     An optional <see cref="WebSocketCloseStatus"/> value indicating the reason for disconnection.
        ///
        ///     Defaults to <see cref="WebSocketCloseStatus.NormalClosure"/>.
        /// </param>
        /// <param name="closeStatusDescription">
        ///     An optional textual description of the reason for disconnection.
        ///
        ///     Defaults to "Normal Closure".
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected async Task Disconnect(WebSocket clientSocket, WebSocket serverSocket,
            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
            string closeStatusDescription = "Normal Closure")
        {
            if (clientSocket == null)
            {
                throw new ArgumentNullException(nameof(clientSocket));
            }

            if (serverSocket == null)
            {
                throw new ArgumentNullException(nameof(serverSocket));
            }

            testOutput.WriteLine("Disconnecting...");

            // Asynchronously perform the server's half of the handshake (the call to clientSocket.CloseAsync will block until it receives the server-side response).
            ArraySegment<byte> receiveBuffer = new byte[1024];
            Task closeServerSocket = serverSocket.ReceiveAsync(receiveBuffer, TestCancellation)
                .ContinueWith(async received =>
                {
                    if (received.IsFaulted)
                    {
                        testOutput.WriteLine("Server socket operation to receive Close message failed: {0}",
                            received.Exception.Flatten().InnerExceptions[0]);
                    }
                    else if (received.IsCanceled)
                    {
                        testOutput.WriteLine("Server socket operation to receive Close message was canceled.");
                    }
                    else
                    {
                        testOutput.WriteLine(
                            $"Received {received.Result.MessageType} message from server socket (expecting {WebSocketMessageType.Close}).");

                        if (received.Result.MessageType == WebSocketMessageType.Close)
                        {
                            testOutput.WriteLine(
                                $"Closing server socket (with status {received.Result.CloseStatus})...");

                            await serverSocket.CloseAsync(
                                received.Result.CloseStatus.Value,
                                received.Result.CloseStatusDescription,
                                TestCancellation).ConfigureAwait(false);

                            testOutput.WriteLine("Server socket closed.");
                        }

                        Assert.Equal(WebSocketMessageType.Close, received.Result.MessageType);
                    }
                });

            testOutput.WriteLine("Closing client socket...");

            await clientSocket.CloseAsync(closeStatus, closeStatusDescription, TestCancellation).ConfigureAwait(false);

            testOutput.WriteLine("Client socket closed.");

            await closeServerSocket.ConfigureAwait(false);

            testOutput.WriteLine("Disconnected.");

            Assert.Equal(closeStatus, clientSocket.CloseStatus);
            Assert.Equal(clientSocket.CloseStatus, serverSocket.CloseStatus);

            Assert.Equal(closeStatusDescription, clientSocket.CloseStatusDescription);
            Assert.Equal(clientSocket.CloseStatusDescription, serverSocket.CloseStatusDescription);
        }

        /// <summary>
        ///     Send text to a multiplexed substream over the specified WebSocket.
        /// </summary>
        /// <param name="webSocket">
        ///     The target <see cref="WebSocket"/>.
        /// </param>
        /// <param name="streamIndex">
        ///     The 0-based index of the target substream.
        /// </param>
        /// <param name="text">
        ///     The text to send.
        /// </param>
        /// <returns>
        ///     The number of bytes sent to the WebSocket.
        /// </returns>
        protected async Task<int> SendMultiplexed(WebSocket webSocket, byte streamIndex, string text)
        {
            if (webSocket == null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            byte[] payload = Encoding.ASCII.GetBytes(text);
            byte[] sendBuffer = new byte[payload.Length + 1];

            sendBuffer[0] = streamIndex;
            Array.Copy(payload, 0, sendBuffer, 1, payload.Length);

            await webSocket.SendAsync(sendBuffer, WebSocketMessageType.Binary,
                endOfMessage: true,
                cancellationToken: TestCancellation).ConfigureAwait(false);

            return sendBuffer.Length;
        }

        /// <summary>
        ///     Receive text from a multiplexed substream over the specified WebSocket.
        /// </summary>
        /// <param name="webSocket">
        ///     The target <see cref="WebSocket"/>.
        /// </param>
        /// <param name="text">
        ///     The text to send.
        /// </param>
        /// <returns>
        ///     A tuple containing the received text, 0-based substream index, and total bytes received.
        /// </returns>
        protected async Task<(string text, byte streamIndex, int totalBytes)> ReceiveTextMultiplexed(
            WebSocket webSocket)
        {
            if (webSocket == null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            byte[] receivedData;
            using (MemoryStream buffer = new MemoryStream())
            {
                byte[] receiveBuffer = new byte[1024];
                WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(receiveBuffer, TestCancellation).ConfigureAwait(false);
                if (receiveResult.MessageType != WebSocketMessageType.Binary)
                {
                    throw new IOException(
                        $"Received unexpected WebSocket message of type '{receiveResult.MessageType}'.");
                }

                buffer.Write(receiveBuffer, 0, receiveResult.Count);

                while (!receiveResult.EndOfMessage)
                {
                    receiveResult = await webSocket.ReceiveAsync(receiveBuffer, TestCancellation).ConfigureAwait(false);
                    buffer.Write(receiveBuffer, 0, receiveResult.Count);
                }

                buffer.Flush();

                receivedData = buffer.ToArray();
            }

            return (
                text: Encoding.ASCII.GetString(receivedData, 1, receivedData.Length - 1),
                streamIndex: receivedData[0],
                totalBytes: receivedData.Length);
        }



        /// <summary>
        ///     A <see cref="ServiceClientCredentials"/> implementation representing no credentials (i.e. anonymous).
        /// </summary>
        protected class AnonymousClientCredentials
            : ServiceClientCredentials
        {
            /// <summary>
            ///     The singleton instance of <see cref="AnonymousClientCredentials"/>.
            /// </summary>
            public static readonly AnonymousClientCredentials Instance = new AnonymousClientCredentials();

            /// <summary>
            ///     Create new <see cref="AnonymousClientCredentials"/>.
            /// </summary>
            private AnonymousClientCredentials()
            {
            }
        }

        /// <summary>
        ///     Event Id constants used in WebSocket tests.
        /// </summary>
        protected static class EventIds
        {
            /// <summary>
            ///     An error occurred while closing the server-side socket.
            /// </summary>
            private static readonly EventId ErrorClosingServerSocket = new EventId(1000, nameof(ErrorClosingServerSocket));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CancellationSource.Dispose();
                    Host.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WebSocketTestBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
