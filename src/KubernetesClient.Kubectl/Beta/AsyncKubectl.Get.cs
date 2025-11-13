using k8s.Models;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    /// <summary>
    /// Get a pod by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the pod.</param>
    /// <param name="namespace">The namespace of the pod. Defaults to "default".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pod.</returns>
    public async Task<V1Pod> GetPodAsync(string name, string @namespace = "default", CancellationToken cancellationToken = default)
    {
        return await client.CoreV1.ReadNamespacedPodAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a deployment by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the deployment.</param>
    /// <param name="namespace">The namespace of the deployment. Defaults to "default".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deployment.</returns>
    public async Task<V1Deployment> GetDeploymentAsync(string name, string @namespace = "default", CancellationToken cancellationToken = default)
    {
        return await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a service by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <param name="namespace">The namespace of the service. Defaults to "default".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The service.</returns>
    public async Task<V1Service> GetServiceAsync(string name, string @namespace = "default", CancellationToken cancellationToken = default)
    {
        return await client.CoreV1.ReadNamespacedServiceAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a namespace by name.
    /// </summary>
    /// <param name="name">The name of the namespace.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The namespace.</returns>
    public async Task<V1Namespace> GetNamespaceAsync(string name, CancellationToken cancellationToken = default)
    {
        return await client.CoreV1.ReadNamespaceAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a node by name.
    /// </summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The node.</returns>
    public async Task<V1Node> GetNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        return await client.CoreV1.ReadNodeAsync(name, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a config map by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the config map.</param>
    /// <param name="namespace">The namespace of the config map. Defaults to "default".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The config map.</returns>
    public async Task<V1ConfigMap> GetConfigMapAsync(string name, string @namespace = "default", CancellationToken cancellationToken = default)
    {
        return await client.CoreV1.ReadNamespacedConfigMapAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get a secret by name in a namespace.
    /// </summary>
    /// <param name="name">The name of the secret.</param>
    /// <param name="namespace">The namespace of the secret. Defaults to "default".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The secret.</returns>
    public async Task<V1Secret> GetSecretAsync(string name, string @namespace = "default", CancellationToken cancellationToken = default)
    {
        return await client.CoreV1.ReadNamespacedSecretAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
