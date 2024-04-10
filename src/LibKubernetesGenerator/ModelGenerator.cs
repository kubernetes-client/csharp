using Microsoft.CodeAnalysis;
using NSwag;

namespace LibKubernetesGenerator
{
    internal class ModelGenerator
    {
        private readonly ClassNameHelper classNameHelper;
        private readonly ScriptObjectFactory scriptObjectFactory;

        public ModelGenerator(ClassNameHelper classNameHelper, ScriptObjectFactory scriptObjectFactory)
        {
            this.classNameHelper = classNameHelper;
            this.scriptObjectFactory = scriptObjectFactory;
        }

        public void Generate(OpenApiDocument swagger, IncrementalGeneratorPostInitializationContext context)
        {
            var sc = scriptObjectFactory.CreateScriptObject();


            foreach (var kv in swagger.Definitions)
            {
                var def = kv.Value;
                var clz = classNameHelper.GetClassNameForSchemaDefinition(def);

                sc.SetValue("clz", clz, true);
                sc.SetValue("def", def, true);
                sc.SetValue("properties", def.Properties.Values, true);

                context.RenderToContext("Model.cs.template", sc, $"Models_{clz}.g.cs");
            }
        }
    }
}
