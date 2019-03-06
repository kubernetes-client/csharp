# Kubernetes C# Client
[![Travis](https://img.shields.io/travis/kubernetes-client/csharp.svg)](https://travis-ci.org/kubernetes-client/csharp)
[![Client Capabilities](https://img.shields.io/badge/Kubernetes%20client-Silver-blue.svg?style=flat&colorB=C0C0C0&colorA=306CE8)](http://bit.ly/kubernetes-client-capabilities-badge)
[![Client Support Level](https://img.shields.io/badge/kubernetes%20client-beta-green.svg?style=flat&colorA=306CE8)](http://bit.ly/kubernetes-client-support-badge)

# Usage
[Nuget Package](https://www.nuget.org/packages/KubernetesClient/)

```sh
dotnet add package KubernetesClient
```

# Usage

## Authentication/Configuration
You should be able to use an standard KubeConfig file with this library,
see the `BuildConfigFromConfigFile` function below. Most authentication
methods are currently supported, but a few are not, see the 
[known-issues](https://github.com/kubernetes-client/csharp#known-issues)

You should also be able to authenticate using the in-cluster service
account using the `InClusterConfig` function shown below.

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

### Running the Examples

```bash
git clone git@github.com:kubernetes-client/csharp.git
cd csharp\examples\simple
dotnet run
```

## Known issues

While preferred way of connecting to a remote cluster from local machine is:

```
var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
var client = new Kubernetes(config);
```

Not all auth providers are supported at moment [#91](https://github.com/kubernetes-client/csharp/issues/91#issuecomment-362920478), but you still can connect to cluster by starting proxy:

```bash
$ kubectl proxy
Starting to serve on 127.0.0.1:8001
```

and changing config:

```csharp
var config = new KubernetesClientConfiguration {  Host = "http://127.0.0.1:8001" };
```

Notice that this is a workaround and is not recommended for production use

## Testing

The project uses [XUnit](https://xunit.github.io) as unit testing framework.

To run the tests

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
(henceforth `$GEN_DIR`)

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

## Contributing

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for instructions on how to contribute.
