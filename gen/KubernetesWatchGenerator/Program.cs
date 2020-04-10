using NJsonSchema;
using NSwag;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KubernetesWatchGenerator
{
    class Program
    {
        private static HashSet<string> _classesWithValidation;
        static readonly Dictionary<string, string> ClassNameMap = new Dictionary<string, string>();
        private static Dictionary<JsonSchema4, string> _schemaToNameMap;
        private static HashSet<string> _schemaDefinitionsInMultipleGroups;
        private static Dictionary<string, string> _classNameToPluralMap;

        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine($"usage {args[0]} path/to/generated");
                Environment.Exit(1);
            }

            var outputDirectory = args[1];

            // Read the spec trimmed
            // here we cache all name in gen project for later use
            var swagger = await SwaggerDocument.FromFileAsync(Path.Combine(args[1], "swagger.json"));
            foreach (var (k, v) in swagger.Definitions)
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


            // gen project removed all watch operations, so here we switch back to unprocessed version
            swagger = await SwaggerDocument.FromFileAsync(Path.Combine(args[1], "swagger.json.unprocessed"));
            _schemaToNameMap = swagger.Definitions.ToDictionary(x => x.Value, x => x.Key);
            _schemaDefinitionsInMultipleGroups = _schemaToNameMap.Values.Select(x =>
            {
                var parts = x.Split(".");
                return new {FullName = x, Name = parts[parts.Length - 1], Version = parts[parts.Length - 2], Group = parts[parts.Length - 3]};
            })
                .GroupBy(x => new {x.Name, x.Version})
                .Where(x => x.Count() > 1)
                .SelectMany(x => x)
                .Select(x => x.FullName)
                .ToHashSet();

            _classNameToPluralMap = swagger.Operations
                .Where(x => x.Operation.OperationId.StartsWith("list"))
                .Select(x => { return new {PluralName = x.Path.Split("/").Last(), ClassName = GetClassNameForSchemaDefinition(x.Operation.Responses["200"].ActualResponseSchema)}; })
                .Distinct()
                .ToDictionary(x => x.ClassName, x => x.PluralName);

            // dictionary only contains "list" plural maps. assign the same plural names to entities those lists support
            _classNameToPluralMap = _classNameToPluralMap
                .Where(x => x.Key.EndsWith("List"))
                .Select(x =>
                    new {ClassName = x.Key.Remove(x.Key.Length - 4), PluralName = x.Value})
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

            // Generate the Watcher operations
            // We skip operations where the name of the class in the C# client could not be determined correctly.
            // That's usually because there are different version of the same object (e.g. for deployments).
            var blacklistedOperations = new HashSet<string>()
            {
            };

            var watchOperations = swagger.Operations.Where(
                o => o.Path.Contains("/watch/")
                && o.Operation.ActualParameters.Any(p => p.Name == "name")
                && !blacklistedOperations.Contains(o.Operation.OperationId)).ToArray();

            // Render.
            Render.FileToFile("IKubernetes.Watch.cs.template", watchOperations, $"{outputDirectory}IKubernetes.Watch.cs");
            Render.FileToFile("Kubernetes.Watch.cs.template", watchOperations, $"{outputDirectory}Kubernetes.Watch.cs");

            // Generate the interface declarations
            var skippedTypes = new HashSet<string>()
            {
                "V1WatchEvent",
            };

            var definitions = swagger.Definitions.Values
                .Where(
                    d => d.ExtensionData != null
                    && d.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                    && !skippedTypes.Contains(GetClassName(d)));

            var modelsDir = Path.Combine(outputDirectory, "Models");
            _classesWithValidation = Directory.EnumerateFiles(modelsDir)
                .Select(x => new {Class = Path.GetFileNameWithoutExtension(x), Content = File.ReadAllText(x)})
                .Where(x => x.Content.Contains("public virtual void Validate()"))
                .Select(x => x.Class)
                .ToHashSet();

            Render.FileToFile("ModelExtensions.cs.template", definitions, Path.Combine(outputDirectory, "ModelExtensions.cs"));
        }

        static void ToXmlDoc(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is string)
            {
                bool first = true;

                using (StringReader reader = new StringReader(arguments[0] as string))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
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
                        context.Write(line);
                    }
                }
            }
        }

        static void GetClassName(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperation)
            {
                context.Write(GetClassName(arguments[0] as SwaggerOperation));
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetClassName(arguments[0] as JsonSchema4));
            }
        }

        static string GetClassName(SwaggerOperation watchOperation)
        {
            var groupVersionKind = (Dictionary<string, object>)watchOperation.ExtensionData["x-kubernetes-group-version-kind"];
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
        private static void GetInterfaceName(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {

            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetInterfaceName(arguments[0] as JsonSchema4));
            }

        }

        static string GetClassNameForSchemaDefinition(JsonSchema4 definition)
        {
            if (definition.ExtensionData != null && definition.ExtensionData.ContainsKey("x-kubernetes-group-version-kind"))
                return GetClassName(definition);

            var schemaName = _schemaToNameMap[definition];

            var parts = schemaName.Split(".");
            var group = parts[parts.Length - 3];
            var version = parts[parts.Length - 2];
            var entityName = parts[parts.Length - 1];
            if (!_schemaDefinitionsInMultipleGroups.Contains(schemaName))
                group = null;
            var className = ToPascalCase($"{group}{version}{entityName}");
            return className;

        }
        static string GetInterfaceName(JsonSchema4 definition)
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
                var schema = itemsProperty.Type == JsonObjectType.Object ? itemsProperty.Reference : itemsProperty.Item.Reference;
                interfaces.Add($"IItems<{GetClassNameForSchemaDefinition(schema)}>");
            }

            if (definition.Properties.TryGetValue("spec", out var specProperty))
            {
                interfaces.Add($"ISpec<{GetClassNameForSchemaDefinition(specProperty.Reference)}>");
            }

            if(_classesWithValidation.Contains(className))
                interfaces.Add("IValidate");
            var result = string.Join(", ", interfaces);
            return result;
        }


        static void GetKind(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
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

        static void GetPlural(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                var plural = GetPlural(arguments[0] as JsonSchema4);
                if(plural != null)
                    context.Write($"\"{plural}\"");
                else
                    context.Write("null");
            }
        }

        private static string GetPlural(JsonSchema4 definition)
        {
            var className = GetClassNameForSchemaDefinition(definition);
            return _classNameToPluralMap.GetValueOrDefault(className, null);
        }

        static void GetGroup(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
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

        static void GetMethodName(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperation)
            {
                context.Write(GetMethodName(arguments[0] as SwaggerOperation));
            }
        }

        static string GetMethodName(SwaggerOperation watchOperation)
        {
            var tag = watchOperation.Tags[0];
            tag = tag.Replace("_", string.Empty);

            var methodName = ToPascalCase(watchOperation.OperationId);

            // This tries to remove the version from the method name, e.g. watchCoreV1NamespacedPod => WatchNamespacedPod
            methodName = methodName.Replace(tag, string.Empty, StringComparison.OrdinalIgnoreCase);
            methodName += "Async";
            return methodName;
        }

        static void GetDotNetType(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerParameter)
            {
                var parameter = arguments[0] as SwaggerParameter;
                context.Write(GetDotNetType(parameter.Type, parameter.Name, parameter.IsRequired));
            }
            else if (arguments != null && arguments.Count > 2 && arguments[0] != null && arguments[1] != null && arguments[2] != null && arguments[0] is JsonObjectType && arguments[1] is string && arguments[2] is bool)
            {
                context.Write(GetDotNetType((JsonObjectType)arguments[0], (string)arguments[1], (bool)arguments[2]));
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

        private static string GetDotNetType(JsonObjectType jsonType, string name, bool required)
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
                    if (required)
                    {
                        return "int";
                    }
                    else
                    {
                        return "int?";
                    }

                case JsonObjectType.String:
                    return "string";

                default:
                    throw new NotSupportedException();
            }
        }

        static void GetDotNetName(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerParameter)
            {
                var parameter = arguments[0] as SwaggerParameter;
                context.Write(GetDotNetName(parameter.Name));
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is string)
            {
                var parameter = arguments[0] as SwaggerParameter;
                context.Write(GetDotNetName((string)arguments[0]));
            }
        }

        private static string GetDotNetName(string jsonName)
        {
            if (jsonName == "namespace")
            {
                return "@namespace";
            }
            else if (jsonName == "continue")
            {
                return "@continue";
            }

            return jsonName;
        }

        static void GetPathExpression(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperationDescription)
            {
                var operation = arguments[0] as SwaggerOperationDescription;
                context.Write(GetPathExpression(operation));
            }
        }

        private static string GetPathExpression(SwaggerOperationDescription operation)
        {
            string pathExpression = operation.Path;

            if (pathExpression.StartsWith("/"))
            {
                pathExpression = pathExpression.Substring(1);
            }

            pathExpression = pathExpression.Replace("{namespace}", "{@namespace}");
            return pathExpression;
        }

        static void GetApiVersion(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema4)
            {
                context.Write(GetApiVersion(arguments[0] as JsonSchema4));
            }
        }

        private static string GetApiVersion(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return groupVersionKind["version"] as string;
        }

        private static string ToPascalCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}
