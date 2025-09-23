using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;


var config = KubernetesClientConfiguration.BuildDefaultConfig();
var client = new Kubernetes(config);


var pod = new V1Pod
{
    Metadata = new V1ObjectMeta { Name = "nginx-pod" },
    Spec = new V1PodSpec
    {
        Containers =
        [
            new V1Container
            {
                Name = "nginx",
                Image = "nginx",
                Resources = new V1ResourceRequirements
                {
                    Requests = new Dictionary<string, ResourceQuantity>()
                    {
                        ["cpu"] = "100m",
                    },
                },
            },
        ],
    },
};
{
    var created = await client.CoreV1.CreateNamespacedPodAsync(pod, "default").ConfigureAwait(false);
    Console.WriteLine($"Created pod: {created.Metadata.Name}");
}

{
    var patchStr = @"
    {
    ""spec"": {
        ""containers"": [
            {
                ""name"": ""nginx"",
                ""resources"": {
                    ""requests"": {
                        ""cpu"": ""200m""
                    }
                }
            }
        ]
    }
    }";

    var patch = await client.CoreV1.PatchNamespacedPodResizeAsync(new V1Patch(patchStr, V1Patch.PatchType.MergePatch), "nginx-pod", "default").ConfigureAwait(false);

    if (patch?.Spec?.Containers?.Count > 0 &&
        patch.Spec.Containers[0].Resources?.Requests != null &&
        patch.Spec.Containers[0].Resources.Requests.TryGetValue("cpu", out var cpuQty))
    {
        Console.WriteLine($"CPU request: {cpuQty}");
    }
}
