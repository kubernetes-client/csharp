using System;

namespace k8s.LeaderElection
{
    public class LeaderElectionConfig
    {
        public ILock Lock { get; set; }

        public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromMilliseconds(15);

        public TimeSpan RenewDeadline { get; set; } = TimeSpan.FromMilliseconds(10);

        public TimeSpan RetryPeriod { get; set; } = TimeSpan.FromMilliseconds(2);

        public LeaderElectionConfig(ILock @lock)
        {
            Lock = @lock;
        }
    }
}
