using System.Threading;
using System.Threading.Tasks;

namespace k8s.LeaderElection
{
    /// <summary>
    /// ILock offers a common interface for locking on arbitrary resources used in leader election. The Interface is used to hide the details on specific implementations in order to allow them to change over time.
    /// </summary>
    public interface ILock
    {
        /// <summary>
        /// Get returns the LeaderElectionRecord
        /// </summary>
        /// <param name="cancellationToken">token to cancel the task</param>
        /// <returns>the record</returns>
        Task<LeaderElectionRecord> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Create attempts to create a LeaderElectionRecord
        /// </summary>
        /// <param name="record">record to create</param>
        /// <param name="cancellationToken">token to cancel the task</param>
        /// <returns>true if created</returns>
        Task<bool> CreateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update will update and existing LeaderElectionRecord
        /// </summary>
        /// <param name="record">record to create</param>
        /// <param name="cancellationToken">token to cancel the task</param>
        /// <returns>true if updated</returns>
        Task<bool> UpdateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default);


        /// <summary>
        /// the locks Identity
        /// </summary>
        string Identity { get; }

        /// <summary>
        /// Describe is used to convert details on current resource lock into a string
        /// </summary>
        /// <returns>resource lock description</returns>
        string Describe();
    }
}
