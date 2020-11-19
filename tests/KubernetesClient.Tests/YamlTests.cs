using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class YamlTests
    {
        [Fact]
        public void LoadAllFromString()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
---
apiVersion: v1
kind: Namespace
metadata:
  name: ns";

            var types = new Dictionary<string, Type>();
            types.Add("v1/Pod", typeof(V1Pod));
            types.Add("v1/Namespace", typeof(V1Namespace));

            var objs = Yaml.LoadAllFromString(content, types);
            Assert.Equal(2, objs.Count);
            Assert.IsType<V1Pod>(objs[0]);
            Assert.IsType<V1Namespace>(objs[1]);
            Assert.Equal("foo", ((V1Pod)objs[0]).Metadata.Name);
            Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
        }

        [Fact]
        public void LoadFromString()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

            var obj = Yaml.LoadFromString<V1Pod>(content);

            Assert.Equal("foo", obj.Metadata.Name);
        }

        [Fact]
        public void LoadNamespacedFromString()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  namespace: bar
  name: foo
";

            var obj = Yaml.LoadFromString<V1Pod>(content);

            Assert.Equal("foo", obj.Metadata.Name);
            Assert.Equal("bar", obj.Metadata.NamespaceProperty);
        }

        [Fact]
        public void LoadPropertyNamedReadOnlyFromString()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  namespace: bar
  name: foo
spec:
  containers:
    - image: nginx
      volumeMounts:
      - name: vm1
        mountPath: /vm1
        readOnly: true
      - name: vm2
        mountPath: /vm2
        readOnly: false
";

            var obj = Yaml.LoadFromString<V1Pod>(content);

            Assert.True(obj.Spec.Containers[0].VolumeMounts[0].ReadOnlyProperty);
            Assert.False(obj.Spec.Containers[0].VolumeMounts[1].ReadOnlyProperty);
        }

        [Fact]
        public void LoadFromStream()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var obj = Yaml.LoadFromStreamAsync<V1Pod>(stream).Result;

                Assert.Equal("foo", obj.Metadata.Name);
            }
        }

        [Fact]
        public void WriteToString()
        {
            var pod = new V1Pod() { ApiVersion = "v1", Kind = "Pod", Metadata = new V1ObjectMeta() { Name = "foo" } };

            var yaml = Yaml.SaveToString(pod);
            Assert.Equal(
                ToLines(@"apiVersion: v1
kind: Pod
metadata:
  name: foo"), ToLines(yaml));
        }

        [Fact]
        public void WriteNamespacedToString()
        {
            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta() { Name = "foo", NamespaceProperty = "bar" },
            };

            var yaml = Yaml.SaveToString(pod);
            Assert.Equal(
                ToLines(@"apiVersion: v1
kind: Pod
metadata:
  name: foo
  namespace: bar"), ToLines(yaml));
        }

        [Fact]
        public void WritePropertyNamedReadOnlyToString()
        {
            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta() { Name = "foo", NamespaceProperty = "bar" },
                Spec = new V1PodSpec()
                {
                    Containers = new[]
                    {
                        new V1Container()
                        {
                            Image = "nginx",
                            VolumeMounts = new[]
                            {
                                new V1VolumeMount
                                {
                                    Name = "vm1", MountPath = "/vm1", ReadOnlyProperty = true,
                                },
                                new V1VolumeMount
                                {
                                    Name = "vm2", MountPath = "/vm2", ReadOnlyProperty = false,
                                },
                            },
                        },
                    },
                },
            };

            var yaml = Yaml.SaveToString(pod);
            Assert.Equal(
                ToLines(@"apiVersion: v1
kind: Pod
metadata:
  name: foo
  namespace: bar
spec:
  containers:
  - image: nginx
    volumeMounts:
    - mountPath: /vm1
      name: vm1
      readOnly: true
    - mountPath: /vm2
      name: vm2
      readOnly: false"), ToLines(yaml));
        }

        private static IEnumerable<string> ToLines(string s)
        {
            using (var reader = new StringReader(s))
            {
                for (; ; )
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        yield break;
                    }

                    yield return line;
                }
            }
        }

        [Fact]
        public void CpuRequestAndLimitFromString()
        {
            // Taken from https://raw.githubusercontent.com/kubernetes/website/master/docs/tasks/configure-pod-container/cpu-request-limit.yaml, although
            // the 'namespace' property on 'metadata' was removed since it was rejected by the C# client.
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: cpu-demo
spec:
  containers:
  - name: cpu-demo-ctr
    image: vish/stress
    resources:
        limits:
            cpu: ""1""
        requests:
            cpu: ""0.5""
    args:
            - -cpus
            - ""2""";

            var obj = Yaml.LoadFromString<V1Pod>(content);

            Assert.NotNull(obj?.Spec?.Containers);
            var container = Assert.Single(obj.Spec.Containers);

            Assert.NotNull(container.Resources);
            Assert.NotNull(container.Resources.Limits);
            Assert.NotNull(container.Resources.Requests);

            var cpuLimit = Assert.Single(container.Resources.Limits);
            var cpuRequest = Assert.Single(container.Resources.Requests);

            Assert.Equal("cpu", cpuLimit.Key);
            Assert.Equal("1", cpuLimit.Value.ToString());

            Assert.Equal("cpu", cpuRequest.Key);
            Assert.Equal("500m", cpuRequest.Value.ToString());
        }

        [Fact]
        public void LoadIntOrString()
        {
            var content = @"apiVersion: v1
kind: Service
spec:
  ports:
  - port: 3000
    targetPort: 3000
";

            var obj = Yaml.LoadFromString<V1Service>(content);

            Assert.Equal(3000, obj.Spec.Ports[0].Port);
            Assert.Equal(3000, int.Parse(obj.Spec.Ports[0].TargetPort));
        }

        [Fact]
        public void SerializeIntOrString()
        {
            var content = @"apiVersion: v1
kind: Service
metadata:
  labels:
    app: test
  name: test-svc
spec:
  ports:
  - port: 3000
    targetPort: 3000";

            var labels = new Dictionary<string, string> { { "app", "test" } };
            var obj = new V1Service
            {
                Kind = "Service",
                Metadata = new V1ObjectMeta(labels: labels, name: "test-svc"),
                ApiVersion = "v1",
                Spec = new V1ServiceSpec
                {
                    Ports = new List<V1ServicePort> { new V1ServicePort { Port = 3000, TargetPort = 3000 } },
                },
            };

            var output = Yaml.SaveToString(obj);
            Assert.Equal(ToLines(output), ToLines(content));
        }

        [Fact]
        public void QuotedValuesShouldRemainQuotedAfterSerialization()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  annotations:
    custom.annotation: ""null""
  name: cpu-demo
spec:
  containers:
  - env:
    - name: PORT
      value: ""3000""
    - name: NUM_RETRIES
      value: ""3""
    - name: ENABLE_CACHE
      value: ""true""
    - name: ENABLE_OTHER
      value: ""false""
    image: vish/stress
    name: cpu-demo-ctr";
            var obj = Yaml.LoadFromString<V1Pod>(content);
            Assert.NotNull(obj?.Spec?.Containers);
            var container = Assert.Single(obj.Spec.Containers);
            Assert.NotNull(container.Env);
            var objStr = Yaml.SaveToString(obj);
            Assert.Equal(content, objStr);
        }

        [Fact]
        public void LoadSecret()
        {
            var kManifest = @"
apiVersion: v1
kind: Secret
metadata:
  name: test-secret
data:
  username: bXktYXBw
  password: Mzk1MjgkdmRnN0pi
";

            var result = Yaml.LoadFromString<V1Secret>(kManifest);
            Assert.Equal("bXktYXBw", Encoding.UTF8.GetString(result.Data["username"]));
            Assert.Equal("Mzk1MjgkdmRnN0pi", Encoding.UTF8.GetString(result.Data["password"]));
        }
    }
}
