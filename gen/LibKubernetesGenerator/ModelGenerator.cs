using System.IO;
using Microsoft.CodeAnalysis;
using NSwag;
using Nustache.Core;

namespace LibKubernetesGenerator
{
    internal class ModelGenerator
    {
        private readonly ClassNameHelper classNameHelper;

        public ModelGenerator(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void Generate(OpenApiDocument swagger, GeneratorExecutionContext context)
        {
            foreach (var kv in swagger.Definitions)
            {
                var def = kv.Value;
                var clz = classNameHelper.GetClassNameForSchemaDefinition(def);
                context.RenderToContext(
                    "Model.cs.template",
                    new { clz, def, properties = def.Properties.Values },
                    $"Models_{clz}.g.cs");
            }
        }
    }
}
