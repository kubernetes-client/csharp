namespace k8s.LeaderElection
{
    /// <summary>
    /// LeaderElectionRecord is the record that is stored in the leader election annotation.
    /// This information should be used for observational purposes only and could be replaced with a random string (e.g. UUID) with only slight modification of this code.
    /// </summary>
    public class LeaderElectionRecord
    {
        /// <summary>
        /// the ID that owns the lease. If empty, no one owns this lease and all callers may acquire.
        /// </summary>
        public string HolderIdentity { get; set; }

        /// <summary>
        /// LeaseDuration in seconds
        /// </summary>
        public int LeaseDurationSeconds { get; set; }

        /// <summary>
        /// acquire time
        /// </summary>
        // public DateTimeOffset? AcquireTime { get; set; }
        public DateTime? AcquireTime { get; set; }

        /// <summary>
        /// renew time
        /// </summary>
        // public DateTimeOffset? RenewTime { get; set; }
        public DateTime? RenewTime { get; set; }

        /// <summary>
        /// leader transitions
        /// </summary>
        public int LeaderTransitions { get; set; }

        protected bool Equals(LeaderElectionRecord other)
        {
            return HolderIdentity == other?.HolderIdentity && Nullable.Equals(AcquireTime, other.AcquireTime) && Nullable.Equals(RenewTime, other.RenewTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((LeaderElectionRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (HolderIdentity != null ? HolderIdentity.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ AcquireTime.GetHashCode();
                hashCode = (hashCode * 397) ^ RenewTime.GetHashCode();
                return hashCode;
            }
        }
    }
}
