using System.Collections.Generic;
using System.Linq;
using CaseExtensions;
using NJsonSchema;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    public class ClassNameHelper : INustacheHelper
    {
        private readonly Dictionary<string, string> classNameMap;
        private readonly HashSet<string> schemaDefinitionsInMultipleGroups;
        private readonly Dictionary<JsonSchema, string> schemaToNameMapCooked;
        private readonly Dictionary<JsonSchema, string> schemaToNameMapUnprocessed;

        public ClassNameHelper(OpenApiDocument swaggerCooked, OpenApiDocument swaggerUnprocessed)
        {
            classNameMap = InitClassNameMap(swaggerCooked);

            schemaToNameMapCooked = GenerateSchemaToNameMapCooked(swaggerCooked);
            schemaToNameMapUnprocessed = GenerateSchemaToNameMapUnprocessed(swaggerUnprocessed);
            schemaDefinitionsInMultipleGroups = InitSchemaDefinitionsInMultipleGroups(schemaToNameMapUnprocessed);
        }

        public void RegisterHelper()
        {
            Helpers.Register(nameof(GetClassName), GetClassName);
        }

        private static Dictionary<JsonSchema, string> GenerateSchemaToNameMapUnprocessed(
            OpenApiDocument swaggerUnprocessed)
        {
            return swaggerUnprocessed.Definitions.ToDictionary(x => x.Value, x => x.Key);
        }

        private static Dictionary<JsonSchema, string> GenerateSchemaToNameMapCooked(OpenApiDocument swaggerCooked)
        {
            return swaggerCooked.Definitions.ToDictionary(x => x.Value, x => x.Key.Replace(".", "").ToPascalCase());
        }

        private static HashSet<string> InitSchemaDefinitionsInMultipleGroups(
            Dictionary<JsonSchema, string> schemaToNameMap)
        {
            return schemaToNameMap.Values.Select(x =>
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
        }

        private Dictionary<string, string> InitClassNameMap(OpenApiDocument doc)
        {
            var map = new Dictionary<string, string>();
            foreach (var (k, v) in doc.Definitions)
            {
                if (v.ExtensionData?.TryGetValue("x-kubernetes-group-version-kind", out _) == true)
                {
                    var groupVersionKindElements = (object[])v.ExtensionData["x-kubernetes-group-version-kind"];
                    var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

                    var group = (string)groupVersionKind["group"];
                    var kind = (string)groupVersionKind["kind"];
                    var version = (string)groupVersionKind["version"];
                    map[$"{group}_{kind}_{version}"] = k.Replace(".", "").ToPascalCase();
                }
            }

            return map;
        }

        public void GetClassName(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is OpenApiOperation)
            {
                context.Write(GetClassName(arguments[0] as OpenApiOperation));
            }
            else if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema)
            {
                context.Write(GetClassNameForSchemaDefinition(arguments[0] as JsonSchema));
            }
        }

        public string GetClassName(OpenApiOperation operation)
        {
            var groupVersionKind =
                (Dictionary<string, object>)operation.ExtensionData["x-kubernetes-group-version-kind"];
            return GetClassName(groupVersionKind);
        }

        public string GetClassName(Dictionary<string, object> groupVersionKind)
        {
            var group = (string)groupVersionKind["group"];
            var kind = (string)groupVersionKind["kind"];
            var version = (string)groupVersionKind["version"];

            return classNameMap[$"{group}_{kind}_{version}"];
        }

        public string GetClassName(JsonSchema definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            return GetClassName(groupVersionKind);
        }

        public string GetClassNameForSchemaDefinition(JsonSchema definition)
        {
            if (definition.ExtensionData != null &&
                definition.ExtensionData.ContainsKey("x-kubernetes-group-version-kind"))
            {
                return GetClassName(definition);
            }

            if (schemaToNameMapCooked.TryGetValue(definition, out var name))
            {
                return name;
            }

            var schemaName = schemaToNameMapUnprocessed[definition];

            var parts = schemaName.Split(".");
            var group = parts[parts.Length - 3];
            var version = parts[parts.Length - 2];
            var entityName = parts[parts.Length - 1];
            if (!schemaDefinitionsInMultipleGroups.Contains(schemaName))
            {
                group = null;
            }

            return $"{group}{version}{entityName}".ToPascalCase();
        }

        private static Dictionary<JsonSchema, string> InitSchemaToNameCooked(OpenApiDocument swaggercooked)
        {
            return swaggercooked.Definitions.ToDictionary(x => x.Value, x => x.Key.Replace(".", "").ToPascalCase());
        }
    }
}
