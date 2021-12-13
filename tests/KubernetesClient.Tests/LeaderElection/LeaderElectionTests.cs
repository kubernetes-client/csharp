using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using k8s.LeaderElection;
using Moq;
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

            Task.Run(() =>
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

                leaderElector.RunAsync().Wait();
            });


            lockAStopLeading.WaitOne(TimeSpan.FromSeconds(3));

            Task.Run(() =>
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

                leaderElector.RunAsync().Wait();
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
        public void LeaderElectionThrowException()
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
                leaderElector.RunAsync().Wait();
            }
            catch (Exception e)
            {
                Assert.Equal("noxu", e.InnerException?.Message);
                return;
            }

            Assert.True(false, "exception not thrown");
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

            Task.Run(() => leaderElector.RunAsync());
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

            Task.Run(() => leaderElector.RunAsync());
            countdown.Wait(TimeSpan.FromSeconds(10));

            Assert.True(notifications.SequenceEqual(new[] { "foo1" }));
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
