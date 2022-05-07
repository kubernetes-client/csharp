using System.Collections.Generic;
using System.Linq;
using CaseExtensions;
using Microsoft.CodeAnalysis;
using NSwag;

namespace LibKubernetesGenerator
{
    internal class ApiGenerator
    {
        public void Generate(OpenApiDocument swagger, GeneratorExecutionContext context)
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

            var groups = new List<string>();

            foreach (var grouped in data.GroupBy(d => d.Operation.Tags.First()))
            {
                var name = grouped.Key.ToPascalCase();
                groups.Add(name);

                var apis = grouped.ToArray();
                var gctx = new { name, apis };
                context.RenderToContext($"IOperations.cs.template", gctx, $"I{name}Operations.g.cs");
                context.RenderToContext("Operations.cs.template", gctx, $"{name}Operations.g.cs");
                context.RenderToContext("OperationsExtensions.cs.template", gctx, $"{name}OperationsExtensions.g.cs");
            }

            context.RenderToContext($"IBasicKubernetes.cs.template", groups, $"IBasicKubernetes.g.cs");
            context.RenderToContext($"AbstractKubernetes.cs.template", groups, $"AbstractKubernetes.g.cs");
        }
    }
}
