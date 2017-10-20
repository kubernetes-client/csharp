# Kubernetes C# Client
Work In Progress

[![Travis](https://img.shields.io/travis/kubernetes-client/csharp.svg)](https://travis-ci.org/kubernetes-client/csharp)

# Usage
[Nuget Package](https://www.nuget.org/packages/KubernetesClient/0.1.0-beta)

```sh
dotnet add package KubernetesClient
```

# Generating the Client Code

## Prerequisites

Check out the generator project into some other directory
(henceforth `$GEN_DIR`)

```bash
cd $GEN_DIR/..
git clone https://github.com/kubernetes-client/gen
```

Install the [`autorest` tool](https://github.com/azure/autorest):

```bash
npm install autorest
```

## Generating code

```bash
# Where REPO_DIR points to the root of the csharp repository
cd ${REPO_DIR}/csharp/src
${GEN_DIR}/openapi/csharp.sh generated csharp.settings
```

# Usage

## Running the Examples

```bash
git clone git@github.com:kubernetes-client/csharp.git
cd csharp\examples\simple
dotnet run
```

## Testing

The project uses [XUnit](https://xunit.github.io) as unit testing framework.

To run the tests

```bash
cd csharp\tests
dotnet restore
dotnet test
```
