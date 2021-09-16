using System;
using System.Threading;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using k8s.Util.Common;
using k8s.Util.Common.Generic;

namespace GenericKubernetesApiExample
{
    public class Program
    {
        private static GenericKubernetesApi _genericKubernetesApi;

        public static void Main(string[] args)
        {
            var config = KubernetesClientConfiguration.BuildDefaultConfig();
            IKubernetes client = new Kubernetes(config);
            var cts = new CancellationTokenSource();

            _genericKubernetesApi = new GenericKubernetesApi(
                apiGroup: "pod",
                apiVersion: "v1",
                resourcePlural: "pods",
                apiClient: client);

            var aPod = GetNamespacedPod(Namespaces.NamespaceDefault, "my-pod-name", cts.Token);
            var aListOfPods = ListPodsInNamespace(Namespaces.NamespaceDefault, cts.Token);

            // Watch for pod actions in a namespsace
            using var watch = _genericKubernetesApi.Watch<V1Pod>(
                Namespaces.NamespaceDefault,
                (eventType, pod) => { Console.WriteLine("The event {0} happened on pod named {1}", eventType, pod.Metadata.Name); },
                exception => { Console.WriteLine("Oh no! An exception happened while watching pods. The message was '{0}'.", exception.Message); },
                () => { Console.WriteLine("The server closed the connection."); });

            Console.WriteLine("press ctrl + c to stop watching");

            var ctrlc = new ManualResetEventSlim(false);
            Console.CancelKeyPress += (sender, eventArgs) => ctrlc.Set();
            ctrlc.Wait();
            cts.Cancel();
        }

        private static V1Pod GetNamespacedPod(string @namespace, string podName, CancellationToken cancellationToken)
        {
            var resp = Task.Run(
                async () => await _genericKubernetesApi.GetAsync<V1Pod>(@namespace, podName, cancellationToken).ConfigureAwait(false), cancellationToken);

            return resp.Result;
        }

        private static V1PodList ListPodsInNamespace(string @namespace, CancellationToken cancellationToken)
        {
            var resp = Task.Run(
                async () => await _genericKubernetesApi.ListAsync<V1PodList>(@namespace, cancellationToken).ConfigureAwait(false), cancellationToken);

            return resp.Result;
        }
    }
}
