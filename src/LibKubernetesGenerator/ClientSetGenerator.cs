using CaseExtensions;
using Microsoft.CodeAnalysis;
using NSwag;
using System.Collections.Generic;
using System.Linq;

namespace LibKubernetesGenerator
{
    internal class ClientSetGenerator
    {
        private readonly ScriptObjectFactory _scriptObjectFactory;

        public ClientSetGenerator(ScriptObjectFactory scriptObjectFactory)
        {
            _scriptObjectFactory = scriptObjectFactory;
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
                            p.Name += i++;
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

            var sc = _scriptObjectFactory.CreateScriptObject();

            var groups = new List<string>();
            var apiGroups = new Dictionary<string, OpenApiOperationDescription[]>();

            foreach (var grouped in data.Where(d => HasKubernetesAction(d.Operation?.ExtensionData))
                         .GroupBy(d => d.Operation.Tags.First()))
            {
                var clients = new List<string>();
                var name = grouped.Key.ToPascalCase();
                groups.Add(name);
                var apis = grouped.Select(x =>
                {
                    var groupVersionKindElements = x.Operation?.ExtensionData?["x-kubernetes-group-version-kind"];
                    var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements;

                    return new { Kind = groupVersionKind?["kind"], Api = x };

                });

                foreach (var item in apis.GroupBy(x => x.Kind))
                {
                    var kind = item.Key as string;
                    apiGroups[kind] = item.Select(x => x.Api).ToArray();
                    clients.Add(kind);
                }

                sc.SetValue("clients", clients, true);
                sc.SetValue("name", name, true);
                context.RenderToContext("GroupClient.cs.template", sc, $"{name}GroupClient.g.cs");
            }

            foreach (var apiGroup in apiGroups)
            {
                var name = apiGroup.Key;
                var apis = apiGroup.Value.ToArray();

                sc.SetValue("apis", apis, true);
                sc.SetValue("name", name, true);
                context.RenderToContext("Client.cs.template", sc, $"{name}Client.g.cs");
                context.RenderToContext("ClientExtensions.cs.template", sc, $"{name}ClientExtensions.g.cs");
            }

            sc = _scriptObjectFactory.CreateScriptObject();
            sc.SetValue("groups", groups, true);

            context.RenderToContext("ClientSet.cs.template", sc, $"ClientSet.g.cs");
        }

        private bool HasKubernetesAction(IDictionary<string, object> extensionData) =>
            extensionData?.ContainsKey("x-kubernetes-action") ?? false;
    }
}
