using k8s;
using workerServiceDependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Load kubernetes configuration
        var kubernetesClientConfig = KubernetesClientConfiguration.BuildDefaultConfig();

        // Register Kubernetes client interface as sigleton
        services.AddSingleton<IKubernetes>(_ => new Kubernetes(kubernetesClientConfig));

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
