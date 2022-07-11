using k8s;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var serviceName = "MyCompany.MyProduct.MyService";
var serviceVersion = "1.0.0";

// Create the OpenTelemetry TraceProvide with HttpClient instrumentation enabled
// NOTE: for this example telemetry will be exported to console
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddHttpClientInstrumentation()
    .AddConsoleExporter()
    .Build();

// Load kubernetes configuration
var config = KubernetesClientConfiguration.BuildDefaultConfig();

// Create an istance of Kubernetes client
IKubernetes client = new Kubernetes(config);

// Read the list of pods contained in default namespace
var list = client.CoreV1.ListNamespacedPod("default");

// Print the name of pods 
foreach (var item in list.Items)
{
    Console.WriteLine(item.Metadata.Name);
}
// Or empty if there are no pods
if (list.Items.Count == 0)
{
    Console.WriteLine("Empty!");
}
