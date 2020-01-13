using System;
using System.Threading;
using k8s;
using k8s.Models;
using k8s.informers;
using Newtonsoft.Json;

namespace informers
{
    internal class Program
    {
        public static void Main(string[] args)
        {
     
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(Environment.GetEnvironmentVariable("KUBECONFIG"));

            IKubernetes client = new Kubernetes(config);
            var lw = new ListerWatcher<V1Pod, V1PodList>(client);

            ISharedInformer<V1Pod, V1PodList> informer = new SharedInformer<V1Pod, V1PodList>(lw);
            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            informer.Run(cancellationToken);            
               
            while(true){
                var input = Console.ReadLine();
                Console.WriteLine("Here is the list");
                try {                                        
                    var l = informer.GetStore().List();                                        
                    Console.WriteLine("count: {0}", l.Count);
                    for (var i=0; i<l.Count; i++) {
                        var p = (V1Pod)l[i];
                        Console.WriteLine("{0}/{1}", p.Metadata.NamespaceProperty, p.Metadata.Name);
                    }
                    if (input == "a") {
                        Console.WriteLine("Adding resource handler");
                        informer.AddResourceHandlers( 
                            (type, o) => {
                                var color = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Green;
                                var json = JsonConvert.SerializeObject(o, Formatting.Indented);
                                Console.WriteLine("Action: {0}, Object: {1}", type, json);
                                Console.ForegroundColor = color;
                            },
                            (type, o) => {
                                var color = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Green;
                                var json = JsonConvert.SerializeObject(o, Formatting.Indented);
                                Console.WriteLine("Action: {0}, Object: {1}", type, json);
                                Console.ForegroundColor = color;
                            },
                            (type, old, o) => {
                                var color = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Green;
                                var json = JsonConvert.SerializeObject(o, Formatting.Indented);
                                var oldJson = JsonConvert.SerializeObject(o, Formatting.Indented);
                                Console.WriteLine("Action: {0}, \n Old Object: {1}. \n New: {2}", type, oldJson, json);
                                Console.ForegroundColor = color;
                        }
                            
                            );
                    } else if (input == "c") {
                        source.Cancel();
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                }                              
            
            }
        }           
    }
}