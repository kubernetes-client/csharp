using System.Globalization;
using System.Text.Json;
using Json.Patch;
using k8s;
using k8s.Models;

double ConvertToUnixTimestamp(DateTime date)
{
    var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    var diff = date.ToUniversalTime() - origin;
    return Math.Floor(diff.TotalSeconds);
}

async Task RestartDaemonSetAsync(string name, string @namespace, IKubernetes client)
{
    var daemonSet = await client.ReadNamespacedDaemonSetAsync(name, @namespace);
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    var old = JsonSerializer.SerializeToDocument(daemonSet, options);

    var restart = new Dictionary<string, string>(daemonSet.Spec.Template.Metadata.Annotations)
    {
        ["date"] = ConvertToUnixTimestamp(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture)
    };

    daemonSet.Spec.Template.Metadata.Annotations = restart;

    var expected = JsonSerializer.SerializeToDocument(daemonSet);

    var patch = old.CreatePatch(expected);
    await client.PatchNamespacedDaemonSetAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace);
}

async Task RestartDeploymentAsync(string name, string @namespace, IKubernetes client)
{
    var deployment = await client.ReadNamespacedDeploymentAsync(name, @namespace);
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    var old = JsonSerializer.SerializeToDocument(deployment, options);

    var restart = new Dictionary<string, string>(deployment.Spec.Template.Metadata.Annotations)
    {
        ["date"] = ConvertToUnixTimestamp(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture)
    };

    deployment.Spec.Template.Metadata.Annotations = restart;

    var expected = JsonSerializer.SerializeToDocument(deployment);

    var patch = old.CreatePatch(expected);
    await client.PatchNamespacedDeploymentAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace);
}

async Task RestartStatefulSetAsync(string name, string @namespace, IKubernetes client)
{
    var deployment = await client.ReadNamespacedStatefulSetAsync(name, @namespace);
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    var old = JsonSerializer.SerializeToDocument(deployment, options);

    var restart = new Dictionary<string, string>(deployment.Spec.Template.Metadata.Annotations)
    {
        ["date"] = ConvertToUnixTimestamp(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture)
    };

    deployment.Spec.Template.Metadata.Annotations = restart;

    var expected = JsonSerializer.SerializeToDocument(deployment);

    var patch = old.CreatePatch(expected);
    await client.PatchNamespacedStatefulSetAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace);
}

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
IKubernetes client = new Kubernetes(config);

await RestartDeploymentAsync("event-exporter", "monitoring", client);
await RestartDaemonSetAsync("prometheus-exporter", "monitoring", client);
await RestartStatefulSetAsync("argocd-application-controlle", "argocd", client);
