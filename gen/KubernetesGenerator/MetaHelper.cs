using System;
using System.Collections.Generic;
using NJsonSchema;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    internal class MetaHelper : INustacheHelper
    {
        public void RegisterHelper()
        {
            Helpers.Register(nameof(GetGroup), GetGroup);
            Helpers.Register(nameof(GetApiVersion), GetApiVersion);
            Helpers.Register(nameof(GetKind), GetKind);
            Helpers.Register(nameof(GetPathExpression), GetPathExpression);
        }

        public static void GetKind(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema)
            {
                context.Write(GetKind(arguments[0] as JsonSchema));
            }
        }

        private static string GetKind(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["kind"] as string;
        }

        public static void GetGroup(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema)
            {
                context.Write(GetGroup(arguments[0] as JsonSchema));
            }
        }

        private static string GetGroup(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["group"] as string;
        }

        public static void GetApiVersion(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema)
            {
                context.Write(GetApiVersion(arguments[0] as JsonSchema));
            }
        }

        private static string GetApiVersion(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["version"] as string;
        }

        public static void GetPathExpression(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null &&
                arguments[0] is OpenApiOperationDescription)
            {
                var operation = arguments[0] as OpenApiOperationDescription;
                context.Write(GetPathExpression(operation));
            }
        }

        private static string GetPathExpression(OpenApiOperationDescription operation)
        {
            var pathExpression = operation.Path;

            if (pathExpression.StartsWith("/", StringComparison.InvariantCulture))
            {
                pathExpression = pathExpression.Substring(1);
            }

            pathExpression = pathExpression.Replace("{namespace}", "{namespaceParameter}");
            return pathExpression;
        }
    }
}
