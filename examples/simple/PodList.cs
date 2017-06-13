using System;

using k8s;

namespace simple
{
    class PodList
    {
        static void Main(string[] args)
        {
            IKubernetes client = new Kubernetes();
            client.BaseUri = new Uri("http://localhost:8001");
            var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default");
            listTask.Wait();
            var list = listTask.Result.Body;
            foreach (var item in list.Items) {
                Console.WriteLine(item.Metadata.Name);
            }
        }
    }
}
