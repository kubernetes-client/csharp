using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace yaml
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var typeMap = new Dictionary<String, Type>
            {
                { "v1/Pod", typeof(V1Pod) },
                { "v1/Service", typeof(V1Service) },
                { "apps/v1/Deployment", typeof(V1Deployment) }
            };

            var objects = await KubernetesYaml.LoadAllFromFileAsync(args[0], typeMap);

            foreach (var obj in objects)
            {
                Console.WriteLine(obj);
            }
        }
    }
}
