using Microsoft.CodeAnalysis;
using NSwag;
using System.Collections.Generic;
using System.Linq;

namespace LibKubernetesGenerator
{
    internal class ModelExtGenerator
    {
        private readonly ClassNameHelper classNameHelper;
        private readonly ScriptObjectFactory scriptObjectFactory;

        public ModelExtGenerator(ClassNameHelper classNameHelper, ScriptObjectFactory scriptObjectFactory)
        {
            this.classNameHelper = classNameHelper;
            this.scriptObjectFactory = scriptObjectFactory;
        }

        public void Generate(OpenApiDocument swagger, IncrementalGeneratorPostInitializationContext context)
        {
            // Generate the interface declarations
            var skippedTypes = new HashSet<string> { "V1WatchEvent" };

            var definitions = swagger.Definitions.Values
                .Where(
                    d => d.ExtensionData != null
                         && d.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                         && !skippedTypes.Contains(classNameHelper.GetClassName(d)));

            var sc = scriptObjectFactory.CreateScriptObject();
            sc.SetValue("definitions", definitions, true);

            context.RenderToContext("ModelExtensions.cs.template", sc, "ModelExtensions.g.cs");
        }
    }
}
