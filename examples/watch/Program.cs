using k8s;
using System;
using System.Threading;
using System.Threading.Tasks;

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

IKubernetes client = new Kubernetes(config);

var podlistResp = client.CoreV1.WatchListNamespacedPodAsync("default");

// C# 8 required https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8
await foreach (var (type, item) in podlistResp.ConfigureAwait(false))
{
    Console.WriteLine("==on watch event==");
    Console.WriteLine(type);
    Console.WriteLine(item.Metadata.Name);
    Console.WriteLine("==on watch event==");
}

#pragma warning disable CS8321 // Remove unused private members
void WatchUsingCallback(IKubernetes client)
#pragma warning restore CS8321 // Remove unused private members
{
    using (var podlistResp = client.CoreV1.WatchListNamespacedPod("default", onEvent: (type, item) =>
    {
        Console.WriteLine("==on watch event==");
        Console.WriteLine(type);
        Console.WriteLine(item.Metadata.Name);
        Console.WriteLine("==on watch event==");
    }))
    {
        Console.WriteLine("press ctrl + c to stop watching");

        var ctrlc = new ManualResetEventSlim(false);
        Console.CancelKeyPress += (sender, eventArgs) => ctrlc.Set();
        ctrlc.Wait();
    }
}