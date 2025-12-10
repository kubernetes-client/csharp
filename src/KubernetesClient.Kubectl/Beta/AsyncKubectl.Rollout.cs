using Json.Patch;
using k8s.Models;
using System.Text.Json;

namespace k8s.kubectl.beta;

public partial class AsyncKubectl
{
    private const string RestartedAtAnnotation = "kubectl.kubernetes.io/restartedAt";
    private const string RevisionAnnotation = "deployment.kubernetes.io/revision";
    private const string ChangeCauseAnnotation = "kubernetes.io/change-cause";

    private static string BuildLabelSelector(IDictionary<string, string> matchLabels)
    {
        return string.Join(",", matchLabels.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    /// <summary>
    /// Restart a Deployment by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RolloutRestartDeploymentAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var old = JsonSerializer.SerializeToDocument(deployment);

        // Add or update the restart annotation to trigger a rollout
        deployment.Spec.Template.Metadata ??= new V1ObjectMeta();
        deployment.Spec.Template.Metadata.Annotations ??= new Dictionary<string, string>();
        deployment.Spec.Template.Metadata.Annotations[RestartedAtAnnotation] = DateTime.UtcNow.ToString("o");

        var patch = old.CreatePatch(deployment);

        await client.AppsV1.PatchNamespacedDeploymentAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Restart a DaemonSet by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <param name="name">The name of the DaemonSet.</param>
    /// <param name="namespace">The namespace of the DaemonSet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RolloutRestartDaemonSetAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var daemonSet = await client.AppsV1.ReadNamespacedDaemonSetAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var old = JsonSerializer.SerializeToDocument(daemonSet);

        // Add or update the restart annotation to trigger a rollout
        daemonSet.Spec.Template.Metadata ??= new V1ObjectMeta();
        daemonSet.Spec.Template.Metadata.Annotations ??= new Dictionary<string, string>();
        daemonSet.Spec.Template.Metadata.Annotations[RestartedAtAnnotation] = DateTime.UtcNow.ToString("o");

        var patch = old.CreatePatch(daemonSet);

        await client.AppsV1.PatchNamespacedDaemonSetAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Restart a StatefulSet by adding a restart annotation to trigger a rollout.
    /// </summary>
    /// <param name="name">The name of the StatefulSet.</param>
    /// <param name="namespace">The namespace of the StatefulSet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RolloutRestartStatefulSetAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var statefulSet = await client.AppsV1.ReadNamespacedStatefulSetAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var old = JsonSerializer.SerializeToDocument(statefulSet);

        // Add or update the restart annotation to trigger a rollout
        statefulSet.Spec.Template.Metadata ??= new V1ObjectMeta();
        statefulSet.Spec.Template.Metadata.Annotations ??= new Dictionary<string, string>();
        statefulSet.Spec.Template.Metadata.Annotations[RestartedAtAnnotation] = DateTime.UtcNow.ToString("o");

        var patch = old.CreatePatch(statefulSet);

        await client.AppsV1.PatchNamespacedStatefulSetAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the rollout status of a Deployment.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A string describing the rollout status.</returns>
    public async Task<string> RolloutStatusDeploymentAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var status = deployment.Status;
        var spec = deployment.Spec;

        if (status == null)
        {
            return "Waiting for deployment spec update to be observed...";
        }

        if (status.ObservedGeneration < deployment.Metadata.Generation)
        {
            return "Waiting for deployment spec update to be observed...";
        }

        if (status.UpdatedReplicas < spec.Replicas)
        {
            return $"Waiting for deployment \"{name}\" rollout to finish: {status.UpdatedReplicas ?? 0} out of {spec.Replicas ?? 0} new replicas have been updated...";
        }

        if (status.Replicas > status.UpdatedReplicas)
        {
            return $"Waiting for deployment \"{name}\" rollout to finish: {status.Replicas - status.UpdatedReplicas} old replicas are pending termination...";
        }

        if (status.AvailableReplicas < status.UpdatedReplicas)
        {
            return $"Waiting for deployment \"{name}\" rollout to finish: {status.AvailableReplicas ?? 0} of {status.UpdatedReplicas ?? 0} updated replicas are available...";
        }

        return $"deployment \"{name}\" successfully rolled out";
    }

    /// <summary>
    /// Get the rollout status of a DaemonSet.
    /// </summary>
    /// <param name="name">The name of the DaemonSet.</param>
    /// <param name="namespace">The namespace of the DaemonSet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A string describing the rollout status.</returns>
    public async Task<string> RolloutStatusDaemonSetAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var daemonSet = await client.AppsV1.ReadNamespacedDaemonSetAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var status = daemonSet.Status;

        if (status == null)
        {
            return "Waiting for daemon set spec update to be observed...";
        }

        if (status.ObservedGeneration < daemonSet.Metadata.Generation)
        {
            return "Waiting for daemon set spec update to be observed...";
        }

        if (status.UpdatedNumberScheduled < status.DesiredNumberScheduled)
        {
            return $"Waiting for daemon set \"{name}\" rollout to finish: {status.UpdatedNumberScheduled} out of {status.DesiredNumberScheduled} new pods have been updated...";
        }

        if (status.NumberAvailable < status.DesiredNumberScheduled)
        {
            return $"Waiting for daemon set \"{name}\" rollout to finish: {status.NumberAvailable ?? 0} of {status.DesiredNumberScheduled} updated pods are available...";
        }

        return $"daemon set \"{name}\" successfully rolled out";
    }

    /// <summary>
    /// Get the rollout status of a StatefulSet.
    /// </summary>
    /// <param name="name">The name of the StatefulSet.</param>
    /// <param name="namespace">The namespace of the StatefulSet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A string describing the rollout status.</returns>
    public async Task<string> RolloutStatusStatefulSetAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var statefulSet = await client.AppsV1.ReadNamespacedStatefulSetAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var status = statefulSet.Status;
        var spec = statefulSet.Spec;

        if (status == null)
        {
            return "Waiting for statefulset spec update to be observed...";
        }

        if (status.ObservedGeneration < statefulSet.Metadata.Generation)
        {
            return "Waiting for statefulset spec update to be observed...";
        }

        if (spec.Replicas != null && status.ReadyReplicas < spec.Replicas)
        {
            return $"Waiting for {spec.Replicas - status.ReadyReplicas} pods to be ready...";
        }

        if (spec.UpdateStrategy?.Type == "RollingUpdate" && spec.UpdateStrategy.RollingUpdate != null)
        {
            if (spec.Replicas != null && spec.UpdateStrategy.RollingUpdate.Partition != null)
            {
                if (status.UpdatedReplicas < (spec.Replicas - spec.UpdateStrategy.RollingUpdate.Partition))
                {
                    return $"Waiting for partitioned roll out to finish: {status.UpdatedReplicas} out of {spec.Replicas - spec.UpdateStrategy.RollingUpdate.Partition} new pods have been updated...";
                }
            }
        }

        if (status.UpdateRevision != status.CurrentRevision)
        {
            return $"waiting for statefulset rolling update to complete {status.UpdatedReplicas} pods at revision {status.UpdateRevision}...";
        }

        return $"statefulset rolling update complete {status.CurrentReplicas} pods at revision {status.CurrentRevision}...";
    }

    /// <summary>
    /// Pause a Deployment rollout.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RolloutPauseDeploymentAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var old = JsonSerializer.SerializeToDocument(deployment);

        deployment.Spec.Paused = true;

        var patch = old.CreatePatch(deployment);

        await client.AppsV1.PatchNamespacedDeploymentAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resume a paused Deployment rollout.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RolloutResumeDeploymentAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        var old = JsonSerializer.SerializeToDocument(deployment);

        deployment.Spec.Paused = false;

        var patch = old.CreatePatch(deployment);

        await client.AppsV1.PatchNamespacedDeploymentAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Undo a Deployment rollout to a previous revision.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="toRevision">The revision to roll back to. If 0 or not specified, rolls back to the previous revision.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RolloutUndoDeploymentAsync(string name, string @namespace, long? toRevision = null, CancellationToken cancellationToken = default)
    {
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get all ReplicaSets for this deployment
        var labelSelector = BuildLabelSelector(deployment.Spec.Selector.MatchLabels);
        var replicaSets = await client.AppsV1.ListNamespacedReplicaSetAsync(@namespace, labelSelector: labelSelector, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Filter ReplicaSets owned by this deployment
        var ownedReplicaSets = replicaSets.Items
            .Where(rs => rs.Metadata.OwnerReferences?.Any(or => or.Uid == deployment.Metadata.Uid) == true)
            .OrderByDescending(rs =>
            {
                if (rs.Metadata.Annotations?.TryGetValue(RevisionAnnotation, out var revisionStr) == true)
                {
                    return long.TryParse(revisionStr, out var revision) ? revision : 0;
                }

                return 0;
            })
            .ToList();

        if (ownedReplicaSets.Count == 0)
        {
            throw new InvalidOperationException($"No ReplicaSets found for deployment {name}");
        }

        V1ReplicaSet? targetReplicaSet;

        if (toRevision.HasValue && toRevision.Value > 0)
        {
            // Find specific revision
            targetReplicaSet = ownedReplicaSets.FirstOrDefault(rs =>
            {
                if (rs.Metadata.Annotations?.TryGetValue(RevisionAnnotation, out var revisionStr) == true)
                {
                    return long.TryParse(revisionStr, out var revision) && revision == toRevision.Value;
                }

                return false;
            });

            if (targetReplicaSet == null)
            {
                throw new InvalidOperationException($"Revision {toRevision} not found for deployment {name}");
            }
        }
        else
        {
            // Use previous revision (second in the list)
            if (ownedReplicaSets.Count < 2)
            {
                throw new InvalidOperationException($"No previous revision found for deployment {name}");
            }

            targetReplicaSet = ownedReplicaSets[1];
        }

        // Update deployment with the template from the target ReplicaSet
        var old = JsonSerializer.SerializeToDocument(deployment);

        deployment.Spec.Template = targetReplicaSet.Spec.Template;

        // Add annotation to record the rollback
        deployment.Metadata.Annotations ??= new Dictionary<string, string>();
        deployment.Metadata.Annotations[RevisionAnnotation] =
            targetReplicaSet.Metadata.Annotations?[RevisionAnnotation] ?? "0";

        var patch = old.CreatePatch(deployment);

        await client.AppsV1.PatchNamespacedDeploymentAsync(new V1Patch(patch, V1Patch.PatchType.JsonPatch), name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the rollout history of a Deployment.
    /// </summary>
    /// <param name="name">The name of the Deployment.</param>
    /// <param name="namespace">The namespace of the Deployment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of revision history entries.</returns>
    public async Task<IList<RolloutHistoryEntry>> RolloutHistoryDeploymentAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get all ReplicaSets for this deployment
        var labelSelector = BuildLabelSelector(deployment.Spec.Selector.MatchLabels);
        var replicaSets = await client.AppsV1.ListNamespacedReplicaSetAsync(@namespace, labelSelector: labelSelector, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Filter and process ReplicaSets owned by this deployment
        var history = replicaSets.Items
            .Where(rs => rs.Metadata.OwnerReferences?.Any(or => or.Uid == deployment.Metadata.Uid) == true)
            .Select(rs =>
            {
                var revision = 0L;
                if (rs.Metadata.Annotations?.TryGetValue(RevisionAnnotation, out var revisionStr) == true)
                {
                    long.TryParse(revisionStr, out revision);
                }

                var changeCause = "<none>";
                if (rs.Metadata.Annotations?.TryGetValue(ChangeCauseAnnotation, out var cause) == true && !string.IsNullOrEmpty(cause))
                {
                    changeCause = cause;
                }

                return new RolloutHistoryEntry
                {
                    Revision = revision,
                    ChangeCause = changeCause,
                };
            })
            .Where(entry => entry.Revision > 0)
            .OrderBy(entry => entry.Revision)
            .ToList();

        return history;
    }

    /// <summary>
    /// Get the rollout history of a DaemonSet.
    /// </summary>
    /// <param name="name">The name of the DaemonSet.</param>
    /// <param name="namespace">The namespace of the DaemonSet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of revision history entries.</returns>
    public async Task<IList<RolloutHistoryEntry>> RolloutHistoryDaemonSetAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var daemonSet = await client.AppsV1.ReadNamespacedDaemonSetAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get ControllerRevisions for this DaemonSet
        var labelSelector = BuildLabelSelector(daemonSet.Spec.Selector.MatchLabels);
        var controllerRevisions = await client.AppsV1.ListNamespacedControllerRevisionAsync(@namespace, labelSelector: labelSelector, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Filter and process ControllerRevisions owned by this DaemonSet
        var history = controllerRevisions.Items
            .Where(cr => cr.Metadata.OwnerReferences?.Any(or => or.Uid == daemonSet.Metadata.Uid) == true)
            .Select(cr =>
            {
                var changeCause = "<none>";
                if (cr.Metadata.Annotations?.TryGetValue(ChangeCauseAnnotation, out var cause) == true && !string.IsNullOrEmpty(cause))
                {
                    changeCause = cause;
                }

                return new RolloutHistoryEntry
                {
                    Revision = cr.Revision,
                    ChangeCause = changeCause,
                };
            })
            .OrderBy(entry => entry.Revision)
            .ToList();

        return history;
    }

    /// <summary>
    /// Get the rollout history of a StatefulSet.
    /// </summary>
    /// <param name="name">The name of the StatefulSet.</param>
    /// <param name="namespace">The namespace of the StatefulSet.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of revision history entries.</returns>
    public async Task<IList<RolloutHistoryEntry>> RolloutHistoryStatefulSetAsync(string name, string @namespace, CancellationToken cancellationToken = default)
    {
        var statefulSet = await client.AppsV1.ReadNamespacedStatefulSetAsync(name, @namespace, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get ControllerRevisions for this StatefulSet
        var labelSelector = BuildLabelSelector(statefulSet.Spec.Selector.MatchLabels);
        var controllerRevisions = await client.AppsV1.ListNamespacedControllerRevisionAsync(@namespace, labelSelector: labelSelector, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Filter and process ControllerRevisions owned by this StatefulSet
        var history = controllerRevisions.Items
            .Where(cr => cr.Metadata.OwnerReferences?.Any(or => or.Uid == statefulSet.Metadata.Uid) == true)
            .Select(cr =>
            {
                var changeCause = "<none>";
                if (cr.Metadata.Annotations?.TryGetValue(ChangeCauseAnnotation, out var cause) == true && !string.IsNullOrEmpty(cause))
                {
                    changeCause = cause;
                }

                return new RolloutHistoryEntry
                {
                    Revision = cr.Revision,
                    ChangeCause = changeCause,
                };
            })
            .OrderBy(entry => entry.Revision)
            .ToList();

        return history;
    }
}

/// <summary>
/// Represents a single entry in the rollout history.
/// </summary>
public class RolloutHistoryEntry
{
    /// <summary>
    /// The revision number.
    /// </summary>
    public long Revision { get; set; }

    /// <summary>
    /// The change cause annotation for this revision.
    /// </summary>
    public string ChangeCause { get; set; } = "<none>";
}
