using NJsonSchema;
using NSwag;
using Scriban.Runtime;
using System;
using System.Collections.Generic;

namespace LibKubernetesGenerator
{
    internal class MetaHelper : IScriptObjectHelper
    {
        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(GetGroup), GetGroup);
            scriptObject.Import(nameof(GetApiVersion), GetApiVersion);
            scriptObject.Import(nameof(GetKind), GetKind);
            scriptObject.Import(nameof(GetPathExpression), GetPathExpression);
        }

        private static string GetKind(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["kind"] as string;
        }

        private static string GetGroup(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["group"] as string;
        }

        private static string GetApiVersion(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["version"] as string;
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
