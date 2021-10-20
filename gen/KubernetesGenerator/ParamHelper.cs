using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class ParamHelper : INustacheHelper
    {
        private readonly GeneralNameHelper generalNameHelper;
        private readonly TypeHelper typeHelper;

        public ParamHelper(GeneralNameHelper generalNameHelper, TypeHelper typeHelper)
        {
            this.generalNameHelper = generalNameHelper;
            this.typeHelper = typeHelper;
        }

        public void RegisterHelper()
        {
            Helpers.Register(nameof(IfParamContains), IfParamContains);
            Helpers.Register(nameof(IfParamDoesNotContain), IfParamDoesNotContain);
            Helpers.Register(nameof(GetModelCtorParam), GetModelCtorParam);
        }

        public static void IfParamContains(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var operation = arguments?.FirstOrDefault() as OpenApiOperation;
            if (operation != null)
            {
                string name = null;
                if (arguments.Count > 1)
                {
                    name = arguments[1] as string;
                }

                var found = false;

                foreach (var param in operation.Parameters)
                {
                    if (param.Name == name)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    fn(null);
                }
            }
        }

        public static void IfParamDoesNotContain(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var operation = arguments?.FirstOrDefault() as SwaggerOperation;
            if (operation != null)
            {
                string name = null;
                if (arguments.Count > 1)
                {
                    name = arguments[1] as string;
                }

                var found = false;

                foreach (var param in operation.Parameters)
                {
                    if (param.Name == name)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    fn(null);
                }
            }
        }

        public void GetModelCtorParam(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var schema = arguments[0] as JsonSchema;

            if (schema != null)
            {
                context.Write(string.Join(", ", schema.Properties.Values
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
                    })));
            }
        }
    }
}
