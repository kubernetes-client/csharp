using Microsoft.CodeAnalysis;
using NSwag;

namespace LibKubernetesGenerator
{
    internal class SourceGenerationContextGenerator
    {
        private readonly ScriptObjectFactory scriptObjectFactory;

        public SourceGenerationContextGenerator(ScriptObjectFactory scriptObjectFactory)
        {
            this.scriptObjectFactory = scriptObjectFactory;
        }

        public void Generate(OpenApiDocument swagger, IncrementalGeneratorPostInitializationContext context)
        {
            var definitions = swagger.Definitions.Values;
            var sc = scriptObjectFactory.CreateScriptObject();
            sc.SetValue("definitions", definitions, true);

            context.RenderToContext("SourceGenerationContext.cs.template", sc, "SourceGenerationContext.g.cs");
        }
    }
}
