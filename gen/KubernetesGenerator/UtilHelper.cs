using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class UtilHelper : INustacheHelper
    {
        public void RegisterHelper()
        {
            Helpers.Register(nameof(GetTuple), GetTuple);
            Helpers.Register(nameof(IfKindIs), IfKindIs);
        }

        public static void GetTuple(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] is ITuple &&
                options.TryGetValue("index", out var indexObj) && int.TryParse(indexObj?.ToString(), out var index))
            {
                var pair = (ITuple)arguments[0];
                var value = pair[index];
                context.Write(value.ToString());
            }
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
