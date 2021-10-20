using System.IO;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class ModelGenerator
    {
        private readonly ClassNameHelper classNameHelper;

        public ModelGenerator(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void Generate(OpenApiDocument swaggercooked, string outputDirectory)
        {
            Directory.CreateDirectory(Path.Combine(outputDirectory, "Models"));

            foreach (var (_, def) in swaggercooked.Definitions)
            {
                var clz = classNameHelper.GetClassNameForSchemaDefinition(def);
                Render.FileToFile(Path.Combine("templates", "Model.cs.template"),
                    new { clz, def, properties = def.Properties.Values },
                    Path.Combine(outputDirectory, "Models", $"{clz}.cs"));
            }
        }
    }
}
