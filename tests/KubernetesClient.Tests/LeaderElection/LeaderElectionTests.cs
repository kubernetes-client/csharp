using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s.LeaderElection;
using Xunit;

namespace k8s.Tests.LeaderElection
{
    public class LeaderElectionTests
    {
        [Fact]
        public void SimpleLeaderElection()
        {
            var electionHistory = new List<string>();
            var leadershipHistory = new List<string>();

            var renewCount = 3;
            var mockLock = new MockResourceLock("mock")
            {
                UpdateWillFail = () => renewCount <= 0,
            };

            mockLock.OnCreate += _ =>
            {
                renewCount--;
                electionHistory.Add("create record");
                leadershipHistory.Add("get leadership");
            };

            mockLock.OnUpdate += _ =>
            {
                renewCount--;
                electionHistory.Add("update record");
            };

            mockLock.OnChange += _ =>
            {
                electionHistory.Add("change record");
            };


            var leaderElectionConfig = new LeaderElectionConfig(mockLock)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(1000),
                RetryPeriod = TimeSpan.FromMilliseconds(500),
                RenewDeadline = TimeSpan.FromMilliseconds(600),
            };

            var countdown = new CountdownEvent(2);
            Task.Run(() =>
            {
                var leaderElector = new LeaderElector(leaderElectionConfig);

                leaderElector.OnStartedLeading += () =>
                {
                    leadershipHistory.Add("start leading");
                    countdown.Signal();
                };

                leaderElector.OnStoppedLeading += () =>
                {
                    leadershipHistory.Add("stop leading");
                    countdown.Signal();
                };

                leaderElector.RunAsync().Wait();
            });

            countdown.Wait(TimeSpan.FromSeconds(10));


            Assert.True(electionHistory.SequenceEqual(new[] { "create record", "update record", "update record" }));
            Assert.True(leadershipHistory.SequenceEqual(new[] { "get leadership", "start leading", "stop leading" }));
        }

        private class MockResourceLock : ILock
        {
            private readonly string id;
            private LeaderElectionRecord leaderRecord;
            private readonly object lockobj = new object();

            public event Action<LeaderElectionRecord> OnCreate;
            public event Action<LeaderElectionRecord> OnUpdate;
            public event Action<LeaderElectionRecord> OnChange;
            public event Action<LeaderElectionRecord> OnTryUpdate;

            public MockResourceLock(string id)
            {
                this.id = id;
            }

            public Func<bool> UpdateWillFail { get; set; }

            public Task<LeaderElectionRecord> GetAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult(leaderRecord);
            }

            public Task<bool> CreateAsync(
                LeaderElectionRecord record,
                CancellationToken cancellationToken = default)
            {
                lock (lockobj)
                {
                    if (leaderRecord != null)
                    {
                        return Task.FromResult(false);
                    }

                    leaderRecord = record;
                    OnCreate?.Invoke(record);
                    return Task.FromResult(true);
                }
            }

            public Task<bool> UpdateAsync(LeaderElectionRecord record, CancellationToken cancellationToken = default)
            {
                lock (lockobj)
                {
                    OnTryUpdate?.Invoke(record);

                    if (UpdateWillFail?.Invoke() == true)
                    {
                        return Task.FromResult(false);
                    }

                    var oldRecord = leaderRecord;
                    leaderRecord = record;
                    OnUpdate?.Invoke(record);
                    if (oldRecord?.HolderIdentity != record.HolderIdentity)
                    {
                        OnChange?.Invoke(record);
                    }

                    return Task.FromResult(true);
                }
            }

            public string Identity => id;

            public string Describe() => id;
        }
    }
}
