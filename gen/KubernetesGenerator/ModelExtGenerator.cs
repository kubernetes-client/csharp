using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class ModelExtGenerator
    {
        private readonly ClassNameHelper classNameHelper;

        public ModelExtGenerator(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void Generate(OpenApiDocument swagger, string outputDirectory)
        {
            // Generate the interface declarations
            var skippedTypes = new HashSet<string> { "V1WatchEvent" };

            var definitions = swagger.Definitions.Values
                .Where(
                    d => d.ExtensionData != null
                         && d.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                         && !skippedTypes.Contains(classNameHelper.GetClassName(d)));

            Render.FileToFile(Path.Combine("templates", "ModelExtensions.cs.template"), definitions,
                Path.Combine(outputDirectory, "ModelExtensions.cs"));
        }
    }
}
