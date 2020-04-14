using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using k8s.Informers.Notifications;
using k8s.Models;
using k8s.Tests.Utils;

namespace k8s.Tests
{
    public static class TestData
    {
        public static class Events
        {
            public static List<Tuple<string, ScheduledEvent<TestResource>[]>> AllScenarios =>
                typeof(Events).GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.PropertyType == typeof(ScheduledEvent<TestResource>[]))
                    .Select(x => Tuple.Create(x.Name, (ScheduledEvent<TestResource>[])x.GetMethod.Invoke(null, null)))
                    .ToList();
            public static ScheduledEvent<TestResource>[] EmptyReset_Delay_Add => new[]
                 {
                    new ResourceEvent<TestResource>(EventTypeFlags.ResetEmpty, null).ScheduleFiring(0),
                    new TestResource(1).ToResourceEvent(EventTypeFlags.Add).ScheduleFiring(100)
                };
            public static ScheduledEvent<TestResource>[] ResetWith2_Delay_2xUpdateTo1 => new[]
                {
                    new TestResource(1).ToResourceEvent(EventTypeFlags.ResetStart).ScheduleFiring(0),
                    new TestResource(2).ToResourceEvent(EventTypeFlags.ResetEnd).ScheduleFiring(0),
                    new TestResource(1).ToResourceEvent(EventTypeFlags.Modify).ScheduleFiring(100),
                    new TestResource(1).ToResourceEvent(EventTypeFlags.Modify).ScheduleFiring(200)
                };

            public static ScheduledEvent<TestResource>[] ResetWith2_Delay_UpdateBoth_Delay_Add1 => new[]
            {
                new TestResource(1).ToResourceEvent(EventTypeFlags.ResetStart).ScheduleFiring(0),
                new TestResource(2).ToResourceEvent(EventTypeFlags.ResetEnd).ScheduleFiring(0),
                new TestResource(1).ToResourceEvent(EventTypeFlags.Modify).ScheduleFiring(100),
                new TestResource(1).ToResourceEvent(EventTypeFlags.Modify).ScheduleFiring(200),
                new TestResource(3).ToResourceEvent(EventTypeFlags.Add).ScheduleFiring(400),
            };

            public static ScheduledEvent<TestResource>[] ResetWith2_Delay_ResetWith1One_ImplicitDeletion => new[]
                {
                    new TestResource(1).ToResourceEvent(EventTypeFlags.ResetStart).ScheduleFiring(0),
                    new TestResource(2).ToResourceEvent(EventTypeFlags.ResetEnd).ScheduleFiring(0),

                    new TestResource(1).ToResourceEvent(EventTypeFlags.ResetStart | EventTypeFlags.ResetEnd).ScheduleFiring(100),
                };

            public static ScheduledEvent<TestResource>[] ResetWith1_Delay_ResetWith2_ImplicitAddition => new[]
                {
                    new TestResource(1).ToResourceEvent(EventTypeFlags.ResetStart | EventTypeFlags.ResetEnd).ScheduleFiring(0),

                    new TestResource(1).ToResourceEvent(EventTypeFlags.ResetStart).ScheduleFiring(100),
                    new TestResource(2).ToResourceEvent(EventTypeFlags.ResetEnd).ScheduleFiring(100),
                };
            public static ScheduledEvent<TestResource>[] ResetWith2_Delay_ResetWith2OneDifferentVersion_ImplicitUpdate => new[]
            {
                new TestResource(1,  1).ToResourceEvent(EventTypeFlags.ResetStart).ScheduleFiring(0),
                new TestResource(2,  1).ToResourceEvent(EventTypeFlags.ResetEnd).ScheduleFiring(0),

                new TestResource(1,  1).ToResourceEvent(EventTypeFlags.ResetStart).ScheduleFiring(100),
                new TestResource(2,  2).ToResourceEvent(EventTypeFlags.ResetEnd).ScheduleFiring(100),
            };

            public static ScheduledEvent<TestResource>[] Sync_Delay_SyncAndUpdate => new[]
                {
                    new TestResource(1).ToResourceEvent(EventTypeFlags.Sync).ScheduleFiring(0),

                    new TestResource(1).ToResourceEvent(EventTypeFlags.Sync).ScheduleFiring(100),
                    new TestResource(1).ToResourceEvent(EventTypeFlags.Modify).ScheduleFiring(120),
                };
        }
        public static V1Pod TestPod1ResourceVersion1 => new V1Pod()
        {
            Kind = V1Pod.KubeKind,
            ApiVersion = V1Pod.KubeApiVersion,
            Metadata = new V1ObjectMeta()
            {
                Name = "pod1",
                ResourceVersion = "pod1V1"
            }
        };


        public static V1Pod TestPod1ResourceVersion2 => new V1Pod()
        {
            Kind = V1Pod.KubeKind,
            ApiVersion = V1Pod.KubeApiVersion,
            Metadata = new V1ObjectMeta()
            {
                Name = "pod1",
                ResourceVersion = "pod1V2"
            }
        };
        public static V1Pod TestPod2ResourceVersion1 => new V1Pod()
        {
            Kind = V1Pod.KubeKind,
            ApiVersion = V1Pod.KubeApiVersion,
            Metadata = new V1ObjectMeta()
            {
                Name = "pod2",
                ResourceVersion = "pod2V1"
            }
        };

        public static V1Pod TestPod2ResourceVersion2 => new V1Pod()
        {
            Kind = V1Pod.KubeKind,
            ApiVersion = V1Pod.KubeApiVersion,
            Metadata = new V1ObjectMeta()
            {
                Name = "pod2",
                ResourceVersion = "pod2V2"
            }
        };

        public static V1PodList ListPodEmpty => new V1PodList()
        {
            Kind = V1PodList.KubeKind,
            ApiVersion = V1PodList.KubeApiVersion,
            Metadata = new V1ListMeta()
            {
                ResourceVersion = "podlistV1"
            }
        };
        public static V1PodList ListPodOneItem => new V1PodList()
        {
            Kind = V1PodList.KubeKind,
            ApiVersion = V1PodList.KubeApiVersion,
            Metadata = new V1ListMeta()
            {
                ResourceVersion = "podlistV2"
            },
            Items = new List<V1Pod>()
            {
                TestPod1ResourceVersion1
            }
        };
        public static V1PodList ListPodsTwoItems => new V1PodList()
        {
            Kind = V1PodList.KubeKind,
            ApiVersion = V1PodList.KubeApiVersion,
            Metadata = new V1ListMeta()
            {
                ResourceVersion = "podlistV3"
            },
            Items = new List<V1Pod>()
            {
                TestPod1ResourceVersion1,
                TestPod2ResourceVersion1
            }
        };
    }
}
