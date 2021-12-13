# Kubernetes C# Client
[![Travis](https://img.shields.io/travis/kubernetes-client/csharp.svg)](https://travis-ci.org/kubernetes-client/csharp)
[![Client Capabilities](https://img.shields.io/badge/Kubernetes%20client-Silver-blue.svg?style=flat&colorB=C0C0C0&colorA=306CE8)](http://bit.ly/kubernetes-client-capabilities-badge)
[![Client Support Level](https://img.shields.io/badge/kubernetes%20client-beta-green.svg?style=flat&colorA=306CE8)](http://bit.ly/kubernetes-client-support-badge)

# Usage
[Nuget Package](https://www.nuget.org/packages/KubernetesClient/)

```sh
dotnet add package KubernetesClient
```

## Authentication/Configuration
You should be able to use a standard KubeConfig file with this library,
see the `BuildConfigFromConfigFile` function below. Most authentication
methods are currently supported, but a few are not, see the 
[known-issues](https://github.com/kubernetes-client/csharp#known-issues).

You should also be able to authenticate with the in-cluster service
account using the `InClusterConfig` function shown below.

## Monitoring
There is optional built-in metric generation for prometheus client metrics.
The exported metrics are:

* `k8s_dotnet_request_total` - Counter of request, broken down by HTTP Method
* `k8s_dotnet_response_code_total` - Counter of responses, broken down by HTTP Method and response code
* `k8s_request_latency_seconds` - Latency histograms broken down by method, api group, api version and resource kind

There is an example integrating these monitors in the examples/prometheus directory.

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
var namespaces = client.ListNamespace();
foreach (var ns in namespaces.Items) {
    Console.WriteLine(ns.Metadata.Name);
    var list = client.ListNamespacedPod(ns.Metadata.Name);
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

var result = client.CreateNamespace(ns);
Console.WriteLine(result);

var status = client.DeleteNamespace(ns.Metadata.Name, new V1DeleteOptions());
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

Not all auth providers are supported at moment [#91](https://github.com/kubernetes-client/csharp/issues/91#issuecomment-362920478). You can still connect to a cluster by starting the proxy command:

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

The project uses [XUnit](https://xunit.github.io) as unit testing framework.

To run the tests:

```bash
cd csharp\tests
dotnet restore
dotnet test
```

# Generating the Client Code

## Prerequisites

You'll need a Linux machine with Docker.

The generated code works on all platforms supported by .NET or .NET Core.

Check out the generator project into some other directory
(henceforth `$GEN_DIR`).

```bash
cd $GEN_DIR/..
git clone https://github.com/kubernetes-client/gen
```

## Generating code

```bash
# Where REPO_DIR points to the root of the csharp repository
cd ${REPO_DIR}/csharp/src/KubernetesClient
${GEN_DIR}/openapi/csharp.sh generated ../csharp.settings
```

# Version Compatibility 

| SDK Version | Kubernetes Version | .NET Targeting                        |
|-------------|--------------------|---------------------------------------|
| 7.0         | 1.23               | netstandard2.1;net5;net6              |
| 6.0         | 1.22               | netstandard2.1;net5                   |
| 5.0         | 1.21               | netstandard2.1;net5                   |
| 4.0         | 1.20               | netstandard2.0;netstandard2.1         |
| 3.0         | 1.19               | netstandard2.0;net452                 |
| 2.0         | 1.18               | netstandard2.0;net452                 |
| 1.6         | 1.16               | netstandard1.4;netstandard2.0;net452; |
| 1.4         | 1.13               | netstandard1.4;net451                 |
| 1.3         | 1.12               | netstandard1.4;net452                 |

 * Starting from `2.0`, [dotnet sdk versioning](https://github.com/kubernetes-client/csharp/issues/400) adopted
 * `Kubernetes Version` here means the version sdk models and apis were generated from
 * Kubernetes api server guarantees the compatibility with `n-2` version. for exmaple, 1.19 based sdk should work with 1.21 cluster, but no guarantee works with 1.22 cluster. see also <https://kubernetes.io/releases/version-skew-policy/>


## Contributing

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for instructions on how to contribute.
