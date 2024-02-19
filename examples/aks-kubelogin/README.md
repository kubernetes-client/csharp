# AKS C# example using kubelogin + MSI

This example shows how to use the [kubelogin](https://github.com/Azure/kubelogin) to authenticate using [managed identities](https://learn.microsoft.com/en-us/entra/identity/managed-identities-azure-resources/overview) with Azure Kubernetes Service (AKS) using the C# SDK.


## Prerequisites

 - turn on AAD support for AKS, see [here](https://docs.microsoft.com/en-us/azure/aks/managed-aad)
 - create a managed identity for the AKS cluster
 - assign the managed identity the `Azure Kubernetes Service RBAC Cluster Admin` (or other RBAC permission) on the AKS cluster
 - assign the managed identity to the VM, see [here](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/qs-configure-portal-windows-vm)
 - install the [kubelogin](https://github.com/Azure/kubelogin) to your machine

## Running the code

 *You must the the code on VM with MSI*

 - Replace `server` with the address of your AKS cluster
 - Replace `clientid` with the client id of the managed identity
 - Replace `kubelogin` with the path to the kubelogin executable

```
dotnet run
```