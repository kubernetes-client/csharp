namespace simple
{
    using System;
    using System.IO;
    using k8s;

    class PodList
    {
        static void Main(string[] args)
        {
            var k8sClientConfig = KubernetesClientConfiguration.defaultConfiguration();
            if (k8sClientConfig.CurrentContext == null) {
                Console.WriteLine("No current context");
                return;
            }
            IKubernetes client = new Kubernetes(k8sClientConfig);
            Console.WriteLine("Starting Request!");
            var listTask = client.ListNamespacedPodWithHttpMessagesAsync("default").Result;
            var list = listTask.Body;
            foreach (var item in list.Items) {
                Console.WriteLine(item.Metadata.Name);
            }
            if (list.Items.Count == 0) {
                Console.WriteLine("Empty!");
            }
        }
    }
}
