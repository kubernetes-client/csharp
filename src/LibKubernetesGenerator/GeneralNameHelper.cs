using CaseExtensions;
using NJsonSchema;
using NSwag;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibKubernetesGenerator
{
    internal class GeneralNameHelper : IScriptObjectHelper
    {
        private readonly ClassNameHelper classNameHelper;

        public GeneralNameHelper(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(GetInterfaceName), new Func<JsonSchema, string>(GetInterfaceName));
            scriptObject.Import(nameof(GetMethodName), new Func<OpenApiOperation, string, string>(GetMethodName));
            scriptObject.Import(nameof(GetDotNetName), new Func<string, string, string>(GetDotNetName));
            scriptObject.Import(nameof(GetDotNetNameOpenApiParameter), new Func<OpenApiParameter, string, string>(GetDotNetNameOpenApiParameter));
        }

        private string GetInterfaceName(JsonSchema definition)
        {
            var interfaces = new List<string>();
            if (definition.Properties.TryGetValue("metadata", out var metadataProperty))
            {
                interfaces.Add(
                    $"IKubernetesObject<{classNameHelper.GetClassNameForSchemaDefinition(metadataProperty.Reference)}>");
            }
            else
            {
                interfaces.Add("IKubernetesObject");
            }

            if (definition.Properties.TryGetValue("items", out var itemsProperty))
            {
                var schema = itemsProperty.Type == JsonObjectType.Object
                    ? itemsProperty.Reference
                    : itemsProperty.Item.Reference;
                interfaces.Add($"IItems<{classNameHelper.GetClassNameForSchemaDefinition(schema)}>");
            }

            if (definition.Properties.TryGetValue("spec", out var specProperty))
            {
                // ignore empty spec placeholder
                if (specProperty.Reference?.ActualProperties.Any() == true)
                {
                    interfaces.Add($"ISpec<{classNameHelper.GetClassNameForSchemaDefinition(specProperty.Reference)}>");
                }
            }

            interfaces.Add("IValidate");

            return string.Join(", ", interfaces);
        }

        public string GetDotNetNameOpenApiParameter(OpenApiParameter parameter, string init)
        {
            var name = GetDotNetName(parameter.Name);

            if (init == "true" && !parameter.IsRequired)
            {
                name += " = null";
            }

            return name;
        }

        public string GetDotNetName(string jsonName, string style = "parameter")
        {
            switch (style)
            {
                case "parameter":
                    switch (jsonName)
                    {
                        case "namespace":
                            return "namespaceParameter";
                        case "continue":
                            return "continueParameter";
                        default:
                            break;
                    }

                    break;

                case "fieldctor":

                    switch (jsonName)
                    {
                        case "namespace":
                            return "namespaceProperty";
                        case "continue":
                            return "continueProperty";
                        case "$ref":
                            return "refProperty";
                        case "default":
                            return "defaultProperty";
                        case "operator":
                            return "operatorProperty";
                        case "$schema":
                            return "schema";
                        case "enum":
                            return "enumProperty";
                        case "object":
                            return "objectProperty";
                        case "readOnly":
                            return "readOnlyProperty";
                        case "from":
                            return "fromProperty";
                        case "int":
                            return "intValue";
                        case "bool":
                            return "boolValue";
                        case "string":
                            return "stringValue";

                        default:
                            break;
                    }

                    if (jsonName.Contains("-"))
                    {
                        return jsonName.ToCamelCase();
                    }

                    break;
                case "field":
                    return GetDotNetName(jsonName, "fieldctor").ToPascalCase();
            }

            return jsonName.ToCamelCase();
        }

        public static string GetMethodName(OpenApiOperation watchOperation, string suffix)
        {
            var tag = watchOperation.Tags[0];
            tag = tag.Replace("_", string.Empty);

            var methodName = watchOperation.OperationId.ToPascalCase();

            switch (suffix)
            {
                case "":
                case "Async":
                case "WithHttpMessagesAsync":
                    methodName += suffix;
                    break;

                default:
                    // This tries to remove the version from the method name, e.g. watchCoreV1NamespacedPod => WatchNamespacedPod
                    methodName = Regex.Replace(methodName, tag, string.Empty, RegexOptions.IgnoreCase);
                    methodName += "Async";
                    break;
            }

            return methodName;
        }
    }
}
