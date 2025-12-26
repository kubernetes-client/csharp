using NJsonSchema;
using NSwag;
using Scriban.Runtime;
using System;

namespace LibKubernetesGenerator
{
    internal class TypeHelper : IScriptObjectHelper
    {
        private readonly ClassNameHelper classNameHelper;

        public TypeHelper(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(GetDotNetType), new Func<JsonSchemaProperty, string>(GetDotNetType));
            scriptObject.Import(nameof(GetDotNetTypeOpenApiParameter), new Func<OpenApiParameter, string>(GetDotNetTypeOpenApiParameter));
            scriptObject.Import(nameof(GetReturnType), new Func<OpenApiOperation, string, string>(GetReturnType));
            scriptObject.Import(nameof(IfReturnType), new Func<OpenApiOperation, string, bool>(IfReturnType));
            scriptObject.Import(nameof(IfType), new Func<JsonSchemaProperty, string, bool>(IfType));
        }

        private string GetDotNetType(JsonObjectType jsonType, string name, bool required, string? format)
        {
            if (name == "pretty" && !required)
            {
                return "bool?";
            }

            switch (jsonType)
            {
                case JsonObjectType.Boolean:
                    if (required)
                    {
                        return "bool";
                    }
                    else
                    {
                        return "bool?";
                    }

                case JsonObjectType.Integer:
                    switch (format)
                    {
                        case "int64":
                            if (required)
                            {
                                return "long";
                            }
                            else
                            {
                                return "long?";
                            }

                        default:
                            if (required)
                            {
                                return "int";
                            }
                            else
                            {
                                return "int?";
                            }
                    }

                case JsonObjectType.Number:
                    if (required)
                    {
                        return "double";
                    }
                    else
                    {
                        return "double?";
                    }

                case JsonObjectType.String:

                    switch (format)
                    {
                        case "byte":
                            return required ? "byte[]" : "byte[]?";
                        case "date-time":

                            // eventTime is required but should be optional, see https://github.com/kubernetes-client/csharp/issues/1197
                            if (name == "eventTime")
                            {
                                return "System.DateTime?";
                            }

                            if (required)
                            {
                                return "System.DateTime";
                            }
                            else
                            {
                                return "System.DateTime?";
                            }
                    }

                    return required ? "string" : "string?";
                case JsonObjectType.Object:
                    return required ? "object" : "object?";
                default:
                    throw new NotSupportedException();
            }
        }

        private string GetDotNetType(JsonSchema? schema, JsonSchemaProperty parent, bool isCollectionItem = false)
        {
            if (schema != null)
            {
                if (schema.IsArray)
                {
                    return $"IList<{GetDotNetType(schema.Item!, parent, isCollectionItem: true)}>?";
                }

                if (schema.IsDictionary && schema.AdditionalPropertiesSchema != null)
                {
                    return $"IDictionary<string, {GetDotNetType(schema.AdditionalPropertiesSchema, parent, isCollectionItem: true)}>?";
                }

                if (schema?.Reference != null)
                {
                    var typeName = classNameHelper.GetClassNameForSchemaDefinition(schema.Reference);
                    // Collection items are always non-nullable, unless we're at the root level
                    return (isCollectionItem || parent.IsRequired) ? typeName : typeName + "?";
                }

                if (schema != null)
                {
                    return GetDotNetType(schema.Type, parent.Name, isCollectionItem || parent.IsRequired, schema.Format);
                }
            }

            return GetDotNetType(parent.Type, parent.Name, isCollectionItem || parent.IsRequired, parent.Format);
        }

        public string GetDotNetType(JsonSchemaProperty p)
        {
            if (p.Reference != null)
            {
                var typeName = classNameHelper.GetClassNameForSchemaDefinition(p.Reference);
                return p.IsRequired ? typeName : typeName + "?";
            }

            if (p.IsArray)
            {
                // getType - items in arrays are non-nullable
                return $"IList<{GetDotNetType(p.Item!, p, isCollectionItem: true)}>?";
            }

            if (p.IsDictionary && p.AdditionalPropertiesSchema != null)
            {
                return $"IDictionary<string, {GetDotNetType(p.AdditionalPropertiesSchema, p, isCollectionItem: true)}>?";
            }

            return GetDotNetType(p.Type, p.Name, p.IsRequired, p.Format);
        }

        public string GetDotNetTypeOpenApiParameter(OpenApiParameter parameter)
        {
            if (parameter.Schema?.Reference != null)
            {
                var typeName = classNameHelper.GetClassNameForSchemaDefinition(parameter.Schema.Reference);
                return parameter.IsRequired ? typeName : typeName + "?";
            }
            else if (parameter.Schema != null)
            {
                return (GetDotNetType(parameter.Schema.Type, parameter.Name, parameter.IsRequired,
                    parameter.Schema.Format));
            }
            else
            {
                return (GetDotNetType(parameter.Type, parameter.Name, parameter.IsRequired,
                    parameter.Format));
            }
        }

        private string GetReturnType(OpenApiOperation operation, string sytle)
        {
            OpenApiResponse response;

            if (!operation.Responses.TryGetValue("200", out response))
            {
                operation.Responses.TryGetValue("201", out response);
            }

            string toType()
            {
                if (response != null)
                {
                    var schema = response.Schema;

                    if (schema == null)
                    {
                        return "";
                    }

                    if (schema.Format == "file")
                    {
                        return "Stream";
                    }


                    if (schema.Reference != null)
                    {
                        return classNameHelper.GetClassNameForSchemaDefinition(schema.Reference);
                    }

                    return GetDotNetType(schema.Type, "", true, schema.Format);
                }

                return "";
            }

            var t = toType();

            switch (sytle)
            {
                case "<>":
                    if (t != "")
                    {
                        return "<" + t + ">";
                    }

                    break;
                case "void":
                    if (t == "")
                    {
                        return "void";
                    }

                    break;
                case "return":
                    if (t != "")
                    {
                        return "return";
                    }

                    break;
                case "_result.Body":
                    if (t != "")
                    {
                        return "return _result.Body";
                    }

                    break;
                case "T":
                    var itemType = TryGetItemTypeFromSchema(response);
                    if (itemType != null)
                    {
                        return itemType;
                    }

                    break;
                case "TList":
                    return t;
            }

            return t;
        }

        public bool IfReturnType(OpenApiOperation operation, string type)
        {
            var rt = GetReturnType(operation, "void");
            if (type == "any" && rt != "void")
            {
                return true;
            }
            else if (string.Equals(type, rt.ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (type == "obj" && rt != "void" && rt != "Stream")
            {
                return true;
            }

            return false;
        }

        public static bool IfType(JsonSchemaProperty property, string type)
        {
            if (type == "object" && property.Reference != null && !property.IsArray &&
                property.AdditionalPropertiesSchema == null)
            {
                return true;
            }
            else if (type == "objectarray" && property.IsArray && property.Item?.Reference != null)
            {
                return true;
            }

            return false;
        }

        private string TryGetItemTypeFromSchema(OpenApiResponse response)
        {
            var listSchema = response?.Schema?.Reference;
            if (listSchema?.Properties?.TryGetValue("items", out var itemsProperty) != true)
            {
                return null;
            }

            if (itemsProperty.Reference != null)
            {
                return classNameHelper.GetClassNameForSchemaDefinition(itemsProperty.Reference);
            }

            if (itemsProperty.Item?.Reference != null)
            {
                return classNameHelper.GetClassNameForSchemaDefinition(itemsProperty.Item.Reference);
            }

            return null;
        }
    }
}