# csharp
Work In Progress
Currently only supported on Linux 

# Generating the client code

## Prerequisites

Check out the generator project into some other directory
(henceforth `$GEN_DIR`)

```
cd $GEN_DIR/..
git clone https://github.com/kubernetes-client/gen
```

Install the [`autorest` tool](https://github.com/azure/autorest):

```
npm install autorest
```

## Generating code

```
# Where REPO_DIR points to the root of the csharp repository
cd ${REPO_DIR}/csharp/src
${GEN_DIR}/openapi/csharp.sh generated csharp.settings
```
