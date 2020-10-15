using System;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class ModelExtensionTests
    {
        [Fact]
        public void TestMetadata()
        {
            // test getters on null metadata
            var pod = new V1Pod();
            Assert.Null(pod.Annotations());
            Assert.Null(pod.ApiGroup());
            var (g, v) = pod.ApiGroupAndVersion();
            Assert.Null(g);
            Assert.Null(v);
            Assert.Null(pod.ApiGroupVersion());
            Assert.Null(pod.CreationTimestamp());
            Assert.Null(pod.DeletionTimestamp());
            Assert.Null(pod.Finalizers());
            Assert.Equal(-1, pod.FindOwnerReference(r => true));
            Assert.Null(pod.Generation());
            Assert.Null(pod.GetAnnotation("x"));
            Assert.Null(pod.GetController());
            Assert.Null(pod.GetLabel("x"));
            Assert.Null(pod.GetOwnerReference(r => true));
            Assert.False(pod.HasFinalizer("x"));
            Assert.Null(pod.Labels());
            Assert.Null(pod.Name());
            Assert.Null(pod.Namespace());
            Assert.Null(pod.OwnerReferences());
            Assert.Null(pod.ResourceVersion());
            Assert.Null(pod.Uid());
            Assert.Null(pod.Metadata);

            // test API version stuff
            pod = new V1Pod() { ApiVersion = "v1" };
            Assert.Equal("", pod.ApiGroup());
            (g, v) = pod.ApiGroupAndVersion();
            Assert.Equal("", g);
            Assert.Equal("v1", v);
            Assert.Equal("v1", pod.ApiGroupVersion());
            pod.ApiVersion = "abc/v2";
            Assert.Equal("abc", pod.ApiGroup());
            (g, v) = pod.ApiGroupAndVersion();
            Assert.Equal("abc", g);
            Assert.Equal("v2", v);
            Assert.Equal("v2", pod.ApiGroupVersion());

            // test the Ensure*() functions
            Assert.NotNull(pod.EnsureMetadata());
            Assert.NotNull(pod.Metadata);
            Assert.NotNull(pod.Metadata.EnsureAnnotations());
            Assert.NotNull(pod.Metadata.Annotations);
            Assert.NotNull(pod.Metadata.EnsureFinalizers());
            Assert.NotNull(pod.Metadata.Finalizers);
            Assert.NotNull(pod.Metadata.EnsureLabels());
            Assert.NotNull(pod.Metadata.Labels);

            // test getters with non-null values
            DateTime ts = DateTime.UtcNow, ts2 = DateTime.Now;
            pod.Metadata = new V1ObjectMeta()
            {
                CreationTimestamp = ts,
                DeletionTimestamp = ts2,
                Generation = 1,
                Name = "name",
                NamespaceProperty = "ns",
                ResourceVersion = "42",
                Uid = "id",
            };
            Assert.Equal(ts, pod.CreationTimestamp().Value);
            Assert.Equal(ts2, pod.DeletionTimestamp().Value);
            Assert.Equal(1, pod.Generation().Value);
            Assert.Equal("name", pod.Name());
            Assert.Equal("ns", pod.Namespace());
            Assert.Equal("42", pod.ResourceVersion());
            Assert.Equal("id", pod.Uid());

            // test annotations and labels
            pod.SetAnnotation("x", "y");
            pod.SetLabel("a", "b");
            Assert.Equal(1, pod.Annotations().Count);
            Assert.Equal(1, pod.Labels().Count);
            Assert.Equal("y", pod.GetAnnotation("x"));
            Assert.Equal("y", pod.Metadata.Annotations["x"]);
            Assert.Null(pod.GetAnnotation("a"));
            Assert.Equal("b", pod.GetLabel("a"));
            Assert.Equal("b", pod.Metadata.Labels["a"]);
            Assert.Null(pod.GetLabel("x"));
            pod.SetAnnotation("x", null);
            Assert.Equal(0, pod.Annotations().Count);
            pod.SetLabel("a", null);
            Assert.Equal(0, pod.Labels().Count);

            // test finalizers
            Assert.False(pod.HasFinalizer("abc"));
            Assert.True(pod.AddFinalizer("abc"));
            Assert.True(pod.HasFinalizer("abc"));
            Assert.False(pod.AddFinalizer("abc"));
            Assert.False(pod.HasFinalizer("xyz"));
            Assert.False(pod.RemoveFinalizer("xyz"));
            Assert.True(pod.RemoveFinalizer("abc"));
            Assert.False(pod.HasFinalizer("abc"));
            Assert.False(pod.RemoveFinalizer("abc"));
        }

        [Fact]
        public void TestReferences()
        {
            // test object references
            var pod = new V1Pod() { ApiVersion = "v1", Kind = "Pod" };
            pod.Metadata = new V1ObjectMeta() { Name = "name", NamespaceProperty = "ns", ResourceVersion = "ver", Uid = "id" };

            var objr = new V1ObjectReference() { ApiVersion = pod.ApiVersion, Kind = pod.Kind, Name = pod.Name(), NamespaceProperty = pod.Namespace(), Uid = pod.Uid() };
            Assert.True(objr.Matches(pod));

            (pod.ApiVersion, pod.Kind) = (null, null);
            Assert.False(objr.Matches(pod));
            (pod.ApiVersion, pod.Kind) = ("v1", "Pod");
            Assert.True(objr.Matches(pod));
            pod.Metadata.Name = "nome";
            Assert.False(objr.Matches(pod));

            // test owner references
            (pod.ApiVersion, pod.Kind) = ("abc/xyz", "sometimes");
            var ownr = new V1OwnerReference() { ApiVersion = "abc/xyz", Kind = "sometimes", Name = pod.Name(), Uid = pod.Uid() };
            Assert.True(ownr.Matches(pod));

            (pod.ApiVersion, pod.Kind) = (null, null);
            Assert.False(ownr.Matches(pod));
            (ownr.ApiVersion, ownr.Kind) = ("v1", "Pod");
            Assert.False(ownr.Matches(pod));
            (pod.ApiVersion, pod.Kind) = (ownr.ApiVersion, ownr.Kind);
            Assert.True(ownr.Matches(pod));
            ownr.Name = "nim";
            Assert.False(ownr.Matches(pod));
            ownr.Name = pod.Name();

            var svc = new V1Service();
            svc.AddOwnerReference(ownr);
            Assert.Equal(0, svc.FindOwnerReference(pod));
            Assert.Equal(-1, svc.FindOwnerReference(svc));
            Assert.Same(ownr, svc.GetOwnerReference(pod));
            Assert.Null(svc.GetOwnerReference(svc));
            Assert.Null(svc.GetController());
            svc.OwnerReferences()[0].Controller = true;
            Assert.Same(ownr, svc.GetController());
            Assert.Same(ownr, svc.RemoveOwnerReference(pod));
            Assert.Equal(0, svc.OwnerReferences().Count);
            svc.AddOwnerReference(new V1OwnerReference() { ApiVersion = pod.ApiVersion, Kind = pod.Kind, Name = pod.Name(), Uid = pod.Uid(), Controller = true });
            svc.AddOwnerReference(new V1OwnerReference() { ApiVersion = pod.ApiVersion, Kind = pod.Kind, Name = pod.Name(), Uid = pod.Uid(), Controller = false });
            svc.AddOwnerReference(new V1OwnerReference() { ApiVersion = pod.ApiVersion, Kind = pod.Kind, Name = pod.Name(), Uid = pod.Uid() });
            Assert.Equal(3, svc.OwnerReferences().Count);
            Assert.NotNull(svc.RemoveOwnerReference(pod));
            Assert.Equal(2, svc.OwnerReferences().Count);
            Assert.True(svc.RemoveOwnerReferences(pod));
            Assert.Equal(0, svc.OwnerReferences().Count);
        }

        [Fact]
        public void TestV1Status()
        {
            var s = new V1Status() { Status = "Success" };
            Assert.Equal("Success", s.ToString());

            s = new V1Status() { Status = "Failure" };
            Assert.Equal("Failure", s.ToString());

            s = new V1Status() { Status = "Failure", Reason = "BombExploded" };
            Assert.Equal("BombExploded", s.ToString());

            s = new V1Status() { Status = "Failure", Message = "Something bad happened." };
            Assert.Equal("Something bad happened.", s.ToString());

            s = new V1Status() { Status = "Failure", Code = 400 };
            Assert.Equal("BadRequest", s.ToString());

            s = new V1Status() { Status = "Failure", Code = 911 };
            Assert.Equal("911", s.ToString());

            s = new V1Status() { Status = "Failure", Code = 400, Message = "It's all messed up." };
            Assert.Equal("BadRequest - It's all messed up.", s.ToString());

            s = new V1Status()
            {
                Status = "Failure",
                Code = 400,
                Reason = "IllegalValue",
                Message = "You're breaking the LAW!",
            };
            Assert.Equal("IllegalValue - You're breaking the LAW!", s.ToString());
        }
    }
}
