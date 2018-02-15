# Kubernetes C# Client
[![Travis](https://img.shields.io/travis/kubernetes-client/csharp.svg)](https://travis-ci.org/kubernetes-client/csharp)
[![Client Capabilities](https://img.shields.io/badge/Kubernetes%20client-Silver-blue.svg?style=flat&colorB=C0C0C0&colorA=306CE8)](http://bit.ly/kubernetes-client-capabilities-badge)
[![Client Support Level](https://img.shields.io/badge/kubernetes%20client-beta-green.svg?style=flat&colorA=306CE8)](http://bit.ly/kubernetes-client-support-badge)

# Usage
[Nuget Package](https://www.nuget.org/packages/KubernetesClient/)

```sh
dotnet add package KubernetesClient
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
cd ${REPO_DIR}/csharp/src
${GEN_DIR}/openapi/csharp.sh generated ../csharp.settings
```

# Usage

## Running the Examples

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
