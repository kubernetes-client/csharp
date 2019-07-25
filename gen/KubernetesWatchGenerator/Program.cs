using NJsonSchema;
using NSwag;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KubernetesWatchGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine($"usage {args[0]} path/to/csharp.settings");
                Environment.Exit(1);
            }

            var (kubernetesBranch, outputDirectory) = LoadSettings(args[1]);

            var specUrl = $"https://raw.githubusercontent.com/kubernetes/kubernetes/{kubernetesBranch}/api/openapi-spec/swagger.json";
            var specPath = $"{kubernetesBranch}-swagger.json";

            // Download the Kubernetes spec, and cache it locally. Don't download it if already present in the cache.
            if (!File.Exists(specPath))
            {
                HttpClient client = new HttpClient();
                using (var response = await client.GetAsync(specUrl, HttpCompletionOption.ResponseHeadersRead))
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var output = File.Open(specPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await stream.CopyToAsync(output);
                }
            }

            // Read the spec
            var swagger = await SwaggerDocument.FromFileAsync(specPath);


            // Register helpers used in the templating.
            Helpers.Register(nameof(ToXmlDoc), ToXmlDoc);
            Helpers.Register(nameof(GetClassName), GetClassName);
            Helpers.Register(nameof(GetMethodName), GetMethodName);
            Helpers.Register(nameof(GetDotNetName), GetDotNetName);
            Helpers.Register(nameof(GetDotNetType), GetDotNetType);
            Helpers.Register(nameof(GetPathExpression), GetPathExpression);
            Helpers.Register(nameof(GetGroup), GetGroup);
            Helpers.Register(nameof(GetApiVersion), GetApiVersion);
            Helpers.Register(nameof(GetKind), GetKind);

            // Generate the Watcher operations
            // We skip operations where the name of the class in the C# client could not be determined correctly.
            // That's usually because there are different version of the same object (e.g. for deployments).
            var blacklistedOperations = new HashSet<string>()
            {
                "watchAppsV1beta1NamespacedDeployment",
                "watchAppsV1beta2NamespacedDeployment",
                "watchExtensionsV1beta1NamespacedDeployment",
                "watchExtensionsV1beta1NamespacedNetworkPolicy",
                "watchPolicyV1beta1PodSecurityPolicy",
                "watchExtensionsV1beta1PodSecurityPolicy",
                "watchExtensionsV1beta1NamespacedIngress",
                "watchNamespacedIngress",
                "watchExtensionsV1beta1NamespacedIngressList",
                "watchNetworkingV1beta1NamespacedIngress",
                "watchNetworkingV1beta1NamespacedIngressList",
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
                "V1beta1Deployment",
                "V1beta1DeploymentList",
                "V1beta1DeploymentRollback",
                "V1beta1DeploymentRollback",
                "V1beta1Scale",
                "V1beta1PodSecurityPolicy",
                "V1beta1PodSecurityPolicyList",
                "V1WatchEvent",
                "V1beta1Ingress",
                "V1beta1IngressList"
            };

            var definitions = swagger.Definitions.Values
                .Where(
                    d => d.ExtensionData != null
                    && d.ExtensionData.ContainsKey("x-kubernetes-group-version-kind")
                    && !skippedTypes.Contains(GetClassName(d)));

            // Render.
            Render.FileToFile("ModelExtensions.cs.template", definitions, $"{outputDirectory}ModelExtensions.cs");
        }

        private static (string kubernetesBranch, string outputDirectory) LoadSettings(string path)
        {
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                Console.Error.WriteLine("Cannot find csharp.settings");
                Environment.Exit(1);
            }

            using (var s = new StreamReader(fileInfo.OpenRead()))
            {
                string l;
                while ((l = s.ReadLine()) != null)
                {
                    if (l.Contains("KUBERNETES_BRANCH"))
                    {
                        var kubernetesBranch = l.Split("=")[1];
                        var outputDirectory = Path.Combine(fileInfo.DirectoryName, @"src/KubernetesClient/generated/");

                        Console.WriteLine($"Using branch {kubernetesBranch} output {outputDirectory}");

                        return (kubernetesBranch, outputDirectory);
                    }
                }
            }

            Console.Error.WriteLine("Cannot find KUBERNETES_BRANCH in csharp.settings");
            Environment.Exit(1);
            return (null, null);
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
                            context.Write(Environment.NewLine);
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
            var group = (string)groupVersionKind["group"];
            var kind = (string)groupVersionKind["kind"];
            var version = (string)groupVersionKind["version"];

            var className = $"{ToPascalCase(version)}{kind}";
            return className;
        }

        private static string GetClassName(JsonSchema4 definition)
        {
            var groupVersionKindElements = (object[])definition.ExtensionData["x-kubernetes-group-version-kind"];
            var groupVersionKind = (Dictionary<string, object>)groupVersionKindElements[0];

            var group = groupVersionKind["group"] as string;
            var version = groupVersionKind["version"] as string;
            var kind = groupVersionKind["kind"] as string;

            return $"{ToPascalCase(version)}{ToPascalCase(kind)}";
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

            if(pathExpression.StartsWith("/"))
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
