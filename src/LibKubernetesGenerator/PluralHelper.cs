using System;
using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NSwag;
using Nustache.Core;

namespace LibKubernetesGenerator
{
    internal class PluralHelper : INustacheHelper
    {
        private readonly Dictionary<string, string> _classNameToPluralMap;
        private readonly ClassNameHelper classNameHelper;
        private HashSet<string> opblackList = new HashSet<string>()
        {
            "listClusterCustomObject",
            "listNamespacedCustomObject",
        };

        public PluralHelper(ClassNameHelper classNameHelper, OpenApiDocument swagger)
        {
            this.classNameHelper = classNameHelper;
            _classNameToPluralMap = InitClassNameToPluralMap(swagger);
        }

        public void RegisterHelper()
        {
            Helpers.Register(nameof(GetPlural), GetPlural);
        }

        public void GetPlural(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is JsonSchema)
            {
                var plural = GetPlural(arguments[0] as JsonSchema);
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
