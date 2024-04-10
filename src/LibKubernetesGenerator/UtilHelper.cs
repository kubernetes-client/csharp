using NSwag;
using Scriban.Runtime;

namespace LibKubernetesGenerator
{
    internal class UtilHelper : IScriptObjectHelper
    {
        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(IfKindIs), IfKindIs);
        }

        public static bool IfKindIs(OpenApiParameter parameter, string kind)
        {
            if (parameter != null)
            {
                if (kind == "query" && parameter.Kind == OpenApiParameterKind.Query)
                {
                    return true;
                }
                else if (kind == "path" && parameter.Kind == OpenApiParameterKind.Path)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
