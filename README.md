# Kubernetes C# Client

[![Github Actions Build](https://github.com/kubernetes-client/csharp/actions/workflows/buildtest.yaml/badge.svg)](https://github.com/kubernetes-client/csharp/actions/workflows/buildtest.yaml)
[![Client Capabilities](https://img.shields.io/badge/Kubernetes%20client-Silver-blue.svg?style=flat&colorB=C0C0C0&colorA=306CE8)](http://bit.ly/kubernetes-client-capabilities-badge)
[![Client Support Level](https://img.shields.io/badge/kubernetes%20client-beta-green.svg?style=flat&colorA=306CE8)](http://bit.ly/kubernetes-client-support-badge)

# Usage

[![KubernetesClient](https://img.shields.io/nuget/v/KubernetesClient)](https://www.nuget.org/packages/KubernetesClient/)

```sh
dotnet add package KubernetesClient
```

## Generate with Visual Studio

```
dotnet msbuild /Restore /t:SlnGen kubernetes-client.proj
```

## Authentication/Configuration
You should be able to use a standard KubeConfig file with this library,
see the `BuildConfigFromConfigFile` function below. Most authentication
methods are currently supported, but a few are not, see the
[known-issues](https://github.com/kubernetes-client/csharp#known-issues).

You should also be able to authenticate with the in-cluster service
account using the `InClusterConfig` function shown below.

## Monitoring
Metrics are built in to HttpClient using System.Diagnostics.DiagnosticsSource.
https://learn.microsoft.com/en-us/dotnet/core/diagnostics/built-in-metrics-system-net

There are many ways these metrics can be consumed/exposed but that decision is up to the application, not KubernetesClient itself.
https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-collection

## Sample Code

### Creating the client
```c#
// Load from the default kubeconfig on the machine.
var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

// Load from a specific file:
var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(Environment.GetEnvironmentVariable("KUBECONFIG"));

// Load from in-cluster configuration:
var config = KubernetesClientConfiguration.InClusterConfig()

// Use the config object to create a client.
var client = new Kubernetes(config);
```

### Listing Objects
```c#
var namespaces = client.CoreV1.ListNamespace();
foreach (var ns in namespaces.Items) {
    Console.WriteLine(ns.Metadata.Name);
    var list = client.CoreV1.ListNamespacedPod(ns.Metadata.Name);
    foreach (var item in list.Items)
    {
        Console.WriteLine(item.Metadata.Name);
    }
}
```

### Creating and Deleting Objects
```c#
var ns = new V1Namespace
{
    Metadata = new V1ObjectMeta
    {
        Name = "test"
    }
};

var result = client.CoreV1.CreateNamespace(ns);
Console.WriteLine(result);

var status = client.CoreV1.DeleteNamespace(ns.Metadata.Name, new V1DeleteOptions());
```

## Examples

There is extensive example code in the [examples directory](https://github.com/kubernetes-client/csharp/tree/master/examples).

### Running the examples

```bash
git clone git@github.com:kubernetes-client/csharp.git
cd csharp\examples\simple
dotnet run
```

## Known issues

While the preferred way of connecting to a remote cluster from local machine is:

```c#
var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
var client = new Kubernetes(config);
```

Not all auth providers are supported at the moment [#91](https://github.com/kubernetes-client/csharp/issues/91#issuecomment-362920478). You can still connect to a cluster by starting the proxy command:

```bash
$ kubectl proxy
Starting to serve on 127.0.0.1:8001
```

and changing config:

```c#
var config = new KubernetesClientConfiguration {  Host = "http://127.0.0.1:8001" };
```

Notice that this is a workaround and is not recommended for production use.

## Testing

The project uses [XUnit](https://github.com/xunit/xunit) as unit testing framework.

To run the tests:

```bash
cd csharp\tests
dotnet restore
dotnet test
```

# Update the API model

## Prerequisites

You'll need a Linux machine with Docker.

Check out the generator project into some other directory
(henceforth `$GEN_DIR`).

```bash
cd $GEN_DIR/..
git clone https://github.com/kubernetes-client/gen
```

## Generating new swagger.json

```bash
# Where REPO_DIR points to the root of the csharp repository
cd
${GEN_DIR}/openapi/csharp.sh ${REPO_DIR}/src/KubernetesClient ${REPO_DIR}/csharp.settings
```

# Version Compatibility

| SDK Version | Kubernetes Version | .NET Targeting                                      |
|-------------|--------------------|-----------------------------------------------------|
| 14.0        | 1.30               | net6.0;net8.0;net48*;netstandard2.0*         |
| 13.0        | 1.29               | net6.0;net7.0;net8.0;net48*;netstandard2.0*         |
| 12.0        | 1.28               | net6.0;net7.0;net48*;netstandard2.0*                |
| 11.0        | 1.27               | net6.0;net7.0;net48*;netstandard2.0*                |
| 10.0        | 1.26               | net6.0;net7.0;net48*;netstandard2.0*                |
| 9.1         | 1.25               | netstandard2.1;net6.0;net7.0;net48*;netstandard2.0* |
| 9.0         | 1.25               | netstandard2.1;net5.0;net6.0;net48*;netstandard2.0* |
| 8.0         | 1.24               | netstandard2.1;net5.0;net6.0;net48*;netstandard2.0* |
| 7.2         | 1.23               | netstandard2.1;net5.0;net6.0;net48*;netstandard2.0* |
| 7.0         | 1.23               | netstandard2.1;net5.0;net6.0                        |
| 6.0         | 1.22               | netstandard2.1;net5.0                               |
| 5.0         | 1.21               | netstandard2.1;net5                                 |
| 4.0         | 1.20               | netstandard2.0;netstandard2.1                       |
| 3.0         | 1.19               | netstandard2.0;net452                               |
| 2.0         | 1.18               | netstandard2.0;net452                               |
| 1.6         | 1.16               | netstandard1.4;netstandard2.0;net452;               |
| 1.4         | 1.13               | netstandard1.4;net451                               |
| 1.3         | 1.12               | netstandard1.4;net452                               |

 * Starting from `2.0`, [dotnet sdk versioning](https://github.com/kubernetes-client/csharp/issues/400) adopted
 * `Kubernetes Version` here means the version sdk models and apis were generated from
 * Kubernetes api server guarantees the compatibility with `n-2` (`n-3` after 1.28) version. for example:
   - 1.19 based sdk should work with 1.21 cluster, but not guaranteed to work with 1.22 cluster.<br>

    and vice versa:
   - 1.21 based sdk should work with 1.19 cluster, but not guaranteed to work with 1.18 cluster.<br>
Note: in practice, the sdk might work with much older clusters, at least for the more stable functionality. However, it is not guaranteed past the `n-2` (or `n-3` after 1.28 ) version. See [#1511](https://github.com/kubernetes-client/csharp/issues/1511) for additional details.<br>

    see also <https://kubernetes.io/releases/version-skew-policy/>
 * Fixes (including security fixes) are not back-ported automatically to older sdk versions. However, contributions from the community are welcomed 😊; See [Contributing](#contributing) for instructions on how to contribute.
 * `*` `KubernetesClient.Classic`: netstandard2.0 and net48 are supported with limited features


## Contributing

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for instructions on how to contribute.
