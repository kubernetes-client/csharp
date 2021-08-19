using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using k8s.Models;
using k8s.Util.Common.Generic;

namespace k8s.Tests.Util.Informer
{
    internal static class Util
    {
        internal static IEnumerable<V1Pod> CreatePods(int cnt)
        {
            var pods = new List<V1Pod>();
            for (var i = 0; i < cnt; i++)
            {
                pods.Add(new V1Pod()
                {
                    ApiVersion = "Pod/V1",
                    Kind = "Pod",
                    Metadata = new V1ObjectMeta()
                    {
                        Name = Guid.NewGuid().ToString(),
                        NamespaceProperty = "the-namespace",
                        ResourceVersion = DateTime.Now.Ticks.ToString(),
                    },
                });
            }

            return pods;
        }

        internal static V1PodList CreatePodList(int cnt)
        {
            return new V1PodList()
            {
                ApiVersion = "Pod/V1",
                Kind = "Pod",
                Metadata = new V1ListMeta()
                {
                    ResourceVersion = "1",
                },
                Items = CreatePods(cnt).ToList(),
            };
        }

        internal static Kubernetes BuildApiClient(Uri hostAddress)
        {
            return new Kubernetes(new KubernetesClientConfiguration { Host = hostAddress.ToString() })
            {
                HttpClient =
                {
                    Timeout = Timeout.InfiniteTimeSpan,
                },
            };
        }

        internal static GenericKubernetesApi BuildGenericApi(Uri hostAddress)
        {
            return new GenericKubernetesApi(
                apiGroup: "pod",
                apiVersion: "v1",
                resourcePlural: "pods",
                apiClient: BuildApiClient(hostAddress));
        }
    }
}
