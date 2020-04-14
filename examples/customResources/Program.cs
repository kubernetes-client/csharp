using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using k8s;
using k8s.CustomResources;
using k8s.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace customResources
{
    class Program
    {
        static async Task Main(string[] args)
        {

            try
            {
                var config = KubernetesClientConfiguration.BuildDefaultConfig();
                var client = new Kubernetes(config);
                // var kubernetesVersionResponse = await client.GetCodeAsync();
                // var kubernetesServerVersion = kubernetesVersionResponse.ToVersion();
                var registration = V1CustomResourceDefinition.Builder
                    .SetScope(Scope.Cluster)
                    .AddVersion<MyCustomResource>()
                        .IsServe()
                        .IsStore()
                        .EnableScaleSubresource(x => x.Spec.Replicas, x => x.Status.Replicas)
                    .Build();
                Console.WriteLine("Installing CRD...");

                await client.InstallCustomResourceDefinition(registration);
                Console.WriteLine("CRD installed!");

                var crd = new MyCustomResource("plumbus")
                {
                    Spec = new MyCustomResourceSpec
                    {
                        Item = new List<MyCustomResourceItem>()
                        {
                            new MyCustomResourceItem{ Name = "Foo", Value = "Bar"},
                        },
                        Replicas = 2,
                        Description = "Some Description"
                    }
                };
                var crdMetadata = crd.GetKubernetesTypeMetadata();
                crd.Validate();
                try
                {
                    await client.CreateClusterCustomObjectAsync(crd, crdMetadata.Group, crdMetadata.ApiVersion, crdMetadata.PluralName);
                    Console.WriteLine($"Created a new instance of CRD {crd.ApiVersion} with name {crd.Metadata.Name} ");
                }
                catch (HttpOperationException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"CRD with name {crd.Metadata.Name} already exists");
                }

                Console.WriteLine("Try the following commands:");
                Console.WriteLine("kubectl get customresourcedefinition mycustomresources.stakhov.pro");
                Console.WriteLine("kubectl get mcr");
                Console.WriteLine("Press ENTER to uninstall");
                Console.ReadLine();
                await client.DeleteClusterCustomObjectAsync(crdMetadata.Group, crdMetadata.ApiVersion, crdMetadata.PluralName, crd.Metadata.Name);
                await client.UnInstallCustomResourceDefinition(registration);
                Console.WriteLine("CRD uninstalled!");
            }
            catch (HttpOperationException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("==============Request==================");
                Console.WriteLine(JToken.Parse(e.Request.Content).ToString(Formatting.Indented));
                Console.WriteLine("==============Response==================");
                Console.WriteLine(JToken.Parse(e.Response.Content).ToString(Formatting.Indented));
            }
        }
    }


}
