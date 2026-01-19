using k8s.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
        private record MyPod : V1Pod
        {
        }
#pragma warning restore CA1812

        [Fact]
        public void LoadAllFromStringWithTypes()
        {
            var types = new Dictionary<string, Type>
            {
                { "v1/Pod", typeof(MyPod) },
            };

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

            Assert.Throws<YamlDotNet.Core.YamlException>(() => KubernetesYaml.LoadAllFromString(content, strict: true));
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
            var types = new Dictionary<string, Type>
            {
                { "v1/Pod", typeof(MyPod) },
            };

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

            Assert.Throws<YamlDotNet.Core.YamlException>(() => KubernetesYaml.LoadAllFromString(content, strict: true));
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
                await File.WriteAllTextAsync(tempFileName, content).ConfigureAwait(true);

                var objs = await KubernetesYaml.LoadAllFromFileAsync(tempFileName).ConfigureAwait(true);
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
            var types = new Dictionary<string, Type>
            {
                { "v1/Pod", typeof(MyPod) },
            };

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
                await File.WriteAllTextAsync(tempFileName, content).ConfigureAwait(true);

                var objs = await KubernetesYaml.LoadAllFromFileAsync(tempFileName, types).ConfigureAwait(true);
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
            Assert.Throws<YamlDotNet.Core.YamlException>(() => KubernetesYaml.Deserialize<V1Pod>(content, strict: true));
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
            Assert.Throws<YamlDotNet.Core.YamlException>(() => KubernetesYaml.Deserialize<V1Pod>(content, strict: true));
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

            var obj = KubernetesYaml.Deserialize<V1Pod>(content, true);

            Assert.True(obj.Spec.Containers[0].VolumeMounts[0].ReadOnlyProperty);
            Assert.False(obj.Spec.Containers[0].VolumeMounts[1].ReadOnlyProperty);
        }

        [Fact]
        public async Task LoadFromStream()
        {
            var content = @"apiVersion: v1
kind: Pod
metadata:
  name: foo
";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var obj = await KubernetesYaml.LoadFromStreamAsync<V1Pod>(stream).ConfigureAwait(true);

            Assert.Equal("foo", obj.Metadata.Name);
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
                await File.WriteAllTextAsync(tempFileName, content).ConfigureAwait(true);

                var obj = await KubernetesYaml.LoadFromFileAsync<V1Pod>(tempFileName).ConfigureAwait(true);
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
            using var reader = new StringReader(s);

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

            var obj = KubernetesYaml.Deserialize<V1Pod>(content, true);

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
                Metadata = new V1ObjectMeta { Name = "test-svc", Labels = labels },
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
data:
  username: YlhrdFlYQnc=
  password: TXprMU1qZ2tkbVJuTjBwaQ==
kind: Secret
metadata:
  name: test-secret
";

            var result = KubernetesYaml.Deserialize<V1Secret>(kManifest, true);
            Assert.Equal("bXktYXBw", Encoding.UTF8.GetString(result.Data["username"]));
            Assert.Equal("Mzk1MjgkdmRnN0pi", Encoding.UTF8.GetString(result.Data["password"]));
        }

        [Fact]
        public void WriteSecret()
        {
            var kManifest = """
apiVersion: v1
kind: Secret
metadata:
  name: test-secret
data:
  username: YlhrdFlYQnc=
  password: TXprMU1qZ2tkbVJuTjBwaQ==
""";

            var result = KubernetesYaml.Deserialize<V1Secret>(kManifest, true);
            var yaml = KubernetesYaml.Serialize(result);

            Assert.Equal(kManifest, yaml);
        }

        [Fact]
        public void LoadConfigMap()
        {
            var kManifest = @"
apiVersion: v1
binaryData:
  username: YlhrdFlYQnc=
data:
  password: Mzk1MjgkdmRnN0pi
kind: ConfigMap
metadata:
  name: test-configmap
";

            var result = KubernetesYaml.Deserialize<V1ConfigMap>(kManifest, true);
            Assert.Equal("bXktYXBw", Encoding.UTF8.GetString(result.BinaryData["username"]));
            Assert.Equal("Mzk1MjgkdmRnN0pi", result.Data["password"]);
        }

        [Fact]
        public void WriteConfigMap()
        {
            var kManifest = """
apiVersion: v1
kind: ConfigMap
metadata:
  name: test-configmap
binaryData:
  username: YlhrdFlYQnc=
data:
  password: Mzk1MjgkdmRnN0pi
""";

            var result = KubernetesYaml.Deserialize<V1ConfigMap>(kManifest, true);
            var yaml = KubernetesYaml.Serialize(result);

            Assert.Equal(kManifest, yaml);
        }

        [Fact]
        public void WriteConfigMapOrdersTypeMetaFirst()
        {
            var configMap = new V1ConfigMap
            {
                ApiVersion = "v1",
                Kind = "ConfigMap",
                Metadata = new V1ObjectMeta { Name = "my-configmap", NamespaceProperty = "my-namespace" },
                Data = new Dictionary<string, string> { { "array.json", "value" } },
            };

            var yaml = KubernetesYaml.Serialize(configMap);

            Assert.Equal(ToLines(@"apiVersion: v1
kind: ConfigMap
metadata:
  name: my-configmap
  namespace: my-namespace
data:
  array.json: value"), ToLines(yaml));
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

#pragma warning disable CA1812 // Class is used for YAML deserialization tests
        [KubernetesEntity(Group = KubeGroup, Kind = KubeKind, ApiVersion = KubeApiVersion, PluralName = KubePluralName)]
        private sealed class V1AlphaFoo : IKubernetesObject<V1ObjectMeta>, ISpec<Dictionary<string, object>>
        {
            public const string KubeApiVersion = "v1alpha";
            public const string KubeKind = "foo";
            public const string KubeGroup = "foo.bar";
            public const string KubePluralName = "foos";

            public string ApiVersion { get; set; }
            public string Kind { get; set; }
            public V1ObjectMeta Metadata { get; set; }
            public Dictionary<string, object> Spec { get; set; }

            public V1AlphaFoo()
            {
                Metadata = new V1ObjectMeta();
                Spec = new Dictionary<string, object>();
            }
        }
#pragma warning restore CA1812 // Class is used for YAML deserialization tests

        [Fact]
        public void LoadAllFromStringWithTypeMapGenericCRD()
        {
            var content = @"apiVersion: foo.bar/v1alpha
kind: Foo
metadata:
  name: foo
  namespace: ns
spec:
  bool: false
  byte: 123
  float: 12.0
";

            var objs = KubernetesYaml.LoadAllFromString(content, new Dictionary<string, Type>
            {
                { $"{V1AlphaFoo.KubeGroup}/{V1AlphaFoo.KubeApiVersion}/Foo", typeof(V1AlphaFoo) },
            }, true);
            Assert.Single(objs);
            var v1AlphaFoo = Assert.IsType<V1AlphaFoo>(objs[0]);
            Assert.Equal("foo", v1AlphaFoo.Metadata.Name);
            Assert.Equal("ns", v1AlphaFoo.Metadata.NamespaceProperty);
            Assert.Equal(3, v1AlphaFoo.Spec.Count);
            Assert.False(Assert.IsType<bool>(v1AlphaFoo.Spec["bool"]));
            Assert.Equal(123, Assert.IsType<byte>(v1AlphaFoo.Spec["byte"]));
            Assert.Equal(12.0, Assert.IsType<float>(v1AlphaFoo.Spec["float"]), 3);
            Assert.Equal(content.Replace("\r\n", "\n"), KubernetesYaml.SerializeAll(objs).Replace("\r\n", "\n"));
        }

        [Fact]
        public void LoadFromStringCRDMerge()
        {
            var content = @"apiVersion: apiextensions.k8s.io/v1
kind: CustomResourceDefinition
metadata:
  labels:
    eventing.knative.dev/source: ""true""
    duck.knative.dev/source: ""true""
    knative.dev/crd-install: ""true""
    app.kubernetes.io/version: ""1.10.1""
    app.kubernetes.io/name: knative-eventing
  annotations:
    # TODO add schemas and descriptions
    registry.knative.dev/eventTypes: |
      [
        { ""type"": ""dev.knative.sources.ping"" }
      ]
  name: pingsources.sources.knative.dev
spec:
  group: sources.knative.dev
  versions:
    - &version
      name: v1beta2
      served: true
      storage: false
      subresources:
        status: {}
      schema:
        openAPIV3Schema:
          type: object
          description: 'PingSource describes an event source with a fixed payload produced on a specified cron schedule.'
          properties:
            spec:
              type: object
              description: 'PingSourceSpec defines the desired state of the PingSource (from the client).'
              properties:
                ceOverrides:
                  description: 'CloudEventOverrides defines overrides to control the output format and modifications of the event sent to the sink.'
                  type: object
                  properties:
                    extensions:
                      description: 'Extensions specify what attribute are added or overridden on the outbound event. Each `Extensions` key-value pair are set on the event as an attribute extension independently.'
                      type: object
                      additionalProperties:
                        type: string
                      x-kubernetes-preserve-unknown-fields: true
                contentType:
                  description: 'ContentType is the media type of `data` or `dataBase64`. Default is empty.'
                  type: string
                data:
                  description: 'Data is data used as the body of the event posted to the sink. Default is empty. Mutually exclusive with `dataBase64`.'
                  type: string
                dataBase64:
                  description: ""DataBase64 is the base64-encoded string of the actual event's body posted to the sink. Default is empty. Mutually exclusive with `data`.""
                  type: string
                schedule:
                  description: 'Schedule is the cron schedule. Defaults to `* * * * *`.'
                  type: string
                sink:
                  description: 'Sink is a reference to an object that will resolve to a uri to use as the sink.'
                  type: object
                  properties:
                    ref:
                      description: 'Ref points to an Addressable.'
                      type: object
                      properties:
                        apiVersion:
                          description: 'API version of the referent.'
                          type: string
                        kind:
                          description: 'Kind of the referent. More info: https://git.k8s.io/community/contributors/devel/sig-architecture/api-conventions.md#types-kinds'
                          type: string
                        name:
                          description: 'Name of the referent. More info: https://kubernetes.io/docs/concepts/overview/working-with-objects/names/#names'
                          type: string
                        namespace:
                          description: 'Namespace of the referent. More info: https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/ This is optional field, it gets defaulted to the object holding it if left out.'
                          type: string
                    uri:
                      description: 'URI can be an absolute URL(non-empty scheme and non-empty host) pointing to the target or a relative URI. Relative URIs will be resolved using the base URI retrieved from Ref.'
                      type: string
                timezone:
                  description: 'Timezone modifies the actual time relative to the specified timezone. Defaults to the system time zone. More general information about time zones: https://www.iana.org/time-zones List of valid timezone values: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones'
                  type: string
            status:
              type: object
              description: 'PingSourceStatus defines the observed state of PingSource (from the controller).'
              properties:
                annotations:
                  description: 'Annotations is additional Status fields for the Resource to save some additional State as well as convey more information to the user. This is roughly akin to Annotations on any k8s resource, just the reconciler conveying richer information outwards.'
                  type: object
                  x-kubernetes-preserve-unknown-fields: true
                ceAttributes:
                  description: 'CloudEventAttributes are the specific attributes that the Source uses as part of its CloudEvents.'
                  type: array
                  items:
                    type: object
                    properties:
                      source:
                        description: 'Source is the CloudEvents source attribute.'
                        type: string
                      type:
                        description: 'Type refers to the CloudEvent type attribute.'
                        type: string
                conditions:
                  description: 'Conditions the latest available observations of a resource''s current state.'
                  type: array
                  items:
                    type: object
                    required:
                      - type
                      - status
                    properties:
                      lastTransitionTime:
                        description: 'LastTransitionTime is the last time the condition transitioned from one status to another. We use VolatileTime in place of metav1.Time to exclude this from creating equality.Semantic differences (all other things held constant).'
                        type: string
                      message:
                        description: 'A human readable message indicating details about the transition.'
                        type: string
                      reason:
                        description: 'The reason for the condition''s last transition.'
                        type: string
                      severity:
                        description: 'Severity with which to treat failures of this type of condition. When this is not specified, it defaults to Error.'
                        type: string
                      status:
                        description: 'Status of the condition, one of True, False, Unknown.'
                        type: string
                      type:
                        description: 'Type of condition.'
                        type: string
                observedGeneration:
                  description: 'ObservedGeneration is the ""Generation"" of the Service that was last processed by the controller.'
                  type: integer
                  format: int64
                sinkUri:
                  description: 'SinkURI is the current active sink URI that has been configured for the Source.'
                  type: string
      additionalPrinterColumns:
        - name: Sink
          type: string
          jsonPath: .status.sinkUri
        - name: Schedule
          type: string
          jsonPath: .spec.schedule
        - name: Age
          type: date
          jsonPath: .metadata.creationTimestamp
        - name: Ready
          type: string
          jsonPath: "".status.conditions[?(@.type=='Ready')].status""
        - name: Reason
          type: string
          jsonPath: "".status.conditions[?(@.type=='Ready')].reason""
    - !!merge <<: *version
      name: v1
      served: true
      storage: true
      # v1 schema is identical to the v1beta2 schema
  names:
    categories:
      - all
      - knative
      - sources
    kind: PingSource
    plural: pingsources
    singular: pingsource
  scope: Namespaced
  conversion:
    strategy: Webhook
    webhook:
      conversionReviewVersions: [""v1"", ""v1beta1""]
      clientConfig:
        service:
          name: eventing-webhook
          namespace: knative-eventing
";

            var obj = KubernetesYaml.Deserialize<V1CustomResourceDefinition>(content);

            Assert.Equal("pingsources.sources.knative.dev", obj.Metadata.Name);
            Assert.Equal("v1beta2", obj.Spec.Versions[0].Name);
            Assert.Equal("v1", obj.Spec.Versions[1].Name);

            var obj2 = KubernetesYaml.LoadAllFromString(content);

            var crd = obj2[0] as V1CustomResourceDefinition;

            Assert.Equal("pingsources.sources.knative.dev", crd.Metadata.Name);
            Assert.Equal("v1beta2", crd.Spec.Versions[0].Name);
            Assert.Equal("v1", crd.Spec.Versions[1].Name);
        }

        [Fact]
        public void NoGlobalization()
        {
            var old = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-fr");
                var yaml = KubernetesYaml.Serialize(new Dictionary<string, double>() { ["hello"] = 10.01 });
                Assert.Equal("hello: 10.01", yaml);
            }
            finally
            {
                CultureInfo.CurrentCulture = old;
            }
        }

        [Fact]
        public void ReadWriteDatesYaml()
        {
            var kManifest = """
            apiVersion: v1
            kind: Secret
            metadata:
              creationTimestamp: "2025-09-03T05:15:53Z"
              name: test-secret
            type: Opaque
            """;

            var objFromYaml = KubernetesYaml.Deserialize<V1Secret>(kManifest, true);
            var yamlFromObj = KubernetesYaml.Serialize(objFromYaml);

            Assert.Equal(kManifest, yamlFromObj);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1812:'KubernetesYamlTests.RfcTime' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it 'static' (Module in Visual Basic). (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1812)", Justification = "json type")]
        private class RfcTime
        {
            public DateTime Rfc3339 { get; set; }
            public DateTime Rfc3339micro { get; set; }
            public DateTime Rfc3339nano { get; set; }
            public DateTime Rfc3339nanolenient1 { get; set; }
            public DateTime Rfc3339nanolenient2 { get; set; }
            public DateTime Rfc3339nanolenient3 { get; set; }
            public DateTime Rfc3339nanolenient4 { get; set; }
            public DateTime Rfc3339nanolenient5 { get; set; }
            public DateTime Rfc3339nanolenient6 { get; set; }
            public DateTime Rfc3339nanolenient7 { get; set; }
            public DateTime Rfc3339nanolenient8 { get; set; }
            public DateTime Rfc3339nanolenient9 { get; set; }
        }

        [Fact]
        public void RFC3339()
        {
            /* go code to generate the json https://go.dev/play/p/VL95pugm6o8

            const RFC3339Micro = "2006-01-02T15:04:05.000000Z07:00"
            const RFC3339Nano = "2006-01-02T15:04:05.000000000Z07:00"

            func main() {
                t := time.Now()
                type Time struct {
                    RFC3339      string `json:"rfc3339"`
                    RFC3339Micro string `json:"rfc3339micro"`
                    RFC3339Nano  string `json:"rfc3339nano"`

                    RFC3339NanoLenient1 string `json:"rfc3339nanolenient1"`
                    RFC3339NanoLenient2 string `json:"rfc3339nanolenient2"`
                    RFC3339NanoLenient3 string `json:"rfc3339nanolenient3"`
                    RFC3339NanoLenient4 string `json:"rfc3339nanolenient4"`
                    RFC3339NanoLenient5 string `json:"rfc3339nanolenient5"`
                    RFC3339NanoLenient6 string `json:"rfc3339nanolenient6"`
                    RFC3339NanoLenient7 string `json:"rfc3339nanolenient7"`
                    RFC3339NanoLenient8 string `json:"rfc3339nanolenient8"`
                    RFC3339NanoLenient9 string `json:"rfc3339nanolenient9"`
                }
                t1 := Time{
                    RFC3339:      t.Add(45 * time.Minute).Add(12 * time.Second).Add(123456789 * time.Nanosecond).Format(time.RFC3339),
                    RFC3339Micro: t.Add(45 * time.Minute).Add(12 * time.Second).Add(123456789 * time.Nanosecond).Format(RFC3339Micro),
                    RFC3339Nano:  t.Add(24 * time.Minute).Add(56 * time.Second).Add(123456789 * time.Nanosecond).Format(RFC3339Nano),

                    RFC3339NanoLenient1: t.Add(100000000 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient2: t.Add(120000000 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient3: t.Add(123000000 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient4: t.Add(123400000 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient5: t.Add(123450000 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient6: t.Add(123456000 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient7: t.Add(123456700 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient8: t.Add(123456780 * time.Nanosecond).Format(time.RFC3339Nano),
                    RFC3339NanoLenient9: t.Add(123456789 * time.Nanosecond).Format(time.RFC3339Nano),
                }
                b, err := json.Marshal(t1)
                if err != nil {
                    fmt.Println("error:", err)
                }
                fmt.Println(string(b))
            }
            */

            var yaml = """
                rfc3339: '2009-11-10T23:45:12Z'
                rfc3339micro: '2009-11-10T23:45:12.123456Z'
                rfc3339nano: '2009-11-10T23:24:56.123456789Z'
                rfc3339nanolenient1: '2009-11-10T23:00:00.1Z'
                rfc3339nanolenient2: '2009-11-10T23:00:00.12Z'
                rfc3339nanolenient3: '2009-11-10T23:00:00.123Z'
                rfc3339nanolenient4: '2009-11-10T23:00:00.1234Z'
                rfc3339nanolenient5: '2009-11-10T23:00:00.12345Z'
                rfc3339nanolenient6: '2009-11-10T23:00:00.123456Z'
                rfc3339nanolenient7: '2009-11-10T23:00:00.1234567Z'
                rfc3339nanolenient8: '2009-11-10T23:00:00.12345678Z'
                rfc3339nanolenient9: '2009-11-10T23:00:00.123456789Z'
                """;

            var t = KubernetesYaml.Deserialize<RfcTime>(yaml);

            Assert.Equal(new DateTime(2009, 11, 10, 23, 45, 12, DateTimeKind.Utc), t.Rfc3339);

            Assert.Equal(2009, t.Rfc3339micro.Year);
            Assert.Equal(11, t.Rfc3339micro.Month);
            Assert.Equal(10, t.Rfc3339micro.Day);
            Assert.Equal(23, t.Rfc3339micro.Hour);
            Assert.Equal(45, t.Rfc3339micro.Minute);
            Assert.Equal(12, t.Rfc3339micro.Second);
            Assert.Equal(123, t.Rfc3339micro.Millisecond);

            Assert.Equal(2009, t.Rfc3339nano.Year);
            Assert.Equal(11, t.Rfc3339nano.Month);
            Assert.Equal(10, t.Rfc3339nano.Day);
            Assert.Equal(23, t.Rfc3339nano.Hour);
            Assert.Equal(24, t.Rfc3339nano.Minute);
            Assert.Equal(56, t.Rfc3339nano.Second);
            Assert.Equal(123, t.Rfc3339nano.Millisecond);

#if NET7_0_OR_GREATER
            Assert.Equal(456, t.Rfc3339micro.Microsecond);
            Assert.Equal(456, t.Rfc3339nano.Microsecond);
            Assert.Equal(700, t.Rfc3339nano.Nanosecond);

            Assert.Equal(100, t.Rfc3339nanolenient1.Millisecond);
            Assert.Equal(120, t.Rfc3339nanolenient2.Millisecond);
            Assert.Equal(123, t.Rfc3339nanolenient3.Millisecond);

            Assert.Equal(400, t.Rfc3339nanolenient4.Microsecond);
            Assert.Equal(450, t.Rfc3339nanolenient5.Microsecond);
            Assert.Equal(456, t.Rfc3339nanolenient6.Microsecond);

            Assert.Equal(456, t.Rfc3339nanolenient7.Microsecond);
            Assert.Equal(456, t.Rfc3339nanolenient8.Microsecond);
            Assert.Equal(456, t.Rfc3339nanolenient9.Microsecond);

            Assert.Equal(700, t.Rfc3339nanolenient7.Nanosecond);
            Assert.Equal(700, t.Rfc3339nanolenient8.Nanosecond);
            Assert.Equal(700, t.Rfc3339nanolenient9.Nanosecond);
#endif

        }
    }
}
