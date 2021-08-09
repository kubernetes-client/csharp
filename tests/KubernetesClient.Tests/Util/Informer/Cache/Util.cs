using System;
using System.Collections.Generic;
using System.Linq;
using k8s.Models;

namespace k8s.Tests.Util.Informer.Cache
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
                        ResourceVersion = "1",
                    },
                });
            }

            return pods;
        }

        internal static V1PodList CreatePostList(int cnt)
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
    }
}
