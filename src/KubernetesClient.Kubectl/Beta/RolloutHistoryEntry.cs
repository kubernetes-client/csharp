namespace k8s.kubectl.beta;

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
