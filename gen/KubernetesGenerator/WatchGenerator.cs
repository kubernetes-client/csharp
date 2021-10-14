using System.IO;
using System.Linq;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class WatchGenerator
    {
        public void Generate(SwaggerDocument swagger, string outputDirectory)
        {
            // Generate the Watcher operations
            // We skip operations where the name of the class in the C# client could not be determined correctly.
            // That's usually because there are different version of the same object (e.g. for deployments).
            var watchOperations = swagger.Operations.Where(
                o => o.Path.Contains("/watch/")
                     && o.Operation.ActualParameters.Any(p => p.Name == "name")).ToArray();

            // Render.
            Render.FileToFile(Path.Combine("templates", "IKubernetes.Watch.cs.template"), watchOperations,
                Path.Combine(outputDirectory, "IKubernetes.Watch.cs"));
            Render.FileToFile(Path.Combine("templates", "Kubernetes.Watch.cs.template"), watchOperations,
                Path.Combine(outputDirectory, "Kubernetes.Watch.cs"));
        }
    }
}
