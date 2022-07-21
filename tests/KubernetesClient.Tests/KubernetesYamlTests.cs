using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class KubernetesYamlTests
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

            var objs = KubernetesYaml.LoadAllFromString(content);
            Assert.Equal(2, objs.Count);
            Assert.IsType<V1Pod>(objs[0]);
            Assert.IsType<V1Namespace>(objs[1]);
            Assert.Equal("foo", ((V1Pod)objs[0]).Metadata.Name);
            Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
        }

#pragma warning disable CA1812 // Class is used for YAML deserialization tests
        private class MyPod : V1Pod
        {
        }
#pragma warning restore CA1812

        [Fact]
        public void LoadAllFromStringWithTypes()
        {
            var types = new Dictionary<string, Type>();
            types.Add("v1/Pod", typeof(MyPod));

            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
---
apiVersion: v1
kind: Namespace
metadata:
  name: ns";

            var objs = KubernetesYaml.LoadAllFromString(content, types);
            Assert.Equal(2, objs.Count);
            Assert.IsType<MyPod>(objs[0]);
            Assert.IsType<V1Namespace>(objs[1]);
            Assert.Equal("foo", ((MyPod)objs[0]).Metadata.Name);
            Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
        }

        [Fact]
        public void LoadAllFromStringWithAdditionalProperties()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
  namespace: ns
  youDontKnow: Me
---
apiVersion: v1
kind: Namespace
metadata:
  name: ns
  youDontKnow: Me";

            var objs = KubernetesYaml.LoadAllFromString(content);
            Assert.Equal(2, objs.Count);
            Assert.IsType<V1Pod>(objs[0]);
            Assert.IsType<V1Namespace>(objs[1]);
            Assert.Equal("foo", ((V1Pod)objs[0]).Metadata.Name);
            Assert.Equal("ns", ((V1Pod)objs[0]).Metadata.NamespaceProperty);
            Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
        }

        [Fact]
        public void LoadAllFromStringWithAdditionalPropertiesAndTypes()
        {
            var types = new Dictionary<string, Type>();
            types.Add("v1/Pod", typeof(MyPod));
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
  namespace: ns
  youDontKnow: Me
---
apiVersion: v1
kind: Namespace
metadata:
  name: ns
  youDontKnow: Me";

            var objs = KubernetesYaml.LoadAllFromString(content, types);
            Assert.Equal(2, objs.Count);
            Assert.IsType<MyPod>(objs[0]);
            Assert.IsType<V1Namespace>(objs[1]);
            Assert.Equal("foo", ((MyPod)objs[0]).Metadata.Name);
            Assert.Equal("ns", ((MyPod)objs[0]).Metadata.NamespaceProperty);
            Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
        }

        [Fact]
        public async Task LoadAllFromFile()
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

            var tempFileName = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(tempFileName, content).ConfigureAwait(false);

                var objs = await KubernetesYaml.LoadAllFromFileAsync(tempFileName).ConfigureAwait(false);
                Assert.Equal(2, objs.Count);
                Assert.IsType<V1Pod>(objs[0]);
                Assert.IsType<V1Namespace>(objs[1]);
                Assert.Equal("foo", ((V1Pod)objs[0]).Metadata.Name);
                Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }

        [Fact]
        public async Task LoadAllFromFileWithTypes()
        {
            var types = new Dictionary<string, Type>();
            types.Add("v1/Pod", typeof(MyPod));

            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
---
apiVersion: v1
kind: Namespace
metadata:
  name: ns";

            var tempFileName = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(tempFileName, content).ConfigureAwait(false);

                var objs = await KubernetesYaml.LoadAllFromFileAsync(tempFileName, types).ConfigureAwait(false);
                Assert.Equal(2, objs.Count);
                Assert.IsType<MyPod>(objs[0]);
                Assert.IsType<V1Namespace>(objs[1]);
                Assert.Equal("foo", ((MyPod)objs[0]).Metadata.Name);
                Assert.Equal("ns", ((V1Namespace)objs[1]).Metadata.Name);
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }

        [Fact]
        public void LoadFromString()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

            var obj = KubernetesYaml.Deserialize<V1Pod>(content);

            Assert.Equal("foo", obj.Metadata.Name);
        }

        [Fact]
        public void LoadFromStringWithAdditionalProperties()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
  youDontKnow: Me
";

            var obj = KubernetesYaml.Deserialize<V1Pod>(content);

            Assert.Equal("foo", obj.Metadata.Name);
        }

        [Fact]
        public void LoadFromStringWithAdditionalPropertiesAndCustomType()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
  youDontKnow: Me
";

            var obj = KubernetesYaml.Deserialize<V1Pod>(content);

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

            var obj = KubernetesYaml.Deserialize<V1Pod>(content);

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

            var obj = KubernetesYaml.Deserialize<V1Pod>(content);

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
                var obj = KubernetesYaml.LoadFromStreamAsync<V1Pod>(stream).Result;

                Assert.Equal("foo", obj.Metadata.Name);
            }
        }

        [Fact]
        public async Task LoadFromFile()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

            var tempFileName = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(tempFileName, content).ConfigureAwait(false);

                var obj = await KubernetesYaml.LoadFromFileAsync<V1Pod>(tempFileName).ConfigureAwait(false);
                Assert.Equal("foo", obj.Metadata.Name);
            }
            finally
            {
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }

        [Fact]
        public void RoundtripTypeWithMismatchedPropertyName()
        {
            var content = @"namespace: foo";

            var deserialized = KubernetesYaml.Deserialize<V1ObjectMeta>(content);
            Assert.Equal("foo", deserialized.NamespaceProperty);

            var serialized = KubernetesYaml.Serialize(deserialized);
            Assert.Equal(content, serialized);
        }

        [Fact]
        public void SerializeAll()
        {
            var pods = new List<object>
            {
                new V1Pod() { ApiVersion = "v1", Kind = "Pod", Metadata = new V1ObjectMeta() { Name = "foo" } },
                new V1Pod() { ApiVersion = "v1", Kind = "Pod", Metadata = new V1ObjectMeta() { Name = "bar" } },
            };
            var yaml = KubernetesYaml.SerializeAll(pods);
            Assert.Equal(
                ToLines(@"apiVersion: v1
kind: Pod
metadata:
  name: foo
---
apiVersion: v1
kind: Pod
metadata:
  name: bar"), ToLines(yaml));
        }

        [Fact]
        public void WriteToString()
        {
            var pod = new V1Pod() { ApiVersion = "v1", Kind = "Pod", Metadata = new V1ObjectMeta() { Name = "foo" } };

            var yaml = KubernetesYaml.Serialize(pod);
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

            var yaml = KubernetesYaml.Serialize(pod);
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

            var yaml = KubernetesYaml.Serialize(pod);
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

            var obj = KubernetesYaml.Deserialize<V1Pod>(content);

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

            var obj = KubernetesYaml.Deserialize<V1Service>(content);

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

            var output = KubernetesYaml.Serialize(obj);
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
            var obj = KubernetesYaml.Deserialize<V1Pod>(content);
            Assert.NotNull(obj?.Spec?.Containers);
            var container = Assert.Single(obj.Spec.Containers);
            Assert.NotNull(container.Env);
            var objStr = KubernetesYaml.Serialize(obj);
            Assert.Equal(content.Replace("\r\n", "\n"), objStr.Replace("\r\n", "\n"));
        }

        [Fact]
        public void CertainPatternsShouldBeSerializedWithDoubleQuotes()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  annotations:
    custom.annotation: ""True""
    second.custom.annotation: ""~""
  name: foo
  namespace: bar
spec:
  containers:
  - env:
    - name: NullLowerCase
      value: ""null""
    - name: NullCamelCase
      value: ""Null""
    - name: NullUpperCase
      value: ""NULL""
    - name: TrueLowerCase
      value: ""true""
    - name: TrueCamelCase
      value: ""True""
    - name: ""TRUE""
      value: ""TRUE""
    - name: ""false""
      value: ""false""
    - name: ""False""
      value: ""False""
    - name: ""FALSE""
      value: ""FALSE""
    - name: ""y""
      value: ""y""
    - name: ""Y""
      value: ""Y""
    - name: ""yes""
      value: ""yes""
    - name: ""Yes""
      value: ""Yes""
    - name: ""YES""
      value: ""YES""
    - name: ""n""
      value: ""n""
    - name: ""N""
      value: ""N""
    - name: ""no""
      value: ""no""
    - name: ""No""
      value: ""No""
    - name: ""NO""
      value: ""NO""
    - name: ""on""
      value: ""on""
    - name: ""On""
      value: ""On""
    - name: ""ON""
      value: ""ON""
    - name: ""off""
      value: ""off""
    - name: ""Off""
      value: ""Off""
    - name: ""OFF""
      value: ""OFF""
    image: nginx
    name: foo";

            var pod = new V1Pod()
            {
                ApiVersion = "v1",
                Kind = "Pod",
                Metadata = new V1ObjectMeta()
                {
                    Name = "foo",
                    NamespaceProperty = "bar",
                    Annotations = new Dictionary<string, string>
                    {
                        { "custom.annotation", "True" },
                        { "second.custom.annotation", "~" },
                    },
                },
                Spec = new V1PodSpec()
                {
                    Containers = new[]
                    {
                        new V1Container()
                        {
                            Image = "nginx",
                            Env = new List<V1EnvVar>
                            {
                                new V1EnvVar
                                {
                                    Name = "NullLowerCase",
                                    Value = "null",
                                },
                                new V1EnvVar
                                {
                                    Name = "NullCamelCase",
                                    Value = "Null",
                                },
                                new V1EnvVar
                                {
                                    Name = "NullUpperCase",
                                    Value = "NULL",
                                },
                                new V1EnvVar
                                {
                                    Name = "TrueLowerCase",
                                    Value = "true",
                                },
                                new V1EnvVar
                                {
                                    Name = "TrueCamelCase",
                                    Value = "True",
                                },
                                new V1EnvVar
                                {
                                    Name = "TRUE",
                                    Value = "TRUE",
                                },
                                new V1EnvVar
                                {
                                    Name = "false",
                                    Value = "false",
                                },
                                new V1EnvVar
                                {
                                    Name = "False",
                                    Value = "False",
                                },
                                new V1EnvVar
                                {
                                    Name = "FALSE",
                                    Value = "FALSE",
                                },
                                new V1EnvVar
                                {
                                    Name = "y",
                                    Value = "y",
                                },
                                new V1EnvVar
                                {
                                    Name = "Y",
                                    Value = "Y",
                                },
                                new V1EnvVar
                                {
                                    Name = "yes",
                                    Value = "yes",
                                },
                                new V1EnvVar
                                {
                                    Name = "Yes",
                                    Value = "Yes",
                                },
                                new V1EnvVar
                                {
                                    Name = "YES",
                                    Value = "YES",
                                },
                                new V1EnvVar
                                {
                                    Name = "n",
                                    Value = "n",
                                },
                                new V1EnvVar
                                {
                                    Name = "N",
                                    Value = "N",
                                },
                                new V1EnvVar
                                {
                                    Name = "no",
                                    Value = "no",
                                },
                                new V1EnvVar
                                {
                                    Name = "No",
                                    Value = "No",
                                },
                                new V1EnvVar
                                {
                                    Name = "NO",
                                    Value = "NO",
                                },
                                new V1EnvVar
                                {
                                    Name = "on",
                                    Value = "on",
                                },
                                new V1EnvVar
                                {
                                    Name = "On",
                                    Value = "On",
                                },
                                new V1EnvVar
                                {
                                    Name = "ON",
                                    Value = "ON",
                                },
                                new V1EnvVar
                                {
                                    Name = "off",
                                    Value = "off",
                                },
                                new V1EnvVar
                                {
                                    Name = "Off",
                                    Value = "Off",
                                },
                                new V1EnvVar
                                {
                                    Name = "OFF",
                                    Value = "OFF",
                                },
                            },
                            Name = "foo",
                        },
                    },
                },
            };

            var objStr = KubernetesYaml.Serialize(pod);
            Assert.Equal(content.Replace("\r\n", "\n"), objStr.Replace("\r\n", "\n"));
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

            var result = KubernetesYaml.Deserialize<V1Secret>(kManifest);
            Assert.Equal("bXktYXBw", Encoding.UTF8.GetString(result.Data["username"]));
            Assert.Equal("Mzk1MjgkdmRnN0pi", Encoding.UTF8.GetString(result.Data["password"]));
        }

        [Fact]
        public void DeserializeWithJsonPropertyName()
        {
            var kManifest = @"
apiVersion: apiextensions.k8s.io/v1
kind: CustomResourceDefinition
metadata:
  name: test-crd
spec:
  group: test.crd
  names:
    kind: Crd
    listKind: CrdList
    plural: crds
    singular: crd
  scope: Namespaced
  versions:
  - name: v1alpha1
    schema:
      openAPIV3Schema:
        description: This is a test crd.
        x-kubernetes-int-or-string: true
        required:
        - metadata
        - spec
        type: object
    served: true
    storage: true
";
            var result = KubernetesYaml.Deserialize<V1CustomResourceDefinition>(kManifest);
            Assert.Single(result?.Spec?.Versions);
            var ver = result.Spec.Versions[0];
            Assert.Equal(true, ver?.Schema?.OpenAPIV3Schema?.XKubernetesIntOrString);
        }
    }
}
