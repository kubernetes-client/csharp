using k8s.LeaderElection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace k8s.Tests.LeaderElection
{
    public class LeaderElectionTests
    {
        private readonly ITestOutputHelper output;

        public LeaderElectionTests(ITestOutputHelper output)
        {
            ThreadPool.SetMaxThreads(32, 32);
            ThreadPool.SetMinThreads(32, 32);
            this.output = output;
            MockResourceLock.ResetGloablRecord();
        }

        [Fact]
        public void SimpleLeaderElection()
        {
            var electionHistory = new List<string>();
            var leadershipHistory = new List<string>();

            var renewCount = 3;
            var mockLock = new MockResourceLock("mock") { UpdateWillFail = () => renewCount <= 0, };

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

            mockLock.OnChange += _ => { electionHistory.Add("change record"); };


            var leaderElectionConfig = new LeaderElectionConfig(mockLock)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(1000),
                RetryPeriod = TimeSpan.FromMilliseconds(500),
                RenewDeadline = TimeSpan.FromMilliseconds(600),
            };

            var countdown = new CountdownEvent(2);
            Task.Run(async () =>
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

                await leaderElector.RunUntilLeadershipLostAsync().ConfigureAwait(true);
            });

            countdown.Wait(TimeSpan.FromSeconds(10));


            Assert.True(electionHistory.SequenceEqual(new[] { "create record", "update record", "update record" }));
            Assert.True(leadershipHistory.SequenceEqual(new[] { "get leadership", "start leading", "stop leading" }));
        }

        [Fact]
        public void LeaderElection()
        {
            var electionHistory = new List<string>();
            var leadershipHistory = new List<string>();
            var electionHistoryCountdown = new CountdownEvent(7);

            var renewCountA = 3;
            var mockLockA = new MockResourceLock("mockA") { UpdateWillFail = () => renewCountA <= 0 };

            mockLockA.OnCreate += (_) =>
            {
                renewCountA--;

                electionHistory.Add("A creates record");
                leadershipHistory.Add("A gets leadership");
                electionHistoryCountdown.Signal();
            };

            mockLockA.OnUpdate += (_) =>
            {
                renewCountA--;
                electionHistory.Add("A updates record");
                electionHistoryCountdown.Signal();
            };

            mockLockA.OnChange += (_) => { leadershipHistory.Add("A gets leadership"); };

            var leaderElectionConfigA = new LeaderElectionConfig(mockLockA)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(500),
                RetryPeriod = TimeSpan.FromMilliseconds(300),
                RenewDeadline = TimeSpan.FromMilliseconds(400),
            };

            var renewCountB = 4;
            var mockLockB = new MockResourceLock("mockB") { UpdateWillFail = () => renewCountB <= 0 };

            mockLockB.OnCreate += (_) =>
            {
                renewCountB--;

                electionHistory.Add("B creates record");
                electionHistoryCountdown.Signal();
                leadershipHistory.Add("B gets leadership");
            };

            mockLockB.OnUpdate += (_) =>
            {
                renewCountB--;
                electionHistory.Add("B updates record");
                electionHistoryCountdown.Signal();
            };

            mockLockB.OnChange += (_) => { leadershipHistory.Add("B gets leadership"); };

            var leaderElectionConfigB = new LeaderElectionConfig(mockLockB)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(500),
                RetryPeriod = TimeSpan.FromMilliseconds(300),
                RenewDeadline = TimeSpan.FromMilliseconds(400),
            };

            var lockAStopLeading = new ManualResetEvent(false);
            var testLeaderElectionLatch = new CountdownEvent(4);

            Task.Run(async () =>
            {
                var leaderElector = new LeaderElector(leaderElectionConfigA);

                leaderElector.OnStartedLeading += () =>
                {
                    leadershipHistory.Add("A starts leading");
                    testLeaderElectionLatch.Signal();
                };

                leaderElector.OnStoppedLeading += () =>
                {
                    leadershipHistory.Add("A stops leading");
                    testLeaderElectionLatch.Signal();
                    lockAStopLeading.Set();
                };

                await leaderElector.RunUntilLeadershipLostAsync().ConfigureAwait(true);
            });


            lockAStopLeading.WaitOne(TimeSpan.FromSeconds(3));

            Task.Run(async () =>
            {
                var leaderElector = new LeaderElector(leaderElectionConfigB);

                leaderElector.OnStartedLeading += () =>
                {
                    leadershipHistory.Add("B starts leading");
                    testLeaderElectionLatch.Signal();
                };

                leaderElector.OnStoppedLeading += () =>
                {
                    leadershipHistory.Add("B stops leading");
                    testLeaderElectionLatch.Signal();
                };

                await leaderElector.RunUntilLeadershipLostAsync().ConfigureAwait(true);
            });

            testLeaderElectionLatch.Wait(TimeSpan.FromSeconds(15));
            electionHistoryCountdown.Wait(TimeSpan.FromSeconds(15));

            Assert.Equal(7, electionHistory.Count);


            Assert.True(electionHistory.SequenceEqual(
                new[]
                {
                    "A creates record", "A updates record", "A updates record",
                    "B updates record", "B updates record", "B updates record", "B updates record",
                }));

            Assert.True(leadershipHistory.SequenceEqual(
                new[]
                {
                    "A gets leadership", "A starts leading", "A stops leading",
                    "B gets leadership", "B starts leading", "B stops leading",
                }));
        }

        [Fact]
        public void LeaderElectionWithRenewDeadline()
        {
            var electionHistory = new List<string>();
            var leadershipHistory = new List<string>();
            var electionHistoryCountdown = new CountdownEvent(9);

            var renewCount = 3;
            var mockLock = new MockResourceLock("mock") { UpdateWillFail = () => renewCount <= 0, };

            mockLock.OnCreate += _ =>
            {
                renewCount--;
                electionHistory.Add("create record");
                leadershipHistory.Add("get leadership");
                electionHistoryCountdown.Signal();
            };

            mockLock.OnUpdate += _ =>
            {
                renewCount--;
                electionHistory.Add("update record");
                electionHistoryCountdown.Signal();
            };

            mockLock.OnChange += _ =>
            {
                electionHistory.Add("change record");
                electionHistoryCountdown.Signal();
            };

            mockLock.OnTryUpdate += _ =>
            {
                electionHistory.Add("try update record");
                electionHistoryCountdown.Signal();
            };


            var leaderElectionConfig = new LeaderElectionConfig(mockLock)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(1000),
                RetryPeriod = TimeSpan.FromMilliseconds(200),
                RenewDeadline = TimeSpan.FromMilliseconds(650),
            };

            var countdown = new CountdownEvent(2);
            Task.Run(async () =>
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

                await leaderElector.RunUntilLeadershipLostAsync().ConfigureAwait(true);
            });

            countdown.Wait(TimeSpan.FromSeconds(15));
            electionHistoryCountdown.Wait(TimeSpan.FromSeconds(15));

            output.WriteLine(string.Join(",", electionHistory));

            Assert.True(electionHistory.Take(9).SequenceEqual(new[]
            {
                 "create record", "try update record", "update record", "try update record", "update record",
                 "try update record", "try update record", "try update record", "try update record",
            }));

            Assert.True(leadershipHistory.SequenceEqual(new[] { "get leadership", "start leading", "stop leading" }));
        }

        [Fact]
        public async Task LeaderElectionThrowException()
        {
            var l = new Mock<ILock>();
            l.Setup(obj => obj.GetAsync(CancellationToken.None))
                .Throws(new Exception("noxu"));

            var leaderElector = new LeaderElector(new LeaderElectionConfig(l.Object)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(1000),
                RetryPeriod = TimeSpan.FromMilliseconds(200),
                RenewDeadline = TimeSpan.FromMilliseconds(700),
            });

            try
            {
                await leaderElector.RunUntilLeadershipLostAsync().ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Assert.Equal("noxu", e.Message);
                return;
            }

            Assert.Fail("exception not thrown");
        }

        [Fact]
        public void LeaderElectionReportLeaderOnStart()
        {
            var l = new Mock<ILock>();
            l.Setup(obj => obj.Identity)
                .Returns("foo1");

            l.SetupSequence(obj => obj.GetAsync(CancellationToken.None))
                .ReturnsAsync(() =>
                {
                    return new LeaderElectionRecord()
                    {
                        HolderIdentity = "foo2",
                        AcquireTime = DateTime.Now,
                        RenewTime = DateTime.Now,
                        LeaderTransitions = 1,
                        LeaseDurationSeconds = 60,
                    };
                })
                .ReturnsAsync(() =>
                {
                    return new LeaderElectionRecord()
                    {
                        HolderIdentity = "foo3",
                        AcquireTime = DateTime.Now,
                        RenewTime = DateTime.Now,
                        LeaderTransitions = 1,
                        LeaseDurationSeconds = 60,
                    };
                });

            var leaderElector = new LeaderElector(new LeaderElectionConfig(l.Object)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(1000),
                RetryPeriod = TimeSpan.FromMilliseconds(200),
                RenewDeadline = TimeSpan.FromMilliseconds(700),
            });

            var countdown = new CountdownEvent(2);
            var notifications = new List<string>();
            leaderElector.OnNewLeader += id =>
            {
                notifications.Add(id);
                countdown.Signal();
            };

            Task.Run(() => leaderElector.RunUntilLeadershipLostAsync());
            countdown.Wait(TimeSpan.FromSeconds(10));

            Assert.True(notifications.SequenceEqual(new[]
            {
                "foo2", "foo3",
            }));
        }

        [Fact]
        public void LeaderElectionShouldReportLeaderItAcquiresOnStart()
        {
            var l = new Mock<ILock>();
            l.Setup(obj => obj.Identity)
                .Returns("foo1");

            l.Setup(obj => obj.GetAsync(CancellationToken.None))
                .ReturnsAsync(new LeaderElectionRecord()
                {
                    HolderIdentity = "foo1",
                    AcquireTime = DateTime.Now,
                    RenewTime = DateTime.Now,
                    LeaderTransitions = 1,
                    LeaseDurationSeconds = 60,
                });

            var leaderElector = new LeaderElector(new LeaderElectionConfig(l.Object)
            {
                LeaseDuration = TimeSpan.FromMilliseconds(1000),
                RetryPeriod = TimeSpan.FromMilliseconds(200),
                RenewDeadline = TimeSpan.FromMilliseconds(700),
            });

            var countdown = new CountdownEvent(1);
            var notifications = new List<string>();
            leaderElector.OnNewLeader += id =>
            {
                notifications.Add(id);
                countdown.Signal();
            };

            Task.Run(() => leaderElector.RunUntilLeadershipLostAsync());
            countdown.Wait(TimeSpan.FromSeconds(10));

            Assert.True(notifications.SequenceEqual(new[] { "foo1" }));
        }

        [Fact]
        public void LeaderElectionUsesActualLeaseDurationFromKubernetesObject()
        {
            // This test validates that the actual lease duration from the Kubernetes object
            // is used instead of the configured lease duration when checking if a lease has expired.
            // This is critical for graceful step-downs where a leader sets lease duration to 1 second.

            var l = new Mock<ILock>();
            l.Setup(obj => obj.Identity).Returns("client1");

            var firstCallTime = DateTime.UtcNow;
            var callCount = 0;

            l.Setup(obj => obj.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    // Return a lease held by another client with a short 1-second duration (graceful step-down)
                    return new LeaderElectionRecord()
                    {
                        HolderIdentity = "client2",
                        AcquireTime = firstCallTime.AddSeconds(-5),
                        RenewTime = firstCallTime,
                        LeaderTransitions = 1,
                        LeaseDurationSeconds = 1, // Actual lease duration is 1 second (not the configured 10 seconds)
                    };
                });

            var updateCalled = false;
            l.Setup(obj => obj.UpdateAsync(It.IsAny<LeaderElectionRecord>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    updateCalled = true;
                    return true;
                });

            var leaderElector = new LeaderElector(new LeaderElectionConfig(l.Object)
            {
                LeaseDuration = TimeSpan.FromSeconds(10), // Configured for 10 seconds
                RetryPeriod = TimeSpan.FromMilliseconds(200),
                RenewDeadline = TimeSpan.FromSeconds(9),
            });

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            // Run the leader election
            var task = Task.Run(async () =>
            {
                try
                {
                    await leaderElector.RunUntilLeadershipLostAsync(cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected when timeout occurs
                }
            });

            // Wait for the task to complete or timeout
            task.Wait(TimeSpan.FromSeconds(4));

            // The key assertion: With the fix, the lease should be recognized as expired after ~1 second
            // (the actual lease duration from the K8s object), not after 10 seconds (the configured duration).
            // Therefore, UpdateAsync should have been called to attempt to acquire leadership.
            Assert.True(updateCalled,
                "UpdateAsync should have been called after the actual lease duration (1 second) expired, " +
                "not after the configured lease duration (10 seconds). This test validates that the fix " +
                "correctly uses oldLeaderElectionRecord.LeaseDurationSeconds instead of config.LeaseDuration.");
        }

        private class MockResourceLock : ILock
        {
            private static LeaderElectionRecord leaderRecord;
            private static readonly object Lockobj = new object();

            public static void ResetGloablRecord()
            {
                leaderRecord = null;
            }

            private readonly string id;

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
                lock (Lockobj)
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
                lock (Lockobj)
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
