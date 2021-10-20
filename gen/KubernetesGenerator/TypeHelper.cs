using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class TypeHelper : INustacheHelper
    {
        private readonly ClassNameHelper classNameHelper;

        public TypeHelper(ClassNameHelper classNameHelper)
        {
            this.classNameHelper = classNameHelper;
        }

        public void RegisterHelper()
        {
            Helpers.Register(nameof(GetDotNetType), GetDotNetType);
            Helpers.Register(nameof(GetReturnType), GetReturnType);
            Helpers.Register(nameof(IfReturnType), IfReturnType);
            Helpers.Register(nameof(IfType), IfType);
        }

        public void GetDotNetType(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is OpenApiParameter)
            {
                var parameter = arguments[0] as OpenApiParameter;

                if (parameter.Schema?.Reference != null)
                {
                    context.Write(classNameHelper.GetClassNameForSchemaDefinition(parameter.Schema.Reference));
                }
                else if (parameter.Schema != null)
                {
                    context.Write(GetDotNetType(parameter.Schema.Type, parameter.Name, parameter.IsRequired,
                        parameter.Schema.Format));
                }
                else
                {
                    context.Write(GetDotNetType(parameter.Type, parameter.Name, parameter.IsRequired,
                        parameter.Format));
                }
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchemaProperty)
            {
                var property = arguments[0] as JsonSchemaProperty;
                context.Write(GetDotNetType(property));
            }
            else if (arguments != null && arguments.Count > 2 && arguments[0] != null && arguments[1] != null &&
                     arguments[2] != null && arguments[0] is JsonObjectType && arguments[1] is string &&
                     arguments[2] is bool)
            {
                context.Write(GetDotNetType((JsonObjectType)arguments[0], (string)arguments[1], (bool)arguments[2],
                    (string)arguments[3]));
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                context.Write($"ERROR: Expected OpenApiParameter but got {arguments[0].GetType().FullName}");
            }
            else
            {
                context.Write("ERROR: Expected a OpenApiParameter argument but got none.");
            }
        }

        private string GetDotNetType(JsonObjectType jsonType, string name, bool required, string format)
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
                            return "byte[]";
                        case "date-time":
                            if (required)
                            {
                                return "System.DateTime";
                            }
                            else
                            {
                                return "System.DateTime?";
                            }
                    }

                    return "string";
                case JsonObjectType.Object:
                    return "object";
                default:
                    throw new NotSupportedException();
            }
        }

        private string GetDotNetType(JsonSchema schema, JsonSchemaProperty parent)
        {
            if (schema != null)
            {
                if (schema.IsArray)
                {
                    return $"IList<{GetDotNetType(schema.Item, parent)}>";
                }

                if (schema.IsDictionary && schema.AdditionalPropertiesSchema != null)
                {
                    return $"IDictionary<string, {GetDotNetType(schema.AdditionalPropertiesSchema, parent)}>";
                }


                if (schema?.Reference != null)
                {
                    return classNameHelper.GetClassNameForSchemaDefinition(schema.Reference);
                }

                if (schema != null)
                {
                    return GetDotNetType(schema.Type, parent.Name, parent.IsRequired, schema.Format);
                }
            }

            return GetDotNetType(parent.Type, parent.Name, parent.IsRequired, parent.Format);
        }

        public string GetDotNetType(JsonSchemaProperty p)
        {
            if (p.Reference != null)
            {
                return classNameHelper.GetClassNameForSchemaDefinition(p.Reference);
            }

            if (p.IsArray)
            {
                // getType
                return $"IList<{GetDotNetType(p.Item, p)}>";
            }

            if (p.IsDictionary && p.AdditionalPropertiesSchema != null)
            {
                return $"IDictionary<string, {GetDotNetType(p.AdditionalPropertiesSchema, p)}>";
            }

            return GetDotNetType(p.Type, p.Name, p.IsRequired, p.Format);
        }

        public void GetReturnType(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var operation = arguments?.FirstOrDefault() as OpenApiOperation;
            if (operation != null)
            {
                string style = null;
                if (arguments.Count > 1)
                {
                    style = arguments[1] as string;
                }

                context.Write(GetReturnType(operation, style));
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
            }

            return t;
        }

        public void IfReturnType(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var operation = arguments?.FirstOrDefault() as OpenApiOperation;
            if (operation != null)
            {
                string type = null;
                if (arguments.Count > 1)
                {
                    type = arguments[1] as string;
                }

                var rt = GetReturnType(operation, "void");
                if (type == "any" && rt != "void")
                {
                    fn(null);
                }
                else if (type.ToLower() == rt.ToLower())
                {
                    fn(null);
                }
                else if (type == "obj" && rt != "void" && rt != "Stream")
                {
                    fn(null);
                }
            }
        }

        public static void IfType(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var property = arguments?.FirstOrDefault() as JsonSchemaProperty;
            if (property != null)
            {
                string type = null;
                if (arguments.Count > 1)
                {
                    type = arguments[1] as string;
                }

                if (type == "object" && property.Reference != null && !property.IsArray &&
                    property.AdditionalPropertiesSchema == null)
                {
                    fn(null);
                }
                else if (type == "objectarray" && property.IsArray && property.Item?.Reference != null)
                {
                    fn(null);
                }
            }
        }
    }
}
