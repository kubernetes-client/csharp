/*
 * These tests are for the netcoreapp2.1 version of the client (there are separate tests for netstandard that don't actually connect to a server).
 */

using k8s.Models;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                    webSocketSubProtol: WebSocketProtocol.ChannelWebSocketProtocol,
                    cancellationToken: TestCancellation
                );
                Assert.Equal(WebSocketProtocol.ChannelWebSocketProtocol, clientSocket.SubProtocol); // For WebSockets, the Kubernetes API defaults to the binary channel (v1) protocol.

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


        [Fact]
        public void GetExitCodeOrThrow_Success()
        {
            var status = new V1Status()
            {
                Metadata = null,
                Status = "Success",
            };

            Assert.Equal(0, Kubernetes.GetExitCodeOrThrow(status));
        }

        [Fact]
        public void GetExitCodeOrThrow_NonZeroExitCode()
        {
            var status = new V1Status()
            {
                Metadata = null,
                Status = "Failure",
                Message = "command terminated with non-zero exit code: Error executing in Docker Container: 1",
                Reason = "NonZeroExitCode",
                Details = new V1StatusDetails()
                {
                    Causes = new List<V1StatusCause>()
                      {
                          new V1StatusCause()
                          {
                               Reason = "ExitCode",
                               Message = "1"
                          }
                      }
                }
            };

            Assert.Equal(1, Kubernetes.GetExitCodeOrThrow(status));
        }

        [Fact]
        public void GetExitCodeOrThrow_InvalidExitCode()
        {
            var status = new V1Status()
            {
                Metadata = null,
                Status = "Failure",
                Message = "command terminated with non-zero exit code: Error executing in Docker Container: 1",
                Reason = "NonZeroExitCode",
                Details = new V1StatusDetails()
                {
                    Causes = new List<V1StatusCause>()
                    {
                        new V1StatusCause()
                        {
                            Reason = "ExitCode",
                            Message = "abc"
                        }
                    }
                }
            };

            var ex = Assert.Throws<KubernetesException>(() => Kubernetes.GetExitCodeOrThrow(status));
            Assert.Equal(status, ex.Status);
        }

        [Fact]
        public void GetExitCodeOrThrow_NoExitCode()
        {
            var status = new V1Status()
            {
                Metadata = null,
                Status = "Failure",
                Message = "command terminated with non-zero exit code: Error executing in Docker Container: 1",
                Reason = "NonZeroExitCode",
                Details = new V1StatusDetails()
                {
                    Causes = new List<V1StatusCause>()
                    {
                    }
                }
            };

            var ex = Assert.Throws<KubernetesException>(() => Kubernetes.GetExitCodeOrThrow(status));
            Assert.Equal(status, ex.Status);
        }

        [Fact]
        public void GetExitCodeOrThrow_OtherError()
        {
            var status = new V1Status()
            {
                Metadata = null,
                Status = "Failure",
                Reason = "SomethingElse"
            };

            var ex = Assert.Throws<KubernetesException>(() => Kubernetes.GetExitCodeOrThrow(status));
            Assert.Equal(status, ex.Status);
        }

        [Fact]
        public async Task NamespacedPodExecAsync_ActionNull()
        {
            using (MemoryStream stdIn = new MemoryStream())
            using (MemoryStream stdOut = new MemoryStream())
            using (MemoryStream stdErr = new MemoryStream())
            using (MemoryStream errorStream = new MemoryStream())
            {
                var muxedStream = new Moq.Mock<IStreamDemuxer>();
                muxedStream.Setup(m => m.GetStream(null, ChannelIndex.StdIn)).Returns(stdIn);
                muxedStream.Setup(m => m.GetStream(ChannelIndex.StdOut, null)).Returns(stdOut);
                muxedStream.Setup(m => m.GetStream(ChannelIndex.StdErr, null)).Returns(stdErr);
                muxedStream.Setup(m => m.GetStream(ChannelIndex.Error, null)).Returns(errorStream);

                var kubernetesMock = new Moq.Mock<Kubernetes>(
                    new object[] { Moq.Mock.Of<ServiceClientCredentials>(), new DelegatingHandler[] { } });
                var command = new string[] { "/bin/bash", "-c", "echo Hello, World!" };

                kubernetesMock.Setup(m => m.MuxedStreamNamespacedPodExecAsync("pod-name", "pod-namespace", command, "my-container", true, true, true, false, WebSocketProtocol.V4BinaryWebsocketProtocol, null, CancellationToken.None))
                    .Returns(Task.FromResult(muxedStream.Object));

                using (Kubernetes client = kubernetesMock.Object)
                {
                    await Assert.ThrowsAsync<ArgumentNullException>(() => client.NamespacedPodExecAsync("pod-name", "pod-namespace", "my-container", command, false, null, CancellationToken.None)).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task NamespacedPodExecAsync_HttpException_WithStatus()
        {
            var kubernetesMock = new Moq.Mock<Kubernetes>(
                new object[] { Moq.Mock.Of<ServiceClientCredentials>(), new DelegatingHandler[] { } });
            var command = new string[] { "/bin/bash", "-c", "echo Hello, World!" };
            var handler = new ExecAsyncCallback((stdIn, stdOut, stdError) => Task.CompletedTask);

            var status = new V1Status();
            kubernetesMock.Setup(m => m.MuxedStreamNamespacedPodExecAsync("pod-name", "pod-namespace", command, "my-container", true, true, true, false, WebSocketProtocol.V4BinaryWebsocketProtocol, null, CancellationToken.None))
                .Throws(new HttpOperationException() { Body = status });

            using (Kubernetes client = kubernetesMock.Object)
            {
                var ex = await Assert.ThrowsAsync<KubernetesException>(() => client.NamespacedPodExecAsync("pod-name", "pod-namespace", "my-container", command, false, handler, CancellationToken.None)).ConfigureAwait(false);
                Assert.Same(status, ex.Status);
            }
        }

        [Fact]
        public async Task NamespacedPodExecAsync_HttpException_NoStatus()
        {
            var kubernetesMock = new Moq.Mock<Kubernetes>(
                new object[] { Moq.Mock.Of<ServiceClientCredentials>(), new DelegatingHandler[] { } });
            var command = new string[] { "/bin/bash", "-c", "echo Hello, World!" };
            var handler = new ExecAsyncCallback((stdIn, stdOut, stdError) => Task.CompletedTask);

            var exception = new HttpOperationException();
            kubernetesMock.Setup(m => m.MuxedStreamNamespacedPodExecAsync("pod-name", "pod-namespace", command, "my-container", true, true, true, false, WebSocketProtocol.V4BinaryWebsocketProtocol, null, CancellationToken.None))
                .Throws(exception);

            using (Kubernetes client = kubernetesMock.Object)
            {
                var ex = await Assert.ThrowsAsync<HttpOperationException>(() => client.NamespacedPodExecAsync("pod-name", "pod-namespace", "my-container", command, false, handler, CancellationToken.None)).ConfigureAwait(false);
                Assert.Same(exception, ex);
            }
        }

        [Fact]
        public async Task NamespacedPodExecAsync_GenericException()
        {
            var kubernetesMock = new Moq.Mock<Kubernetes>(
                new object[] { Moq.Mock.Of<ServiceClientCredentials>(), new DelegatingHandler[] { } });
            var command = new string[] { "/bin/bash", "-c", "echo Hello, World!" };
            var handler = new ExecAsyncCallback((stdIn, stdOut, stdError) => Task.CompletedTask);

            var exception = new Exception();
            kubernetesMock.Setup(m => m.MuxedStreamNamespacedPodExecAsync("pod-name", "pod-namespace", command, "my-container", true, true, true, false, WebSocketProtocol.V4BinaryWebsocketProtocol, null, CancellationToken.None))
                .Throws(exception);

            using (Kubernetes client = kubernetesMock.Object)
            {
                var ex = await Assert.ThrowsAsync<Exception>(() => client.NamespacedPodExecAsync("pod-name", "pod-namespace", "my-container", command, false, handler, CancellationToken.None)).ConfigureAwait(false);
                Assert.Same(exception, ex);
            }
        }

        [Fact]
        public async Task NamespacedPodExecAsync_ExitCode_NonZero()
        {
            var processStatus = new V1Status()
            {
                Metadata = null,
                Status = "Failure",
                Message = "command terminated with non-zero exit code: Error executing in Docker Container: 1",
                Reason = "NonZeroExitCode",
                Details = new V1StatusDetails()
                {
                    Causes = new List<V1StatusCause>()
                      {
                          new V1StatusCause()
                          {
                               Reason = "ExitCode",
                               Message = "1"
                          }
                      }
                }
            };

            var processStatusJson = Encoding.UTF8.GetBytes(SafeJsonConvert.SerializeObject(processStatus));
            var handler = new ExecAsyncCallback((stdIn, stdOut, stdError) => Task.CompletedTask);

            using (MemoryStream stdIn = new MemoryStream())
            using (MemoryStream stdOut = new MemoryStream())
            using (MemoryStream stdErr = new MemoryStream())
            using (MemoryStream errorStream = new MemoryStream(processStatusJson))
            {
                var muxedStream = new Moq.Mock<IStreamDemuxer>();
                muxedStream.Setup(m => m.GetStream(null, ChannelIndex.StdIn)).Returns(stdIn);
                muxedStream.Setup(m => m.GetStream(ChannelIndex.StdOut, null)).Returns(stdOut);
                muxedStream.Setup(m => m.GetStream(ChannelIndex.StdErr, null)).Returns(stdErr);
                muxedStream.Setup(m => m.GetStream(ChannelIndex.Error, null)).Returns(errorStream);

                var kubernetesMock = new Moq.Mock<Kubernetes>(
                    new object[] { Moq.Mock.Of<ServiceClientCredentials>(), new DelegatingHandler[] { } });
                var command = new string[] { "/bin/bash", "-c", "echo Hello, World!" };

                var exception = new Exception();
                kubernetesMock.Setup(m => m.MuxedStreamNamespacedPodExecAsync("pod-name", "pod-namespace", command, "my-container", true, true, true, false, WebSocketProtocol.V4BinaryWebsocketProtocol, null, CancellationToken.None))
                    .Returns(Task.FromResult(muxedStream.Object));

                using (Kubernetes client = kubernetesMock.Object)
                {
                    var exitCode = await client.NamespacedPodExecAsync("pod-name", "pod-namespace", "my-container", command, false, handler, CancellationToken.None).ConfigureAwait(false);
                    Assert.Equal(1, exitCode);
                }
            }
        }
    }
}
