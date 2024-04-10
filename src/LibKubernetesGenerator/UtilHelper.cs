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

        //public void RegisterHelper()
        //{
        //    Helpers.Register(nameof(IfKindIs), IfKindIs);
        //    Helpers.Register(nameof(IfListNotEmpty), IfListNotEmpty);
        //}

        //public static void IfKindIs(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
        //    RenderBlock fn, RenderBlock inverse)
        //{
        //    var parameter = arguments?.FirstOrDefault() as OpenApiParameter;
        //    if (parameter != null)
        //    {
        //        string kind = null;
        //        if (arguments.Count > 1)
        //        {
        //            kind = arguments[1] as string;
        //        }

        //        if (kind == "query" && parameter.Kind == OpenApiParameterKind.Query)
        //        {
        //            fn(null);
        //        }
        //        else if (kind == "path" && parameter.Kind == OpenApiParameterKind.Path)
        //        {
        //            fn(null);
        //        }
        //    }
        //}

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

  
        //public static void IfListNotEmpty(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
        //    RenderBlock fn, RenderBlock inverse)
        //{
        //    var parameter = arguments?.FirstOrDefault() as ObservableCollection<OpenApiParameter>;
        //    if (parameter?.Any() == true)
        //    {
        //        fn(null);
        //    }
        //}
    }
}
