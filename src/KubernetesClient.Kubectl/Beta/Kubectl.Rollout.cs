namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Restart a workload resource by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <typeparam name="T">The type of workload resource (V1Deployment, V1DaemonSet, or V1StatefulSet).</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource.</param>
    public void RolloutRestart<T>(string name, string @namespace)
        where T : IKubernetesObject
    {
        client.RolloutRestartAsync<T>(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout status of a workload resource.
    /// </summary>
    /// <typeparam name="T">The type of workload resource (V1Deployment, V1DaemonSet, or V1StatefulSet).</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource.</param>
    /// <returns>A string describing the rollout status.</returns>
    public string RolloutStatus<T>(string name, string @namespace)
        where T : IKubernetesObject
    {
        return client.RolloutStatusAsync<T>(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Pause a Deployment rollout.
    /// </summary>
    /// <typeparam name="T">The type of resource (must be V1Deployment).</typeparam>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    public void RolloutPause<T>(string name, string @namespace)
        where T : IKubernetesObject
    {
        client.RolloutPauseAsync<T>(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Resume a paused Deployment rollout.
    /// </summary>
    /// <typeparam name="T">The type of resource (must be V1Deployment).</typeparam>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    public void RolloutResume<T>(string name, string @namespace)
        where T : IKubernetesObject
    {
        client.RolloutResumeAsync<T>(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Undo a Deployment rollout to a previous revision.
    /// </summary>
    /// <typeparam name="T">The type of resource (must be V1Deployment).</typeparam>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="toRevision">The revision to roll back to. If 0 or not specified, rolls back to the previous revision.</param>
    public void RolloutUndo<T>(string name, string @namespace, long? toRevision = null)
        where T : IKubernetesObject
    {
        client.RolloutUndoAsync<T>(name, @namespace, toRevision).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout history of a workload resource.
    /// </summary>
    /// <typeparam name="T">The type of workload resource (V1Deployment, V1DaemonSet, or V1StatefulSet).</typeparam>
    /// <param name="name">The name of the resource.</param>
    /// <param name="namespace">The namespace of the resource.</param>
    /// <returns>A list of revision history entries.</returns>
    public IList<RolloutHistoryEntry> RolloutHistory<T>(string name, string @namespace)
        where T : IKubernetesObject
    {
        return client.RolloutHistoryAsync<T>(name, @namespace).GetAwaiter().GetResult();
    }
}
