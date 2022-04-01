using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;
using k8s.Models;

namespace k8s.tests;

public class BasicTests
{
    // TODO: fail to setup asp.net core 6 on net48
    private class DummyHttpServer : System.IDisposable
    {
        private TcpListener server;
        private readonly Task loop;
        private volatile bool running = false;

        public string Addr => $"http://{server.LocalEndpoint}";

        public DummyHttpServer(object obj)
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 0);
            server.Start();
            running = true;
            loop = Task.Run(async () =>
            {
                while (running)
                {
                    var result = KubernetesJson.Serialize(obj);

                    var client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                    var stream = client.GetStream();
                    stream.Read(new byte[1024], 0, 1024); // TODO ensure full header

                    var writer = new StreamWriter(stream);
                    await writer.WriteLineAsync("HTTP/1.0 200 OK").ConfigureAwait(false);
                    await writer.WriteLineAsync("Content-Length: " + result.Length).ConfigureAwait(false);
                    await writer.WriteLineAsync("Content-Type: application/json").ConfigureAwait(false);
                    await writer.WriteLineAsync().ConfigureAwait(false);
                    await writer.WriteLineAsync(result).ConfigureAwait(false);

                    await writer.FlushAsync().ConfigureAwait(false);
                    client.Close();
                }
            });
        }

        public void Dispose()
        {
            try
            {
                running = false;
                server.Stop();
                loop.Wait();
                loop.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }

    [Fact]
    public async Task QueryPods()
    {
        using var server = new DummyHttpServer(new V1Pod()
        {
            Metadata = new V1ObjectMeta()
            {
                Name = "pod0",
            },
        });
        var client = new Kubernetes(new KubernetesClientConfiguration { Host = server.Addr });

        var pod = await client.ReadNamespacedPodAsync("pod", "default").ConfigureAwait(false);

        Assert.Equal("pod0", pod.Metadata.Name);
    }
}
