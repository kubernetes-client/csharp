using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((builder, services) =>
{
    // Register OpenTelemetry TraceProvider
    services.AddOpenTelemetryTracing(tpb =>
    {
        var programName = Assembly.GetExecutingAssembly().GetName().Name!;
        var programVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

        tpb.AddSource(programName);
        tpb.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: programName, serviceVersion: programVersion));
        tpb.AddHttpClientInstrumentation();

        tpb.AddJaegerExporter(options =>
        {
            options.AgentHost = "http://localhost";
            options.AgentPort = 6831;
            options.Protocol = JaegerExportProtocol.UdpCompactThrift;
        });
    });

    // Register IHttpClientFactory
    services.AddHttpClient();
});

// Build the DI Container
var app = builder.Build();



// Load kubernetes configuration
var config = KubernetesClientConfiguration.BuildDefaultConfig();
// Create the httpClient
var httpClient = app.Services.GetService<IHttpClientFactory>().CreateClient();

// Create an istance of Kubernetes client
IKubernetes client = new Kubernetes(config);

// Read the list of pods contained in default namespace
var list = client.CoreV1.ListNamespacedPod("default");

// Print the name of pods
foreach (var item in list.Items)
{
    Console.WriteLine(item.Metadata.Name);
}

if (list.Items.Count == 0)
{
    Console.WriteLine("Empty!");
}
