using System;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class KubernetesSchemeTests
    {
        [Fact]
        public void TestGVK()
        {
            Assert.NotNull(KubernetesScheme.Default);

            var s = new KubernetesScheme();

            // test s.GetGVK
            string g, v, k, p;
            s.GetGVK<V1Pod>(out g, out v, out k, out p);
            Assert.Equal("", g);
            Assert.Equal("v1", v);
            Assert.Equal("Pod", k);
            Assert.Equal("pods", p);

            s.GetGVK<V1PodList>(out g, out v, out k, out p);
            Assert.Equal("", g);
            Assert.Equal("v1", v);
            Assert.Equal("PodList", k);
            Assert.Equal("pods", p);

            s.GetGVK<CustomNew>(out g, out v, out k, out p);
            Assert.Equal("cgrp", g);
            Assert.Equal("v3", v);
            Assert.Equal("Yes", k);
            Assert.Equal("newz", p);

            s.GetGVK<CustomOld>(out g, out v, out k, out p);
            Assert.Equal("ogrp", g);
            Assert.Equal("v0", v);
            Assert.Equal("No", k);
            Assert.Equal("nos", p);

            Assert.Throws<ArgumentException>(() => s.GetGVK<Custom>(out g, out v, out k));

            // test s.GetVK
            s.GetVK<V1Pod>(out v, out k, out p);
            Assert.Equal("v1", v);
            Assert.Equal("Pod", k);
            Assert.Equal("pods", p);

            s.GetVK<V1PodList>(out v, out k, out p);
            Assert.Equal("v1", v);
            Assert.Equal("PodList", k);
            Assert.Equal("pods", p);

            s.GetVK<CustomNew>(out v, out k, out p);
            Assert.Equal("cgrp/v3", v);
            Assert.Equal("Yes", k);
            Assert.Equal("newz", p);

            s.GetVK<CustomOld>(out v, out k, out p);
            Assert.Equal("ogrp/v0", v);
            Assert.Equal("No", k);
            Assert.Equal("nos", p);

            Assert.Throws<ArgumentException>(() => s.GetVK<Custom>(out v, out k));

            // test s.TryGetGVK
            Assert.True(s.TryGetGVK<V1Pod>(out g, out v, out k, out p));
            Assert.Equal("", g);
            Assert.Equal("v1", v);
            Assert.Equal("Pod", k);
            Assert.Equal("pods", p);

            Assert.False(s.TryGetGVK<Custom>(out g, out v, out k, out p));

            // test s.TryGetVK
            Assert.True(s.TryGetVK<V1Pod>(out v, out k, out p));
            Assert.Equal("v1", v);
            Assert.Equal("Pod", k);
            Assert.Equal("pods", p);

            Assert.True(s.TryGetVK<CustomOld>(out v, out k, out p));
            Assert.Equal("ogrp/v0", v);
            Assert.Equal("No", k);
            Assert.Equal("nos", p);

            Assert.False(s.TryGetVK<Custom>(out v, out k, out p));

            // test s.SetGVK and s.RemoveGVK
            s.SetGVK<V1Pod>("g", "v", "k", "p");
            s.GetVK<V1Pod>(out v, out k, out p);
            Assert.Equal("g/v", v);
            Assert.Equal("k", k);
            Assert.Equal("p", p);

            s.GetGVK<V1PodList>(out g, out v, out k, out p);
            Assert.Equal("", g);
            Assert.Equal("v1", v);
            Assert.Equal("PodList", k);
            Assert.Equal("pods", p);

            s.SetGVK<Custom>("g2", "v2", "k2", "p2");
            s.GetVK<Custom>(out v, out k, out p);
            Assert.Equal("g2/v2", v);
            Assert.Equal("k2", k);
            Assert.Equal("p2", p);

            s.RemoveGVK<V1Pod>();
            s.GetGVK<V1Pod>(out g, out v, out k, out p);
            Assert.Equal("", g);
            Assert.Equal("v1", v);
            Assert.Equal("Pod", k);
            Assert.Equal("pods", p);

            s.RemoveGVK<Custom>();
            Assert.Throws<ArgumentException>(() => s.GetGVK<Custom>(out g, out v, out k));
        }

        [Fact]
        public void TestNew()
        {
            var s = new KubernetesScheme();

            var p = s.New<V1Pod>();
            Assert.Equal("v1", p.ApiVersion);
            Assert.Equal("Pod", p.Kind);

            var cn = s.New<CustomNew>("name");
            Assert.Equal("cgrp/v3", cn.ApiVersion);
            Assert.Equal("Yes", cn.Kind);
            Assert.Equal("name", cn.Metadata.Name);

            var co = s.New<CustomOld>("ns", "name");
            Assert.Equal("ogrp/v0", co.ApiVersion);
            Assert.Equal("No", co.Kind);
            Assert.Equal("name", co.Metadata.Name);
            Assert.Equal("ns", co.Metadata.NamespaceProperty);

            Assert.Throws<ArgumentException>(() => s.New<Custom>());
            s.SetGVK<Custom>("g", "v", "k", "p");

            var c = s.New<Custom>("ns", "name");
            Assert.Equal("g/v", c.ApiVersion);
            Assert.Equal("k", c.Kind);
            Assert.Equal("name", c.Metadata.Name);
            Assert.Equal("ns", c.Metadata.NamespaceProperty);
        }

        [KubernetesEntity(ApiVersion = "v3", Group = "cgrp", Kind = "Yes", PluralName = "newz")]
        class CustomNew : IKubernetesObject<V1ObjectMeta>
        {
            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public V1ObjectMeta Metadata { get; set; }
        }

        class CustomOld : IKubernetesObject<V1ObjectMeta>
        {
            public const string KubeApiVersion = "v0", KubeGroup = "ogrp", KubeKind = "No";
            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public V1ObjectMeta Metadata { get; set; }
        }

        class Custom : IKubernetesObject<V1ObjectMeta>
        {
            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public V1ObjectMeta Metadata { get; set; }
        }
    }
}
