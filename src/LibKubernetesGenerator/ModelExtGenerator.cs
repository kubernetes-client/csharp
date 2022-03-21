using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSwag;

namespace LibKubernetesGenerator
{
    internal class ModelExtGenerator
    {
        private readonly ClassNameHelper classNameHelper;

        public ModelExtGenerator(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void Generate(OpenApiDocument swagger, GeneratorExecutionContext context)
        {
            // Generate the interface declarations
            var skippedTypes = new HashSet<string> { "V1WatchEvent" };

            var definitions = swagger.Definitions.Values
                .Where(
                    d => d.ExtensionData != null
                         && d.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                         && !skippedTypes.Contains(classNameHelper.GetClassName(d)));

            context.RenderToContext("ModelExtensions.cs.template", definitions, "ModelExtensions.g.cs");
        }
    }
}
