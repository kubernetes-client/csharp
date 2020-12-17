# Custom Resource Client Example

This example demonstrates how to use the C# Kubernetes Client library to create, get and list custom resources.

## Pre-requisits

Make sure your have added the library package

```shell
dotnet add package KubernetesClient
```

## Create Custom Resource Definition (CRD)

Make sure the  [CRD](./config/crd.yaml) is created, in order to create an instance of it after.

```shell
kubectl create -f ./crd.yaml
```

You can test that the CRD is successfully added, by creating an [instance](./config/yaml-cr-instance.yaml) of it using kubectl:

```shell
kubectl create -f ./config/yaml-cr-instance.yaml
```

```shell
kubectl get customresources.csharp.com
```

## Execute the code

The client uses the `BuildConfigFromConfigFile()` function. If the KUBECONFIG environment variable is set, then that path to the k8s config file will be used.

`dotnet run`

Expected output:

```
strating main()...
working with CRD: customresources.csharp.com
creating CR cr-instance-london
CR list:
- CR Item 0 = cr-instance-london
- CR Item 1 = cr-instance-paris
fetchedCR = cr-instance-london (Labels: {identifier : city, newKey : newValue}), Spec: London
Deleted the CR
```

## Under the hood

For more details, you can look at the Generic client [implementation](https://github.com/kubernetes-client/csharp/blob/master/src/KubernetesClient/GenericClient.cs)

