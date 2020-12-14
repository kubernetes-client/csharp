using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace k8s.LeaderElection
{
    public class LeaderElector : IDisposable
    {
        private const double JitterFactor = 1.2;

        private readonly LeaderElectionConfig config;

        /// <summary>
        /// OnStartedLeading is called when a LeaderElector client starts leading
        /// </summary>
        public event Action OnStartedLeading;

        /// <summary>
        /// OnStoppedLeading is called when a LeaderElector client stops leading
        /// </summary>
        public event Action OnStoppedLeading;

        /// <summary>
        /// OnNewLeader is called when the client observes a leader that is
        /// not the previously observed leader. This includes the first observed
        /// leader when the client starts.
        /// </summary>
        public event Action<string> OnNewLeader;

        private volatile LeaderElectionRecord observedRecord;
        private DateTimeOffset observedTime = DateTimeOffset.MinValue;
        private string reportedLeader;

        public LeaderElector(LeaderElectionConfig config)
        {
            this.config = config;
        }

        public bool IsLeader()
        {
            return observedRecord?.HolderIdentity != null && observedRecord?.HolderIdentity == config.Lock.Identity;
        }

        public string GetLeader()
        {
            return observedRecord?.HolderIdentity;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await AcquireAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                OnStartedLeading?.Invoke();

                // renew loop
                for (; ; )
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var acq = Task.Run(async () =>
                    {
                        try
                        {
                            while (!await TryAcquireOrRenew(cancellationToken).ConfigureAwait(false))
                            {
                                await Task.Delay(config.RetryPeriod, cancellationToken).ConfigureAwait(false);
                                MaybeReportTransition();
                            }
                        }
                        catch
                        {
                            // ignore
                            return false;
                        }

                        return true;
                    });


                    if (await Task.WhenAny(acq, Task.Delay(config.RenewDeadline, cancellationToken))
                        .ConfigureAwait(false) == acq)
                    {
                        var succ = await acq.ConfigureAwait(false);

                        if (succ)
                        {
                            await Task.Delay(config.RetryPeriod, cancellationToken).ConfigureAwait(false);
                            // retry
                            continue;
                        }

                        // renew failed
                    }

                    // timeout
                    break;
                }
            }
            finally
            {
                OnStoppedLeading?.Invoke();
            }
        }

        private async Task<bool> TryAcquireOrRenew(CancellationToken cancellationToken)
        {
            var l = config.Lock;
            var leaderElectionRecord = new LeaderElectionRecord()
            {
                HolderIdentity = l.Identity,
                LeaseDurationSeconds = config.LeaseDuration.Seconds,
                AcquireTime = DateTime.UtcNow,
                RenewTime = DateTime.UtcNow,
                LeaderTransitions = 0,
            };

            // 1. obtain or create the ElectionRecord

            LeaderElectionRecord oldLeaderElectionRecord = null;
            try
            {
                oldLeaderElectionRecord = await l.GetAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (HttpOperationException e)
            {
                if (e.Response.StatusCode != HttpStatusCode.NotFound)
                {
                    return false;
                }
            }

            if (oldLeaderElectionRecord?.AcquireTime == null ||
                oldLeaderElectionRecord?.RenewTime == null ||
                oldLeaderElectionRecord?.HolderIdentity == null)
            {
                var created = await l.CreateAsync(leaderElectionRecord, cancellationToken).ConfigureAwait(false);
                if (created)
                {
                    observedRecord = leaderElectionRecord;
                    observedTime = DateTimeOffset.Now;
                    return true;
                }

                return false;
            }


            // 2. Record obtained, check the Identity & Time
            if (!Equals(observedRecord, oldLeaderElectionRecord))
            {
                observedRecord = oldLeaderElectionRecord;
                observedTime = DateTimeOffset.Now;
            }

            if (!string.IsNullOrEmpty(oldLeaderElectionRecord.HolderIdentity)
                && observedTime + config.LeaseDuration > DateTimeOffset.Now
                && !IsLeader())
            {
                // lock is held by %v and has not yet expired", oldLeaderElectionRecord.HolderIdentity
                return false;
            }

            // 3. We're going to try to update. The leaderElectionRecord is set to it's default
            // here. Let's correct it before updating.
            if (IsLeader())
            {
                leaderElectionRecord.AcquireTime = oldLeaderElectionRecord.AcquireTime;
                leaderElectionRecord.LeaderTransitions = oldLeaderElectionRecord.LeaderTransitions;
            }
            else
            {
                leaderElectionRecord.LeaderTransitions = oldLeaderElectionRecord.LeaderTransitions + 1;
            }

            var updated = await l.UpdateAsync(leaderElectionRecord, cancellationToken).ConfigureAwait(false);
            if (!updated)
            {
                return false;
            }

            observedRecord = leaderElectionRecord;
            observedTime = DateTimeOffset.Now;

            return true;
        }

        private async Task AcquireAsync(CancellationToken cancellationToken)
        {
            for (; ; )
            {
                try
                {
                    var delay = config.RetryPeriod.Milliseconds;
                    var acq = TryAcquireOrRenew(cancellationToken);

                    if (await Task.WhenAny(acq, Task.Delay(delay, cancellationToken))
                        .ConfigureAwait(false) == acq)
                    {
                        if (await acq.ConfigureAwait(false))
                        {
                            return;
                        }
                    }

                    delay = (int)(delay * JitterFactor);
                }
                finally
                {
                    MaybeReportTransition();
                }
            }
        }

        private void MaybeReportTransition()
        {
            if (observedRecord == null)
            {
                return;
            }

            if (observedRecord.HolderIdentity == reportedLeader)
            {
                return;
            }

            reportedLeader = observedRecord.HolderIdentity;

            OnNewLeader?.Invoke(reportedLeader);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
