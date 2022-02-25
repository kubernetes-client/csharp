/*
 * These tests are only for the netstandard version of the client (there are separate tests for netcoreapp that connect to a local test-hosted server).
 */

using k8s.Tests.Mock;
using k8s.Autorest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    public class KubernetesExecTests
    {
        /// <summary>
        /// Tests the <see cref="Kubernetes.WebSocketNamespacedPodExecWithHttpMessagesAsync(string, string, string, string, bool, bool, bool, bool, Dictionary{string, List{string}}, CancellationToken)"/>
        /// method. Changes the <see cref="WebSocketBuilder"/> used by the client with a mock builder, so this test never hits the network.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> which represents the asynchronous test.
        /// </returns>
        [Fact]
        public async Task WebSocketNamespacedPodExecAsync()
        {
            Kubernetes client = new Kubernetes(new KubernetesClientConfiguration()
            {
                Host = "http://localhost",
                Username = "my-user",
                Password = "my-secret-password",
            });
            client.BaseUri = new Uri("http://localhost");

            MockWebSocketBuilder mockWebSocketBuilder = new MockWebSocketBuilder();
            client.CreateWebSocketBuilder = () => mockWebSocketBuilder;

            var webSocket = await client.WebSocketNamespacedPodExecAsync(
                name: "mypod",
                @namespace: "mynamespace",
                command: new string[] { "/bin/bash", "-c", $"echo Hello, World\nexit 0\n" },
                container: "mycontainer",
                stderr: true,
                stdin: true,
                stdout: true,
                tty: true,
                customHeaders: new Dictionary<string, List<string>>()
                {
                    { "X-My-Header", new List<string>() { "myHeaderValue", "myHeaderValue2" } },
                },
                cancellationToken: CancellationToken.None).ConfigureAwait(false);

            var expectedHeaders = new Dictionary<string, string>()
            {
                { "X-My-Header", "myHeaderValue myHeaderValue2" },
                { "Authorization", "Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=" },
            };

            Assert.Equal(
                mockWebSocketBuilder.PublicWebSocket,
                webSocket); // Did the method return the correct web socket?
            Assert.Equal(
                new Uri(
                    "ws://localhost/api/v1/namespaces/mynamespace/pods/mypod/exec?command=%2Fbin%2Fbash&command=-c&command=echo%20Hello%2C%20World%0Aexit%200%0A&container=mycontainer&stderr=1&stdin=1&stdout=1&tty=1"),
                mockWebSocketBuilder.Uri); // Did we connect to the correct URL?
            Assert.Empty(mockWebSocketBuilder.Certificates); // No certificates were used in this test
            Assert.Equal(expectedHeaders, mockWebSocketBuilder.RequestHeaders); // Did we use the expected headers
        }

        [Fact]
        public async Task WebSocketNamespacedPodPortForwardAsync()
        {
            Kubernetes client = new Kubernetes(new KubernetesClientConfiguration()
            {
                Host = "http://localhost",
                Username = "my-user",
                Password = "my-secret-password",
            });

            MockWebSocketBuilder mockWebSocketBuilder = new MockWebSocketBuilder();
            client.CreateWebSocketBuilder = () => mockWebSocketBuilder;

            var webSocket = await client.WebSocketNamespacedPodPortForwardAsync(
                name: "mypod",
                @namespace: "mynamespace",
                ports: new int[] { 80, 8080 },
                customHeaders: new Dictionary<string, List<string>>()
                {
                    { "X-My-Header", new List<string>() { "myHeaderValue", "myHeaderValue2" } },
                },
                cancellationToken: CancellationToken.None).ConfigureAwait(false);

            var expectedHeaders = new Dictionary<string, string>()
            {
                { "X-My-Header", "myHeaderValue myHeaderValue2" },
                { "Authorization", "Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=" },
            };

            Assert.Equal(
                mockWebSocketBuilder.PublicWebSocket,
                webSocket); // Did the method return the correct web socket?
            Assert.Equal(
                new Uri("ws://localhost/api/v1/namespaces/mynamespace/pods/mypod/portforward?ports=80&ports=8080"),
                mockWebSocketBuilder.Uri); // Did we connect to the correct URL?
            Assert.Empty(mockWebSocketBuilder.Certificates); // No certificates were used in this test
            Assert.Equal(expectedHeaders, mockWebSocketBuilder.RequestHeaders); // Did we use the expected headers
        }

        [Fact]
        public async Task WebSocketNamespacedPodAttachAsync()
        {
            Kubernetes client = new Kubernetes(new KubernetesClientConfiguration()
            {
                Host = "http://localhost",
                Username = "my-user",
                Password = "my-secret-password",
            });
            client.BaseUri = new Uri("http://localhost");

            MockWebSocketBuilder mockWebSocketBuilder = new MockWebSocketBuilder();
            client.CreateWebSocketBuilder = () => mockWebSocketBuilder;

            var webSocket = await client.WebSocketNamespacedPodAttachAsync(
                name: "mypod",
                @namespace: "mynamespace",
                container: "my-container",
                stderr: true,
                stdin: true,
                stdout: true,
                tty: true,
                customHeaders: new Dictionary<string, List<string>>()
                {
                    { "X-My-Header", new List<string>() { "myHeaderValue", "myHeaderValue2" } },
                },
                cancellationToken: CancellationToken.None).ConfigureAwait(false);

            var expectedHeaders = new Dictionary<string, string>()
            {
                { "X-My-Header", "myHeaderValue myHeaderValue2" },
                { "Authorization", "Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=" },
            };

            Assert.Equal(
                mockWebSocketBuilder.PublicWebSocket,
                webSocket); // Did the method return the correct web socket?
            Assert.Equal(
                new Uri(
                    "ws://localhost:80/api/v1/namespaces/mynamespace/pods/mypod/attach?stderr=1&stdin=1&stdout=1&tty=1&container=my-container"),
                mockWebSocketBuilder.Uri); // Did we connect to the correct URL?
            Assert.Empty(mockWebSocketBuilder.Certificates); // No certificates were used in this test
            Assert.Equal(expectedHeaders, mockWebSocketBuilder.RequestHeaders); // Did we use the expected headers
        }
    }
}
