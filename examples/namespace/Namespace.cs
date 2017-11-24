using System;
using System.Net;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace @namespace
{
    class NamespaceExample
    {
        static void ListNamespaces(IKubernetes client) {
            var list = client.ListNamespace();
            foreach (var item in list.Items) {
                Console.WriteLine(item.Metadata.Name);
            }
            if (list.Items.Count == 0) {
                Console.WriteLine("Empty!");
            }
        }

        static async Task DeleteAsync(IKubernetes client, string name, int delayMillis) {
            while (true) {
                await Task.Delay(delayMillis);
                try
                {
                    await client.ReadNamespaceAsync(name);
                } catch (AggregateException ex) {
                    foreach (var innerEx in ex.InnerExceptions) {
                        if (innerEx is Microsoft.Rest.HttpOperationException) {
                            var code = ((Microsoft.Rest.HttpOperationException)innerEx).Response.StatusCode;
                            if (code == HttpStatusCode.NotFound) {
                                return;
                            }
                            throw ex;
                        }
                    }
                } catch (Microsoft.Rest.HttpOperationException ex) {
                    if (ex.Response.StatusCode == HttpStatusCode.NotFound) {
                        return;
                    }
                    throw ex;
                }
            }
        }

        static void Delete(IKubernetes client, string name, int delayMillis) {
            DeleteAsync(client, name, delayMillis).Wait();
        }

        private static void Main(string[] args)
        {
            var k8SClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(k8SClientConfig);
            
            ListNamespaces(client);

            var ns = new V1Namespace
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "test"
                }
            };

            var result = client.CreateNamespace(ns);
            Console.WriteLine(result);

            ListNamespaces(client);

            var status = client.DeleteNamespace(new V1DeleteOptions(), ns.Metadata.Name);

            if (status.HasObject)
            {
                var obj = status.ObjectView<V1Namespace>();
                Console.WriteLine(obj.Status.Phase);

                Delete(client, ns.Metadata.Name, 3 * 1000);
            }
            else
            {
                Console.WriteLine(status.Message);
            }

            ListNamespaces(client);
        }
    }
}
