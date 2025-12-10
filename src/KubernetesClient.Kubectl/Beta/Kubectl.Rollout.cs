namespace k8s.kubectl.beta;

public partial class Kubectl
{
    /// <summary>
    /// Restart a Deployment by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    public void RolloutRestartDeployment(string name, string @namespace)
    {
        client.RolloutRestartDeploymentAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Restart a DaemonSet by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <param name="name">The name of the DaemonSet.</param>
    /// <param name="namespace">The namespace of the DaemonSet.</param>
    public void RolloutRestartDaemonSet(string name, string @namespace)
    {
        client.RolloutRestartDaemonSetAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Restart a StatefulSet by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <param name="name">The name of the StatefulSet.</param>
    /// <param name="namespace">The namespace of the StatefulSet.</param>
    public void RolloutRestartStatefulSet(string name, string @namespace)
    {
        client.RolloutRestartStatefulSetAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout status of a Deployment.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <returns>A string describing the rollout status.</returns>
    public string RolloutStatusDeployment(string name, string @namespace)
    {
        return client.RolloutStatusDeploymentAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout status of a DaemonSet.
    /// </summary>
    /// <param name="name">The name of the DaemonSet.</param>
    /// <param name="namespace">The namespace of the DaemonSet.</param>
    /// <returns>A string describing the rollout status.</returns>
    public string RolloutStatusDaemonSet(string name, string @namespace)
    {
        return client.RolloutStatusDaemonSetAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout status of a StatefulSet.
    /// </summary>
    /// <param name="name">The name of the StatefulSet.</param>
    /// <param name="namespace">The namespace of the StatefulSet.</param>
    /// <returns>A string describing the rollout status.</returns>
    public string RolloutStatusStatefulSet(string name, string @namespace)
    {
        return client.RolloutStatusStatefulSetAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Pause a Deployment rollout.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    public void RolloutPauseDeployment(string name, string @namespace)
    {
        client.RolloutPauseDeploymentAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Resume a paused Deployment rollout.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    public void RolloutResumeDeployment(string name, string @namespace)
    {
        client.RolloutResumeDeploymentAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Undo a Deployment rollout to a previous revision.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="toRevision">The revision to roll back to. If 0 or not specified, rolls back to the previous revision.</param>
    public void RolloutUndoDeployment(string name, string @namespace, long? toRevision = null)
    {
        client.RolloutUndoDeploymentAsync(name, @namespace, toRevision).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout history of a Deployment.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <returns>A list of revision history entries.</returns>
    public IList<RolloutHistoryEntry> RolloutHistoryDeployment(string name, string @namespace)
    {
        return client.RolloutHistoryDeploymentAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout history of a DaemonSet.
    /// </summary>
    /// <param name="name">The name of the DaemonSet.</param>
    /// <param name="namespace">The namespace of the DaemonSet.</param>
    /// <returns>A list of revision history entries.</returns>
    public IList<RolloutHistoryEntry> RolloutHistoryDaemonSet(string name, string @namespace)
    {
        return client.RolloutHistoryDaemonSetAsync(name, @namespace).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the rollout history of a StatefulSet.
    /// </summary>
    /// <param name="name">The name of the StatefulSet.</param>
    /// <param name="namespace">The namespace of the StatefulSet.</param>
    /// <returns>A list of revision history entries.</returns>
    public IList<RolloutHistoryEntry> RolloutHistoryStatefulSet(string name, string @namespace)
    {
        return client.RolloutHistoryStatefulSetAsync(name, @namespace).GetAwaiter().GetResult();
    }
}
