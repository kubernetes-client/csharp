using k8s;

var config = KubernetesClientConfiguration.BuildDefaultConfig();
IKubernetes client = new Kubernetes(config);
Console.WriteLine("Starting Request!");

var list = client.CoreV1.ListNamespacedPod("default");
foreach (var item in list.Items)
{
    Console.WriteLine(item.Metadata.Name);
}

if (list.Items.Count == 0)
{
    Console.WriteLine("Empty!");
}