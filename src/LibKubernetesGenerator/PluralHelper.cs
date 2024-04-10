using NJsonSchema;
using NSwag;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibKubernetesGenerator
{
    internal class PluralHelper : IScriptObjectHelper
    {
        private readonly Dictionary<string, string> _classNameToPluralMap;
        private readonly ClassNameHelper classNameHelper;
        private readonly HashSet<string> opblackList = new HashSet<string>()
        {
            "listClusterCustomObject",
            "listNamespacedCustomObject",
        };

        public PluralHelper(ClassNameHelper classNameHelper, OpenApiDocument swagger)
        {
            this.classNameHelper = classNameHelper;
            _classNameToPluralMap = InitClassNameToPluralMap(swagger);
        }

        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(GetPlural), new Func<JsonSchema, string>(GetPlural));
        }

        public string GetPlural(JsonSchema definition)
        {
            var className = classNameHelper.GetClassNameForSchemaDefinition(definition);
            if (_classNameToPluralMap.TryGetValue(className, out var plural))
            {
                return plural;
            }

            return null;
        }

        private Dictionary<string, string> InitClassNameToPluralMap(OpenApiDocument swagger)
        {
            var classNameToPluralMap = swagger.Operations
                .Where(x => x.Operation.OperationId.StartsWith("list", StringComparison.InvariantCulture))
                .Where(x => !opblackList.Contains(x.Operation.OperationId))
                .Select(x => new
                {
                    PluralName = x.Path.Split('/').Last(),
                    ClassName = classNameHelper.GetClassNameForSchemaDefinition(x.Operation.Responses["200"]
                        .ActualResponse.Schema.ActualSchema),
                })
                .Distinct()
                .ToDictionary(x => x.ClassName, x => x.PluralName);

            // dictionary only contains "list" plural maps. assign the same plural names to entities those lists support
            classNameToPluralMap = classNameToPluralMap
                .Where(x => x.Key.EndsWith("List", StringComparison.InvariantCulture))
                .Select(x =>
                    new { ClassName = x.Key.Remove(x.Key.Length - 4), PluralName = x.Value })
                .ToDictionary(x => x.ClassName, x => x.PluralName)
                .Union(classNameToPluralMap)
                .ToDictionary(x => x.Key, x => x.Value);

            return classNameToPluralMap;
        }
    }
}
