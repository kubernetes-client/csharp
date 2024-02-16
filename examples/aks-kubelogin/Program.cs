using k8s;
using System;
using System.IO;
using System.Text;

var server = "https://example.hcp.eastus.azmk8s.io"; // the server url of your aks
var clientid = "00000000-0000-0000-0000-000000000000"; // the client id of the your msi
var kubelogin = @"C:\bin\kubelogin.exe"; // the path to the kubelogin.exe

using var configstream = new MemoryStream(Encoding.ASCII.GetBytes($"""
apiVersion: v1
clusters:
- cluster:
    insecure-skip-tls-verify: true
    server: {server}
  name: aks
contexts:
- context:
    cluster: aks
    user: msi
  name: aks
current-context: aks
kind: Config
users:
- name: msi
  user:
    exec:
      apiVersion: client.authentication.k8s.io/v1beta1
      args:
      - get-token
      - --login
      - msi
      - --server-id
      - 6dae42f8-4368-4678-94ff-3960e28e3630
      - --client-id
      - {clientid}
      command: {kubelogin}
      env: null
"""));

var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(configstream);
IKubernetes client = new Kubernetes(config);
Console.WriteLine("Starting Request!");

var list = client.CoreV1.ListNamespacedPod("default");
foreach (var item in list.Items)
{
    Console.WriteLine(item.Metadata.Name);
}
