using k8s;
using k8s.Models;

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
IKubernetes client = new Kubernetes(config);
Console.WriteLine("Starting Request!");

var pod = client.CoreV1.ListNamespacedPod("default").Items.First();
var name = pod.Metadata.Name;
PrintLabels(pod);

var patchStr = @"
{
    ""metadata"": {
        ""labels"": {
            ""test"": ""test""
        }
    }
}";

client.CoreV1.PatchNamespacedPod(new V1Patch(patchStr, V1Patch.PatchType.MergePatch), name, "default");
PrintLabels(client.CoreV1.ReadNamespacedPod(name, "default"));

static void PrintLabels(V1Pod pod)
{
    Console.WriteLine($"Labels: for {pod.Metadata.Name}");
    foreach (var (k, v) in pod.Metadata.Labels)
    {
        Console.WriteLine($"{k} : {v}");
    }
    Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=");
}
