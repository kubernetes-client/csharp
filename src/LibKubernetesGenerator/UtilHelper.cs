using System.Collections.Generic;
using System.Linq;
using NSwag;
using Nustache.Core;

namespace LibKubernetesGenerator
{
    internal class UtilHelper : INustacheHelper
    {
        public void RegisterHelper()
        {
            Helpers.Register(nameof(IfKindIs), IfKindIs);
        }

        public static void IfKindIs(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var parameter = arguments?.FirstOrDefault() as OpenApiParameter;
            if (parameter != null)
            {
                string kind = null;
                if (arguments.Count > 1)
                {
                    kind = arguments[1] as string;
                }

                if (kind == "query" && parameter.Kind == OpenApiParameterKind.Query)
                {
                    fn(null);
                }
                else if (kind == "path" && parameter.Kind == OpenApiParameterKind.Path)
                {
                    fn(null);
                }
            }
        }
    }
}
