using System;
using System.Collections.Generic;
using System.Linq;
using CaseExtensions;
using NJsonSchema;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    internal class GeneralNameHelper : INustacheHelper
    {
        private readonly ClassNameHelper classNameHelper;

        public GeneralNameHelper(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void RegisterHelper()
        {
            Helpers.Register(nameof(GetInterfaceName), GetInterfaceName);
            Helpers.Register(nameof(GetMethodName), GetMethodName);
            Helpers.Register(nameof(GetDotNetName), GetDotNetName);
        }

        public void GetInterfaceName(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema)
            {
                context.Write(GetInterfaceName(arguments[0] as JsonSchema));
            }
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
                if (specProperty.Reference.ActualProperties.Any())
                {
                    interfaces.Add($"ISpec<{classNameHelper.GetClassNameForSchemaDefinition(specProperty.Reference)}>");
                }
            }

            interfaces.Add("IValidate");

            return string.Join(", ", interfaces);
        }

        public void GetMethodName(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is OpenApiOperation)
            {
                string suffix = null;
                if (arguments.Count > 1)
                {
                    suffix = arguments[1] as string;
                }

                context.Write(GetMethodName(arguments[0] as OpenApiOperation, suffix));
            }
        }

        public void GetDotNetName(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is OpenApiParameter)
            {
                var parameter = arguments[0] as OpenApiParameter;
                context.Write(GetDotNetName(parameter.Name));

                if (arguments.Count > 1 && arguments[1] as string == "true" && !parameter.IsRequired)
                {
                    context.Write(" = null");
                }
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is string)
            {
                var style = "parameter";
                if (arguments.Count > 1)
                {
                    style = arguments[1] as string;
                }

                context.Write(GetDotNetName((string)arguments[0], style));
            }
        }

        public string GetDotNetName(string jsonName, string style = "parameter")
        {
            switch (style)
            {
                case "parameter":
                    if (jsonName == "namespace")
                    {
                        return "namespaceParameter";
                    }
                    else if (jsonName == "continue")
                    {
                        return "continueParameter";
                    }

                    break;

                case "fieldctor":
                    if (jsonName == "namespace")
                    {
                        return "namespaceProperty";
                    }
                    else if (jsonName == "continue")
                    {
                        return "continueProperty";
                    }
                    else if (jsonName == "$ref")
                    {
                        return "refProperty";
                    }
                    else if (jsonName == "default")
                    {
                        return "defaultProperty";
                    }
                    else if (jsonName == "operator")
                    {
                        return "operatorProperty";
                    }
                    else if (jsonName == "$schema")
                    {
                        return "schema";
                    }
                    else if (jsonName == "enum")
                    {
                        return "enumProperty";
                    }
                    else if (jsonName == "object")
                    {
                        return "objectProperty";
                    }
                    else if (jsonName == "readOnly")
                    {
                        return "readOnlyProperty";
                    }
                    else if (jsonName == "from")
                    {
                        return "fromProperty";
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
                    methodName = methodName.Replace(tag, string.Empty, StringComparison.OrdinalIgnoreCase);
                    methodName += "Async";
                    break;
            }

            return methodName;
        }
    }
}
