using NJsonSchema;
using NSwag;
using Scriban.Runtime;
using System;
using System.Linq;

namespace LibKubernetesGenerator
{
    internal class ParamHelper : IScriptObjectHelper
    {
        private readonly GeneralNameHelper generalNameHelper;
        private readonly TypeHelper typeHelper;

        public ParamHelper(GeneralNameHelper generalNameHelper, TypeHelper typeHelper)
        {
            this.generalNameHelper = generalNameHelper;
            this.typeHelper = typeHelper;
        }

        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(GetModelCtorParam), new Func<JsonSchema, string>(GetModelCtorParam));
            scriptObject.Import(nameof(IfParamContains), IfParamContains);
        }

        public static bool IfParamContains(OpenApiOperation operation, string name)
        {
            var found = false;

            foreach (var param in operation.Parameters)
            {
                if (param.Name == name)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public string GetModelCtorParam(JsonSchema schema)
        {
            return string.Join(", ", schema.Properties.Values
                    .OrderBy(p => !p.IsRequired)
                    .Select(p =>
                    {
                        var sp =
                            $"{typeHelper.GetDotNetType(p)} {generalNameHelper.GetDotNetName(p.Name, "fieldctor")}";

                        if (!p.IsRequired)
                        {
                            sp = $"{sp} = null";
                        }

                        return sp;
                    }));
        }
    }
}
