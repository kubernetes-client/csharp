namespace simple
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using k8s;
    using k8s.Models;

    class NamespaceExample
    {
        static void ListNamespaces(IKubernetes client) {
            var listTask = client.ListNamespaceWithHttpMessagesAsync().Result;
            var list = listTask.Body;
            foreach (var item in list.Items) {
                Console.WriteLine(item.Metadata.Name);
            }
            if (list.Items.Count == 0) {
                Console.WriteLine("Empty!");
            }
        }

        static async Task asyncAwaitDelete(IKubernetes client, string name, int delayMillis) {
            while (true) {
                await Task.Delay(delayMillis);
                try {
                    var result = await client.ReadNamespaceWithHttpMessagesAsync(name);
                    var ns = result.Body;
                    Console.WriteLine(ns);
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

        static void awaitDelete(IKubernetes client, string name, int delayMillis) {
            asyncAwaitDelete(client, name, delayMillis).Wait();
        }

        static void Main(string[] args)
        {
            var k8sClientConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(k8sClientConfig);
            
            ListNamespaces(client);

            var ns = new Corev1Namespace();
            ns.Metadata = new V1ObjectMeta();
            ns.Metadata.Name = "test";

            var result = client.CreateNamespaceWithHttpMessagesAsync(ns).Result;
            Console.WriteLine(result);

            ListNamespaces(client);

            var task = client.DeleteNamespaceWithHttpMessagesAsync(new V1DeleteOptions(), ns.Metadata.Name);
            var obj = ObjectOrStatus<Corev1Namespace>.ReadObjectOrStatus(task);
            
            if (obj.Status != null) {
                Console.WriteLine(obj.Status);
            } else {
                awaitDelete(client, ns.Metadata.Name, 3 * 1000);
            }
            ListNamespaces(client);
        }
    }
}
