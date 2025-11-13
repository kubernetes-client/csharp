using k8s.Models;

namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Get a pod by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the pod.</param>
    /// <param name="namespace">The namespace of the pod. Defaults to "default".</param>
    /// <returns>The pod.</returns>
    public V1Pod GetPod(string name, string @namespace = "default")
    {
        return client.GetPodAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get a deployment by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the deployment.</param>
    /// <param name="namespace">The namespace of the deployment. Defaults to "default".</param>
    /// <returns>The deployment.</returns>
    public V1Deployment GetDeployment(string name, string @namespace = "default")
    {
        return client.GetDeploymentAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get a service by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <param name="namespace">The namespace of the service. Defaults to "default".</param>
    /// <returns>The service.</returns>
    public V1Service GetService(string name, string @namespace = "default")
    {
        return client.GetServiceAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get a namespace by name.
    /// </summary>
    /// <param name="name">The name of the namespace.</param>
    /// <returns>The namespace.</returns>
    public V1Namespace GetNamespace(string name)
    {
        return client.GetNamespaceAsync(name).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get a node by name.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <returns>The node.</returns>
    public V1Node GetNode(string name)
    {
        return client.GetNodeAsync(name).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get a config map by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the config map.</param>
    /// <param name="namespace">The namespace of the config map. Defaults to "default".</param>
    /// <returns>The config map.</returns>
    public V1ConfigMap GetConfigMap(string name, string @namespace = "default")
    {
        return client.GetConfigMapAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get a secret by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the secret.</param>
    /// <param name="namespace">The namespace of the secret. Defaults to "default".</param>
    /// <returns>The secret.</returns>
    public V1Secret GetSecret(string name, string @namespace = "default")
    {
        return client.GetSecretAsync(name, @namespace).GetAwaiter().GetResult();
    }
}
