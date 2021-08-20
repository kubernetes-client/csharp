using CaseExtensions;
using NJsonSchema;
using NSwag;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KubernetesWatchGenerator
{
    internal class Program
    {
        private static HashSet<string> _classesWithValidation;
        private static readonly Dictionary<string, string> ClassNameMap = new Dictionary<string, string>();
        private static Dictionary<JsonSchema4, string> _schemaToNameMap;
        private static Dictionary<JsonSchema4, string> _schemaToNameMapCooked;
        private static HashSet<string> _schemaDefinitionsInMultipleGroups;
        private static Dictionary<string, string> _classNameToPluralMap;

        private static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine($"usage {args[0]} path/to/generated");
                Environment.Exit(1);
            }

            var outputDirectory = args[1];

            // Read the spec trimmed
            // here we cache all name in gen project for later use
            var swaggercooked = await SwaggerDocument.FromFileAsync(Path.Combine(args[1], "swagger.json")).ConfigureAwait(false);
            foreach (var (k, v) in swaggercooked.Definitions)
            {
                if (v.ExtensionData?.TryGetValue("x-kubernetes-group-version-kind", out var _) == true)
                {
                    var groupVersionKindElements = (object[])v.ExtensionData["x-kubernetes-group-version-kind"];
                    var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

                    var group = (string)groupVersionKind["group"];
                    var kind = (string)groupVersionKind["kind"];
                    var version = (string)groupVersionKind["version"];
                    ClassNameMap[$"{group}_{kind}_{version}"] = ToPascalCase(k.Replace(".", ""));
                }
            }

            _schemaToNameMapCooked = swaggercooked.Definitions.ToDictionary(x => x.Value, x => ToPascalCase(x.Key.Replace(".", "")));

            // gen project removed all watch operations, so here we switch back to unprocessed version
            var swagger = await SwaggerDocument.FromFileAsync(Path.Combine(args[1], "swagger.json.unprocessed")).ConfigureAwait(false);
            _schemaToNameMap = swagger.Definitions.ToDictionary(x => x.Value, x => x.Key);
            _schemaDefinitionsInMultipleGroups = _schemaToNameMap.Values.Select(x =>
                {
                    var parts = x.Split(".");
                    return new
                    {
                        FullName = x,
                        Name = parts[parts.Length - 1],
                        Version = parts[parts.Length - 2],
                        Group = parts[parts.Length - 3],
                    };
                })
                .GroupBy(x => new { x.Name, x.Version })
                .Where(x => x.Count() > 1)
                .SelectMany(x => x)
                .Select(x => x.FullName)
                .ToHashSet();

            _classNameToPluralMap = swagger.Operations
                .Where(x => x.Operation.OperationId.StartsWith("list", StringComparison.InvariantCulture))
                .Select(x =>
                {
                    return new
                    {
                        PluralName = x.Path.Split("/").Last(),
                        ClassName = GetClassNameForSchemaDefinition(x.Operation.Responses["200"]
                            .ActualResponseSchema),
                    };
                })
                .Distinct()
                .ToDictionary(x => x.ClassName, x => x.PluralName);

            // dictionary only contains "list" plural maps. assign the same plural names to entities those lists support
            _classNameToPluralMap = _classNameToPluralMap
                .Where(x => x.Key.EndsWith("List", StringComparison.InvariantCulture))
                .Select(x =>
                    new { ClassName = x.Key.Remove(x.Key.Length - 4), PluralName = x.Value })
                .ToDictionary(x => x.ClassName, x => x.PluralName)
                .Union(_classNameToPluralMap)
                .ToDictionary(x => x.Key, x => x.Value);

            // Register helpers used in the templating.
            Helpers.Register(nameof(ToXmlDoc), ToXmlDoc);
            Helpers.Register(nameof(GetClassName), GetClassName);
            Helpers.Register(nameof(GetInterfaceName), GetInterfaceName);
            Helpers.Register(nameof(GetMethodName), GetMethodName);
            Helpers.Register(nameof(GetDotNetName), GetDotNetName);
            Helpers.Register(nameof(GetDotNetType), GetDotNetType);
            Helpers.Register(nameof(GetPathExpression), GetPathExpression);
            Helpers.Register(nameof(GetGroup), GetGroup);
            Helpers.Register(nameof(GetApiVersion), GetApiVersion);
            Helpers.Register(nameof(GetKind), GetKind);
            Helpers.Register(nameof(GetPlural), GetPlural);
            Helpers.Register(nameof(GetTuple), GetTuple);
            Helpers.Register(nameof(GetReturnType), GetReturnType);
            Helpers.Register(nameof(IfKindIs), IfKindIs);
            Helpers.Register(nameof(AddCurly), AddCurly);
            Helpers.Register(nameof(GetRequestMethod), GetRequestMethod);
            Helpers.Register(nameof(EscapeDataString), EscapeDataString);
            Helpers.Register(nameof(IfReturnType), IfReturnType);
            Helpers.Register(nameof(GetModelCtorParam), GetModelCtorParam);
            Helpers.Register(nameof(IfType), IfType);

            // Generate the Watcher operations
            // We skip operations where the name of the class in the C# client could not be determined correctly.
            // That's usually because there are different version of the same object (e.g. for deployments).
            var blacklistedOperations = new HashSet<string>() { };

            var watchOperations = swagger.Operations.Where(
                o => o.Path.Contains("/watch/")
                     && o.Operation.ActualParameters.Any(p => p.Name == "name")
                     && !blacklistedOperations.Contains(o.Operation.OperationId)).ToArray();

            // Render.
            Render.FileToFile("IKubernetes.Watch.cs.template", watchOperations,
                Path.Combine(outputDirectory, "IKubernetes.Watch.cs"));
            Render.FileToFile("Kubernetes.Watch.cs.template", watchOperations,
                Path.Combine(outputDirectory, "Kubernetes.Watch.cs"));

            var data = swaggercooked.Operations
                            .Where(o => o.Method != SwaggerOperationMethod.Options)
                            .GroupBy(o => o.Operation.OperationId)
                            .Select(g =>
                            {
                                var gs = g.ToArray();

                                for (int i = 1; i < g.Count(); i++)
                                {
                                    gs[i].Operation.OperationId += i;
                                }

                                return gs;
                            })
                            .SelectMany(g => g)
                            .Select(o =>
                            {
                                var ps = o.Operation.ActualParameters.OrderBy(p => !p.IsRequired).ToArray();

                                o.Operation.Parameters.Clear();

                                var name = new HashSet<string>();

                                var i = 1;
                                foreach (var p in ps)
                                {
                                    if (name.Contains(p.Name))
                                    {
                                        p.Name = p.Name + i++;
                                    }

                                    o.Operation.Parameters.Add(p);
                                    name.Add(p.Name);
                                }

                                return o;
                            })
                            .ToArray();

            Render.FileToFile("IKubernetes.cs.template", data, Path.Combine(outputDirectory, "IKubernetes.cs"));
            Render.FileToFile("KubernetesExtensions.cs.template", data, Path.Combine(outputDirectory, "KubernetesExtensions.cs"));
            Render.FileToFile("Kubernetes.cs.template", data, Path.Combine(outputDirectory, "Kubernetes.cs"));

            Directory.CreateDirectory(Path.Combine(outputDirectory, "Models"));

            foreach (var (_, def) in swaggercooked.Definitions)
            {
                var clz = GetClassNameForSchemaDefinition(def);
                Render.FileToFile("Model.cs.template", new
                {
                    clz,
                    def,
                    properties = def.Properties.Values,
                }, Path.Combine(outputDirectory, "Models", $"{clz}.cs"));
            }

            // Generate the interface declarations
            var skippedTypes = new HashSet<string>() { "V1WatchEvent", };

            var definitions = swagger.Definitions.Values
                .Where(
                    d => d.ExtensionData != null
                         && d.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                         && !skippedTypes.Contains(GetClassName(d)));

            var modelsDir = Path.Combine(outputDirectory, "Models");
            _classesWithValidation = Directory.EnumerateFiles(modelsDir)
                .Select(x => new { Class = Path.GetFileNameWithoutExtension(x), Content = File.ReadAllText(x) })
                .Where(x => x.Content.Contains("public virtual void Validate()"))
                .Select(x => x.Class)
                .ToHashSet();

            Render.FileToFile("ModelExtensions.cs.template", definitions, Path.Combine(outputDirectory, "ModelExtensions.cs"));

            // generate version converter maps
            var allGeneratedModelClassNames = Directory
                .EnumerateFiles(Path.Combine(outputDirectory, "Models"))
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            var versionRegex = @"(^V|v)[0-9]+((alpha|beta)[0-9]+)?";
            var typePairs = allGeneratedModelClassNames
                .OrderBy(x => x)
                .Select(x => new { Version = Regex.Match(x, versionRegex).Value?.ToLower(), Kinda = Regex.Replace(x, versionRegex, string.Empty), Type = x })
                .Where(x => !string.IsNullOrEmpty(x.Version))
                .GroupBy(x => x.Kinda)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.SelectMany((value, index) => x.Skip(index + 1), (first, second) => new { first, second }))
                .OrderBy(x => x.first.Kinda)
                .ThenBy(x => x.first.Version)
                .Select(x => (ITuple)Tuple.Create(x.first.Type, x.second.Type))
                .ToList();

            var versionFile = File.ReadAllText(Path.Combine(outputDirectory, "..", "Versioning", "VersionConverter.cs"));
            var manualMaps = Regex.Matches(versionFile, @"\.CreateMap<(?<T1>.+?),\s?(?<T2>.+?)>")
                .Select(x => Tuple.Create(x.Groups["T1"].Value, x.Groups["T2"].Value))
                .ToList();
            var versionConverterPairs = typePairs.Except(manualMaps).ToList();

            Render.FileToFile("VersionConverter.cs.template", versionConverterPairs, Path.Combine(outputDirectory, "VersionConverter.cs"));
            Render.FileToFile("ModelOperators.cs.template", typePairs, Path.Combine(outputDirectory, "ModelOperators.cs"));
        }

        private static void ToXmlDoc(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is string)
            {
                var first = true;

                using (var reader = new StringReader(arguments[0] as string))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (var wline in WordWrap(line, 80))
                        {
                            if (!first)
                            {
                                context.Write("\n");
                                context.Write("        /// ");
                            }
                            else
                            {
                                first = false;
                            }

                            context.Write(SecurityElement.Escape(wline));
                        }
                    }
                }
            }
        }

        private static void GetTuple(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] is ITuple && options.TryGetValue("index", out var indexObj) && int.TryParse(indexObj?.ToString(), out var index))
            {
                var pair = (ITuple)arguments[0];
                var value = pair[index];
                context.Write(value.ToString());
            }
        }

        private static void GetClassName(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperation)
            {
                context.Write(GetClassName(arguments[0] as SwaggerOperation));
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetClassNameForSchemaDefinition(arguments[0] as JsonSchema4));
            }
        }

        private static string GetClassName(SwaggerOperation watchOperation)
        {
            var groupVersionKind =
                (Dictionary<string, object>)watchOperation.ExtensionData["x-kubernetes-group-version-kind"];
            return GetClassName(groupVersionKind);
        }

        private static string GetClassName(Dictionary<string, object> groupVersionKind)
        {
            var group = (string)groupVersionKind["group"];
            var kind = (string)groupVersionKind["kind"];
            var version = (string)groupVersionKind["version"];

            return ClassNameMap[$"{group}_{kind}_{version}"];
        }

        private static string GetClassName(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return GetClassName(groupVersionKind);
        }

        private static void GetInterfaceName(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetInterfaceName(arguments[0] as JsonSchema4));
            }
        }

        private static string GetClassNameForSchemaDefinition(JsonSchema4 definition)
        {
            if (definition.ExtensionData != null &&
                definition.ExtensionData.ContainsKey("x-kubernetes-group-version-kind"))
            {
                return GetClassName(definition);
            }

            if (_schemaToNameMapCooked.TryGetValue(definition, out var name))
            {
                return name;
            }

            var schemaName = _schemaToNameMap[definition];

            var parts = schemaName.Split(".");
            var group = parts[parts.Length - 3];
            var version = parts[parts.Length - 2];
            var entityName = parts[parts.Length - 1];
            if (!_schemaDefinitionsInMultipleGroups.Contains(schemaName))
            {
                @group = null;
            }

            var className = ToPascalCase($"{group}{version}{entityName}");
            return className;
        }

        private static string GetInterfaceName(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            var group = groupVersionKind["group"] as string;
            var version = groupVersionKind["version"] as string;
            var kind = groupVersionKind["kind"] as string;
            var className = GetClassName(definition);
            var interfaces = new List<string>();
            if (definition.Properties.TryGetValue("metadata", out var metadataProperty))
            {
                interfaces.Add($"IKubernetesObject<{GetClassNameForSchemaDefinition(metadataProperty.Reference)}>");
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
                interfaces.Add($"IItems<{GetClassNameForSchemaDefinition(schema)}>");
            }

            if (definition.Properties.TryGetValue("spec", out var specProperty))
            {
                // ignore empty spec placeholder
                if (specProperty.Reference.ActualProperties.Any())
                {
                    interfaces.Add($"ISpec<{GetClassNameForSchemaDefinition(specProperty.Reference)}>");
                }
            }

            if (_classesWithValidation.Contains(className))
            {
                interfaces.Add("IValidate");
            }

            var result = string.Join(", ", interfaces);
            return result;
        }

        private static void GetKind(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetKind(arguments[0] as JsonSchema4));
            }
        }

        private static string GetKind(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["kind"] as string;
        }

        private static void GetPlural(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                var plural = GetPlural(arguments[0] as JsonSchema4);
                if (plural != null)
                {
                    context.Write($"\"{plural}\"");
                }
                else
                {
                    context.Write("null");
                }
            }
        }

        private static string GetPlural(JsonSchema4 definition)
        {
            var className = GetClassNameForSchemaDefinition(definition);
            return _classNameToPluralMap.GetValueOrDefault(className, null);
        }

        private static void GetGroup(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetGroup(arguments[0] as JsonSchema4));
            }
        }

        private static string GetGroup(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["group"] as string;
        }

        private static void GetMethodName(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperation)
            {
                string suffix = null;
                if (arguments.Count > 1)
                {
                    suffix = arguments[1] as string;
                }

                context.Write(GetMethodName(arguments[0] as SwaggerOperation, suffix));
            }
        }

        private static string GetMethodName(SwaggerOperation watchOperation, string suffix)
        {
            var tag = watchOperation.Tags[0];
            tag = tag.Replace("_", string.Empty);

            var methodName = ToPascalCase(watchOperation.OperationId);

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

        private static void GetDotNetType(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerParameter)
            {
                var parameter = arguments[0] as SwaggerParameter;

                if (parameter.Schema?.Reference != null)
                {
                    context.Write(GetClassNameForSchemaDefinition(parameter.Schema.Reference));
                }
                else if (parameter.Schema != null)
                {
                    context.Write(GetDotNetType(parameter.Schema.Type, parameter.Name, parameter.IsRequired, parameter.Schema.Format));
                }
                else
                {
                    context.Write(GetDotNetType(parameter.Type, parameter.Name, parameter.IsRequired, parameter.Format));
                }
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonProperty)
            {
                var property = arguments[0] as JsonProperty;
                context.Write(GetDotNetType(property));
            }
            else if (arguments != null && arguments.Count > 2 && arguments[0] != null && arguments[1] != null &&
                     arguments[2] != null && arguments[0] is JsonObjectType && arguments[1] is string &&
                     arguments[2] is bool)
            {
                context.Write(GetDotNetType((JsonObjectType)arguments[0], (string)arguments[1], (bool)arguments[2], (string)arguments[3]));
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null)
            {
                context.Write($"ERROR: Expected SwaggerParameter but got {arguments[0].GetType().FullName}");
            }
            else
            {
                context.Write($"ERROR: Expected a SwaggerParameter argument but got none.");
            }
        }

        private static string GetDotNetType(JsonObjectType jsonType, string name, bool required, string format)
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

                            break;
                        case "int32":
                        default:
                            if (required)
                            {
                                return "int";
                            }
                            else
                            {
                                return "int?";
                            }

                            break;
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

        private static string GetDotNetType(JsonSchema4 schema, JsonProperty parent)
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
                    return GetClassNameForSchemaDefinition(schema.Reference);
                }
                else if (schema != null)
                {
                    return GetDotNetType(schema.Type, parent.Name, parent.IsRequired, schema.Format);
                }

            }

            return GetDotNetType(parent.Type, parent.Name, parent.IsRequired, parent.Format);
        }

        private static string GetDotNetType(JsonProperty p)
        {
            if (p.SchemaReference != null)
            {
                return GetClassNameForSchemaDefinition(p.SchemaReference);
            }
            else
            {
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
        }

        private static void GetDotNetName(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerParameter)
            {
                var parameter = arguments[0] as SwaggerParameter;
                context.Write(GetDotNetName(parameter.Name));

                if (arguments.Count > 1 && arguments[1] as string == "true" && !parameter.IsRequired)
                {
                    context.Write($" = null");
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

        private static string GetDotNetName(string jsonName, string style = "parameter")
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
                    else if (jsonName == "__referencePath")
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

        private static void GetPathExpression(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null &&
                arguments[0] is SwaggerOperationDescription)
            {
                var operation = arguments[0] as SwaggerOperationDescription;
                context.Write(GetPathExpression(operation));
            }
        }

        private static string GetPathExpression(SwaggerOperationDescription operation)
        {
            var pathExpression = operation.Path;

            if (pathExpression.StartsWith("/", StringComparison.InvariantCulture))
            {
                pathExpression = pathExpression.Substring(1);
            }

            pathExpression = pathExpression.Replace("{namespace}", "{namespaceParameter}");
            return pathExpression;
        }

        private static void GetApiVersion(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetApiVersion(arguments[0] as JsonSchema4));
            }
        }

        private static void GetReturnType(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var operation = arguments?.FirstOrDefault() as SwaggerOperation;
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

        private static string GetReturnType(SwaggerOperation operation, string sytle)
        {
            SwaggerResponse response;

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
                        return GetClassNameForSchemaDefinition(schema.Reference);
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
                default:
                    break;
            }

            return t;
        }

        private static string GetApiVersion(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["version"] as string;
        }

        private static void IfKindIs(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
           RenderBlock fn, RenderBlock inverse)
        {
            var parameter = arguments?.FirstOrDefault() as SwaggerParameter;
            if (parameter != null)
            {
                string kind = null;
                if (arguments.Count > 1)
                {
                    kind = arguments[1] as string;
                }

                if (kind == "query" && parameter.Kind == SwaggerParameterKind.Query)
                {
                    fn(null);
                }
                else if (kind == "path" && parameter.Kind == SwaggerParameterKind.Path)
                {
                    fn(null);
                }
            }
        }

        private static void AddCurly(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
           RenderBlock fn, RenderBlock inverse)
        {
            var s = arguments?.FirstOrDefault() as string;
            if (s != null)
            {
                context.Write("{" + s + "}");
            }
        }

        private static void GetRequestMethod(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
           RenderBlock fn, RenderBlock inverse)
        {
            var s = arguments?.FirstOrDefault() as SwaggerOperationMethod?;
            if (s != null)
            {
                context.Write(s.ToString().ToUpper());
            }
        }

        private static void IfReturnType(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
           RenderBlock fn, RenderBlock inverse)
        {
            var operation = arguments?.FirstOrDefault() as SwaggerOperation;
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
                else if (type == "stream" && rt == "Stream")
                {
                    fn(null);
                }
                else if (type == "obj" && rt != "void" && rt != "Stream")
                {
                    fn(null);
                }
            }
        }

        private static void EscapeDataString(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
           RenderBlock fn, RenderBlock inverse)
        {
            var name = GetDotNetName(arguments[0] as string);
            var type = arguments[1] as JsonObjectType?;

            if (name == "pretty")
            {
                context.Write($"{name}.Value == true ? \"true\" : \"false\"");
                return;
            }

            switch (type)
            {
                case JsonObjectType.String:
                    context.Write($"System.Uri.EscapeDataString({name})");
                    break;
                default:
                    context.Write($"System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject({name}, SerializationSettings).Trim('\"'))");
                    break;
            }
        }

        private static void GetModelCtorParam(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
           RenderBlock fn, RenderBlock inverse)
        {
            var schema = arguments[0] as JsonSchema4;

            if (schema != null)
            {
                context.Write(string.Join(", ", schema.Properties.Values
                    .OrderBy(p => !p.IsRequired)
                    .Select(p =>
                    {
                        string sp = $"{GetDotNetType(p)} {GetDotNetName(p.Name, "fieldctor")}";

                        if (!p.IsRequired)
                        {
                            sp = $"{sp} = null";
                        }

                        return sp;
                    })));
            }
        }

        private static void IfType(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var property = arguments?.FirstOrDefault() as JsonProperty;
            if (property != null)
            {
                string type = null;
                if (arguments.Count > 1)
                {
                    type = arguments[1] as string;
                }

                if (type == "object" && property.Reference != null && !property.IsArray && property.AdditionalPropertiesSchema == null)
                {
                    fn(null);
                }
                else if (type == "objectarray" && property.IsArray && property.Item?.Reference != null)
                {
                    fn(null);
                }
            }
        }


        private static string ToPascalCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return char.ToUpper(name[0]) + name.Substring(1);
        }

        public static IEnumerable<string> WordWrap(string text, int width)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var processedLine = line.Trim();

                // yield empty lines as they are (probably) intensional
                if (processedLine.Length == 0)
                {
                    yield return processedLine;
                }

                // feast on the line until it's gone
                while (processedLine.Length > 0)
                {
                    // determine potential wrapping points
                    var whitespacePositions = Enumerable
                        .Range(0, processedLine.Length)
                        .Where(i => char.IsWhiteSpace(processedLine[i]))
                        .Concat(new[] { processedLine.Length })
                        .Cast<int?>();
                    var preWidthWrapAt = whitespacePositions.LastOrDefault(i => i <= width);
                    var postWidthWrapAt = whitespacePositions.FirstOrDefault(i => i > width);

                    // choose preferred wrapping point
                    var wrapAt = preWidthWrapAt ?? postWidthWrapAt ?? processedLine.Length;

                    // wrap
                    yield return processedLine.Substring(0, wrapAt);
                    processedLine = processedLine.Substring(wrapAt).Trim();
                }
            }
        }
    }
}
