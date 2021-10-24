using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSwag;
using Nustache.Core;

namespace KubernetesGenerator
{
    internal class ApiGenerator
    {
        public void Generate(OpenApiDocument swagger, string outputDirectory)
        {
            var data = swagger.Operations
                .Where(o => o.Method != OpenApiOperationMethod.Options)
                .GroupBy(o => o.Operation.OperationId)
                .Select(g =>
                {
                    var gs = g.ToArray();

                    for (var i = 1; i < g.Count(); i++)
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
                .Select(o =>
                {
                    o.Path = o.Path.TrimStart('/');
                    o.Method = char.ToUpper(o.Method[0]) + o.Method.Substring(1);
                    return o;
                })
                .ToArray();

            Render.FileToFile(Path.Combine("templates", "IKubernetes.cs.template"), data,
                Path.Combine(outputDirectory, "IKubernetes.cs"));
            Render.FileToFile(Path.Combine("templates", "Kubernetes.cs.template"), data,
                Path.Combine(outputDirectory, "Kubernetes.cs"));
            Render.FileToFile(Path.Combine("templates", "KubernetesExtensions.cs.template"), data,
                Path.Combine(outputDirectory, "KubernetesExtensions.cs"));
        }
    }
}
