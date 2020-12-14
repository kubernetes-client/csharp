using System;

namespace k8s.LeaderElection
{
    public class LeaderElectionConfig
    {
        public ILock Lock { get; set; }

        public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromSeconds(15);

        public TimeSpan RenewDeadline { get; set; } = TimeSpan.FromSeconds(10);

        public TimeSpan RetryPeriod { get; set; } = TimeSpan.FromSeconds(2);

        public LeaderElectionConfig(ILock @lock)
        {
            Lock = @lock;
        }
    }
}
