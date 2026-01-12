/*
 * These tests are only for the netstandard version of the client (there are separate tests for netcoreapp that connect to a local test-hosted server).
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace k8s.Tests
{
    public class KubernetesExecTests
    {
        private class CaptureHandler : HttpMessageHandler
        {
            public HttpRequestMessage LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new MemoryStream()),
                    Version = HttpVersion.Version20,
                };

                return Task.FromResult(response);
            }
        }

        [Fact]
        public async Task WebSocketNamespacedPodExecAsync()
        {
            var handler = new CaptureHandler();
            var clientConfiguration = new KubernetesClientConfiguration()
            {
                Host = "http://localhost",
                Username = "my-user",
                Password = "my-secret-password",
            };

            var client = new Kubernetes(clientConfiguration, handler)
            {
                BaseUri = new Uri("http://localhost"),
            };

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
                cancellationToken: CancellationToken.None).ConfigureAwait(true);

            var expectedHeaders = new Dictionary<string, string>()
            {
                { "X-My-Header", "myHeaderValue myHeaderValue2" },
                { "Authorization", "Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=" },
            };

            Assert.NotNull(webSocket);
            Assert.Equal(
                new Uri(
                    "http://localhost/api/v1/namespaces/mynamespace/pods/mypod/exec?command=%2Fbin%2Fbash&command=-c&command=echo%20Hello%2C%20World%0Aexit%200%0A&container=mycontainer&stderr=1&stdin=1&stdout=1&tty=1"),
                handler.LastRequest.RequestUri);
            Assert.Equal(HttpVersion.Version20, handler.LastRequest.Version);
            Assert.True(handler.LastRequest.Headers.TryGetValues("X-My-Header", out var execHeaderValues));
            Assert.Equal("myHeaderValue myHeaderValue2", execHeaderValues.Single());
            Assert.NotNull(handler.LastRequest.Headers.Authorization);
            Assert.Equal("Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=", handler.LastRequest.Headers.Authorization.ToString());
        }

        [Fact]
        public async Task WebSocketNamespacedPodPortForwardAsync()
        {
            var handler = new CaptureHandler();
            Kubernetes client = new Kubernetes(new KubernetesClientConfiguration()
            {
                Host = "http://localhost",
                Username = "my-user",
                Password = "my-secret-password",
            }, handler);

            var webSocket = await client.WebSocketNamespacedPodPortForwardAsync(
                name: "mypod",
                @namespace: "mynamespace",
                ports: new int[] { 80, 8080 },
                customHeaders: new Dictionary<string, List<string>>()
                {
                    { "X-My-Header", new List<string>() { "myHeaderValue", "myHeaderValue2" } },
                },
                cancellationToken: CancellationToken.None).ConfigureAwait(true);

            var expectedHeaders = new Dictionary<string, string>()
            {
                { "X-My-Header", "myHeaderValue myHeaderValue2" },
                { "Authorization", "Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=" },
            };

            Assert.NotNull(webSocket);
            Assert.Equal(
                new Uri("http://localhost/api/v1/namespaces/mynamespace/pods/mypod/portforward?ports=80&ports=8080"),
                handler.LastRequest.RequestUri); // Did we connect to the correct URL?
            Assert.Equal(HttpVersion.Version20, handler.LastRequest.Version);
            Assert.True(handler.LastRequest.Headers.TryGetValues("X-My-Header", out var portForwardHeaderValues));
            Assert.Equal("myHeaderValue myHeaderValue2", portForwardHeaderValues.Single());
            Assert.NotNull(handler.LastRequest.Headers.Authorization);
            Assert.Equal(expectedHeaders["Authorization"], handler.LastRequest.Headers.Authorization.ToString());
        }

        [Fact]
        public async Task WebSocketNamespacedPodAttachAsync()
        {
            var handler = new CaptureHandler();
            Kubernetes client = new Kubernetes(new KubernetesClientConfiguration()
            {
                Host = "http://localhost",
                Username = "my-user",
                Password = "my-secret-password",
            }, handler)
            {
                BaseUri = new Uri("http://localhost"),
            };

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
                cancellationToken: CancellationToken.None).ConfigureAwait(true);

            var expectedHeaders = new Dictionary<string, string>()
            {
                { "X-My-Header", "myHeaderValue myHeaderValue2" },
                { "Authorization", "Basic bXktdXNlcjpteS1zZWNyZXQtcGFzc3dvcmQ=" },
            };

            Assert.NotNull(webSocket); // Did the method return the correct web socket?
            Assert.Equal(
                new Uri(
                    "http://localhost:80/api/v1/namespaces/mynamespace/pods/mypod/attach?stderr=1&stdin=1&stdout=1&tty=1&container=my-container"),
                handler.LastRequest.RequestUri); // Did we connect to the correct URL?
            Assert.Equal(HttpVersion.Version20, handler.LastRequest.Version);
            Assert.True(handler.LastRequest.Headers.TryGetValues("X-My-Header", out var attachHeaderValues));
            Assert.Equal("myHeaderValue myHeaderValue2", attachHeaderValues.Single());
            Assert.NotNull(handler.LastRequest.Headers.Authorization);
            Assert.Equal(expectedHeaders["Authorization"], handler.LastRequest.Headers.Authorization.ToString());
        }
    }
}
