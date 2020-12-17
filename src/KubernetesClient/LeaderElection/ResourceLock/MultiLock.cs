using System.Threading;
using System.Threading.Tasks;

namespace k8s.LeaderElection.ResourceLock
{
    public class MultiLock : ILock
    {
        private ILock primary;
        private ILock secondary;

        public MultiLock(ILock primary, ILock secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
        }

        public Task<LeaderElectionRecord> GetAsync(CancellationToken cancellationToken = default)
        {
            return primary.GetAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
        {
            return await primary.CreateAsync(record, cancellationToken).ConfigureAwait(false)
                   && await secondary.CreateAsync(record, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
        {
            return await primary.UpdateAsync(record, cancellationToken).ConfigureAwait(false)
                   && await secondary.UpdateAsync(record, cancellationToken).ConfigureAwait(false);
        }

        public string Identity => primary.Identity;

        public string Describe()
        {
            return primary.Describe();
        }
    }
}
