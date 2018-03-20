/*
 * These tests are for the netcoreapp2.1 version of the client (there are separate tests for netstandard that don't actually connect to a server).
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
 {
    /// <summary>
    ///     Tests for <see cref="KubeApiClient"/>'s exec-in-pod functionality.
    /// </summary>
    public class PodExecTests
        : WebSocketTestBase
    {
        /// <summary>
        ///     Create a new <see cref="KubeApiClient"/> exec-in-pod test suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        public PodExecTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        /// <summary>
        ///     Verify that the client can request execution of a command in a pod's default container, with only the STDOUT stream enabled.
        /// </summary>
        [Fact(DisplayName = "Can exec in pod's default container, STDOUT only")]
        public async Task Exec_DefaultContainer_StdOut()
        {
            if (!Debugger.IsAttached)
            {
                CancellationSource.CancelAfter(
                    TimeSpan.FromSeconds(5)
                );
            }
            await Host.StartAsync(TestCancellation);

            using (Kubernetes client = CreateTestClient())
            {
                Log.LogInformation("Invoking exec operation...");

                WebSocket clientSocket = await client.WebSocketNamespacedPodExecAsync(
                    name: "mypod",
                    @namespace: "mynamespace",
                    command: "/bin/bash",
                    container: "mycontainer",
                    stderr: false,
                    stdin: false,
                    stdout: true,
                    cancellationToken: TestCancellation
                );
                Assert.Equal(K8sProtocol.ChannelV1, clientSocket.SubProtocol); // For WebSockets, the Kubernetes API defaults to the binary channel (v1) protocol.

                Log.LogInformation("Client socket connected (socket state is {ClientSocketState}). Waiting for server-side socket to become available...", clientSocket.State);

                WebSocket serverSocket = await WebSocketTestAdapter.AcceptedPodExecV1Connection;
                Log.LogInformation("Server-side socket is now available (socket state is {ServerSocketState}). Sending data to server socket...", serverSocket.State);

                const int STDOUT = 1;
                const string expectedOutput = "This is text send to STDOUT.";

                int bytesSent = await SendMultiplexed(serverSocket, STDOUT, expectedOutput);
                Log.LogInformation("Sent {ByteCount} bytes to server socket; receiving from client socket...", bytesSent);

                (string receivedText, byte streamIndex, int bytesReceived) = await ReceiveTextMultiplexed(clientSocket);
                Log.LogInformation("Received {ByteCount} bytes from client socket ('{ReceivedText}', stream {StreamIndex}).", bytesReceived, receivedText, streamIndex);

                Assert.Equal(STDOUT, streamIndex);
                Assert.Equal(expectedOutput, receivedText);

                await Disconnect(clientSocket, serverSocket,
                    closeStatus: WebSocketCloseStatus.NormalClosure,
                    closeStatusDescription: "Normal Closure"
                );

                WebSocketTestAdapter.CompleteTest();
            }
        }
    }
}
