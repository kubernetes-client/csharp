using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using k8s.Models;
using k8s.Tests.Mock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Rest;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests
{
    public class AuthTests
        : TestBase
    {
        public AuthTests(ITestOutputHelper testOutput) : base(testOutput)
        {
        }

        private static HttpOperationResponse<V1PodList> ExecuteListPods(IKubernetes client)
        {
            return client.ListNamespacedPodWithHttpMessagesAsync("default").Result;
        }

        [Fact]
        public void Anonymous()
        {
            using (var server = new MockKubeApiServer(TestOutput))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = ExecuteListPods(client);

                Assert.True(listTask.Response.IsSuccessStatusCode);
                Assert.Equal(1, listTask.Body.Items.Count);
            }

            using (var server = new MockKubeApiServer(TestOutput, cxt =>
            {
                cxt.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return Task.FromResult(false);
            }))
            {
                var client = new Kubernetes(new KubernetesClientConfiguration
                {
                    Host = server.Uri.ToString()
                });

                var listTask = ExecuteListPods(client);

                Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
            }
        }

        [Fact]
        public void BasicAuth()
        {
            const string testName = "test_name";
            const string testPassword = "test_password";

            using (var server = new MockKubeApiServer(TestOutput, cxt =>
            {
                var header = cxt.Request.Headers["Authorization"].FirstOrDefault();

                var expect = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{testName}:{testPassword}")))
                    .ToString();

                if (header != expect)
                {
                    cxt.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }))
            {
                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        Username = testName,
                        Password = testPassword
                    });

                    var listTask = ExecuteListPods(client);
                    Assert.True(listTask.Response.IsSuccessStatusCode);
                    Assert.Equal(1, listTask.Body.Items.Count);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        Username = "wrong name",
                        Password = testPassword
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        Username = testName,
                        Password = "wrong password"
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        Username = "both wrong",
                        Password = "wrong password"
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString()
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        Username = "xx"
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }
            }
        }

        [Fact]
        public void Cert()
        {
            var serverCertificateData = File.ReadAllText("assets/apiserver-pfx-data.txt");

            var clientCertificateKeyData = File.ReadAllText("assets/client-key-data.txt");
            var clientCertificateData = File.ReadAllText("assets/client-certificate-data.txt");

            var serverCertificate = new X509Certificate2(Convert.FromBase64String(serverCertificateData));
            var clientCertificate = new X509Certificate2(Convert.FromBase64String(clientCertificateData));

            var clientCertificateValidationCalled = false;

            using (var server = new MockKubeApiServer(TestOutput, listenConfigure: options =>
            {
                options.UseHttps(new HttpsConnectionAdapterOptions
                {
                    ServerCertificate = serverCertificate,
                    ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                    ClientCertificateValidation = (certificate, chain, valid) =>
                    {
                        clientCertificateValidationCalled = true;
                        return clientCertificate.Equals(certificate);
                    }
                });
            }))
            {
                {
                    clientCertificateValidationCalled = false;
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        ClientCertificateData = clientCertificateData,
                        ClientCertificateKeyData = clientCertificateKeyData,
                        SslCaCert = serverCertificate,
                        SkipTlsVerify = false
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.True(clientCertificateValidationCalled);
                    Assert.True(listTask.Response.IsSuccessStatusCode);
                    Assert.Equal(1, listTask.Body.Items.Count);
                }

                {
                    clientCertificateValidationCalled = false;
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        ClientCertificateData = clientCertificateData,
                        ClientCertificateKeyData = clientCertificateKeyData,
                        SkipTlsVerify = true
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.True(clientCertificateValidationCalled);
                    Assert.True(listTask.Response.IsSuccessStatusCode);
                    Assert.Equal(1, listTask.Body.Items.Count);
                }

                {
                    clientCertificateValidationCalled = false;
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        ClientCertificateFilePath = "assets/client.crt", // TODO amazoning why client.crt != client-data.txt
                        ClientKeyFilePath = "assets/client.key",
                        SkipTlsVerify = true
                    });

                    Assert.ThrowsAny<Exception>(() => ExecuteListPods(client));
                    Assert.True(clientCertificateValidationCalled);
                }

                {
                    clientCertificateValidationCalled = false;
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        SkipTlsVerify = true
                    });

                    Assert.ThrowsAny<Exception>(() => ExecuteListPods(client));
                    Assert.False(clientCertificateValidationCalled);
                }
            }
        }

        [Fact]
        public void Token()
        {
            const string token = "testingtoken";

            using (var server = new MockKubeApiServer(TestOutput, cxt =>
            {
                var header = cxt.Request.Headers["Authorization"].FirstOrDefault();

                var expect = new AuthenticationHeaderValue("Bearer", token).ToString();

                if (header != expect)
                {
                    cxt.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }))
            {
                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        AccessToken = token
                    });

                    var listTask = ExecuteListPods(client);
                    Assert.True(listTask.Response.IsSuccessStatusCode);
                    Assert.Equal(1, listTask.Body.Items.Count);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        AccessToken = "wrong token"
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }


                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString(),
                        Username = "wrong name",
                        Password = "same password"
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }

                {
                    var client = new Kubernetes(new KubernetesClientConfiguration
                    {
                        Host = server.Uri.ToString()
                    });

                    var listTask = ExecuteListPods(client);

                    Assert.Equal(HttpStatusCode.Unauthorized, listTask.Response.StatusCode);
                }
            }
        }
    }
}
