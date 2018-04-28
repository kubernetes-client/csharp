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
        private readonly ITestOutputHelper testOutput;

        /// <summary>
        ///     Create a new <see cref="KubeApiClient"/> exec-in-pod test suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        public PodExecTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
            this.testOutput = testOutput;
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
                testOutput.WriteLine("Invoking exec operation...");

                WebSocket clientSocket = await client.WebSocketNamespacedPodExecAsync(
                    name: "mypod",
                    @namespace: "mynamespace",
                    command: new string[] { "/bin/bash" },
                    container: "mycontainer",
                    stderr: false,
                    stdin: false,
                    stdout: true,
                    cancellationToken: TestCancellation
                );
                Assert.Equal(K8sProtocol.ChannelV1, clientSocket.SubProtocol); // For WebSockets, the Kubernetes API defaults to the binary channel (v1) protocol.

                testOutput.WriteLine($"Client socket connected (socket state is {clientSocket.State}). Waiting for server-side socket to become available...");

                WebSocket serverSocket = await WebSocketTestAdapter.AcceptedPodExecV1Connection;
                testOutput.WriteLine($"Server-side socket is now available (socket state is {serverSocket.State}). Sending data to server socket...");

                const int STDOUT = 1;
                const string expectedOutput = "This is text send to STDOUT.";

                int bytesSent = await SendMultiplexed(serverSocket, STDOUT, expectedOutput);
                testOutput.WriteLine($"Sent {bytesSent} bytes to server socket; receiving from client socket...");

                (string receivedText, byte streamIndex, int bytesReceived) = await ReceiveTextMultiplexed(clientSocket);
                testOutput.WriteLine($"Received {bytesReceived} bytes from client socket ('{receivedText}', stream {streamIndex}).");

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
