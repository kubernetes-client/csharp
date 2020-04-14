using System;
using System.Linq;
using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace informers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x => x.AddConsole())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddKubernetesClient(KubernetesClientConfiguration.BuildDefaultConfig);
                    services.AddKubernetesInformers();

                    services.AddHostedService<ControllerService>();
                    services.AddSingleton<IController, DeltaChangesQueryingController>();
                });
    }
}
