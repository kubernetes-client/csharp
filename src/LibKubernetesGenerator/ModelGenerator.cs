using System.Collections.Generic;
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

            var extSkippedTypes = new HashSet<string>
            {
                "V1WatchEvent",
            };

            var typeOverrides = new Dictionary<string, string>
            {
                { "IntOrString", "class" },
                { "ResourceQuantity", "class" },
            };

            foreach (var kv in swagger.Definitions)
            {
                var def = kv.Value;
                var clz = classNameHelper.GetClassNameForSchemaDefinition(def);
                var hasExt = def.ExtensionData != null
                     && def.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                     && !extSkippedTypes.Contains(classNameHelper.GetClassName(def));


                var typ = "record";
                if (typeOverrides.TryGetValue(clz, out var to))
                {
                    typ = to;
                }

                sc.SetValue("clz", clz, true);
                sc.SetValue("def", def, true);
                sc.SetValue("properties", def.Properties.Values, true);
                sc.SetValue("typ", typ, true);
                sc.SetValue("hasExt", hasExt, true);


                context.RenderToContext("Model.cs.template", sc, $"Models_{clz}.g.cs");
            }
        }
    }
}
