using CaseExtensions;
using Microsoft.CodeAnalysis;
using NSwag;
using System.Collections.Generic;
using System.Linq;

namespace LibKubernetesGenerator
{
    internal class ApiGenerator
    {
        private readonly ScriptObjectFactory scriptObjectFactory;

        public ApiGenerator(ScriptObjectFactory scriptObjectFactory)
        {
            this.scriptObjectFactory = scriptObjectFactory;
        }

        public void Generate(OpenApiDocument swagger, IncrementalGeneratorPostInitializationContext context)
        {
            var data = swagger.Operations
                .Where(o => o.Method != OpenApiOperationMethod.Options)
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
                .Select(o =>
                {
                    o.Path = o.Path.TrimStart('/');
                    o.Method = char.ToUpper(o.Method[0]) + o.Method.Substring(1);
                    return o;
                })
                .ToArray();

            var sc = scriptObjectFactory.CreateScriptObject();

            var groups = new List<string>();

            foreach (var grouped in data.GroupBy(d => d.Operation.Tags.First()))
            {
                var name = grouped.Key.ToPascalCase();
                groups.Add(name);

                var apis = grouped.ToArray();

                sc.SetValue("name", name, true);
                sc.SetValue("apis", apis, true);

                context.RenderToContext($"IOperations.cs.template", sc, $"I{name}Operations.g.cs");
                context.RenderToContext("Operations.cs.template", sc, $"{name}Operations.g.cs");
                context.RenderToContext("OperationsExtensions.cs.template", sc, $"{name}OperationsExtensions.g.cs");
            }

            sc = scriptObjectFactory.CreateScriptObject();
            sc.SetValue("groups", groups, true);

            context.RenderToContext($"IBasicKubernetes.cs.template", sc, $"IBasicKubernetes.g.cs");
            context.RenderToContext($"AbstractKubernetes.cs.template", sc, $"AbstractKubernetes.g.cs");
        }
    }
}
