using System.Net;

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

        /// <summary>
        /// OnError is called when there is an error trying to determine leadership.
        /// </summary>
        public event Action<Exception> OnError;

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

        /// <summary>
        /// Tries to acquire and hold leadership once via a Kubernetes Lease resource.
        /// Will complete the returned Task and not retry to acquire leadership again after leadership is lost once.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task RunUntilLeadershipLostAsync(CancellationToken cancellationToken = default)
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
                        catch (Exception e)
                        {
                            OnError?.Invoke(e);
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

        /// <summary>
        /// Tries to acquire leadership via a Kubernetes Lease resource.
        /// Will retry to acquire leadership again after leadership was lost.
        /// </summary>
        /// <returns>A Task which completes only on cancellation</returns>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        public async Task RunAndTryToHoldLeadershipForeverAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await RunUntilLeadershipLostAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tries to acquire leadership once via a Kubernetes Lease resource.
        /// Will complete the returned Task and not retry to acquire leadership again after leadership is lost once.
        /// </summary>
        /// <seealso cref="RunUntilLeadershipLostAsync"/>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [Obsolete("Replaced by RunUntilLeadershipLostAsync to encode behavior in method name.")]
        public Task RunAsync(CancellationToken cancellationToken = default)
        {
            return RunUntilLeadershipLostAsync(cancellationToken);
        }

        private async Task<bool> TryAcquireOrRenew(CancellationToken cancellationToken)
        {
            var l = config.Lock;
            var leaderElectionRecord = new LeaderElectionRecord()
            {
                HolderIdentity = l.Identity,
                LeaseDurationSeconds = (int)config.LeaseDuration.TotalSeconds,
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

                OnError?.Invoke(e);
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
            var delay = (int)config.RetryPeriod.TotalMilliseconds;
            for (; ; )
            {
                try
                {
                    var acq = TryAcquireOrRenew(cancellationToken);

                    if (await Task.WhenAny(acq, Task.Delay((int)(delay * JitterFactor * (new Random().NextDouble() + 1)), cancellationToken))
                        .ConfigureAwait(false) == acq)
                    {
                        if (await acq.ConfigureAwait(false))
                        {
                            return;
                        }

                        // wait RetryPeriod since acq return immediately
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        // else timeout
                        _ = acq.ContinueWith(t => OnError?.Invoke(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                    }
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
