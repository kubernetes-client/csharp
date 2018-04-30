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
            // Initialize variables - such as the Kubernetes branch for which to generate the API.
            string kubernetesBranch = "v1.10.0";

            if (Environment.GetEnvironmentVariable("KUBERNETES_BRANCH") != null)
            {
                kubernetesBranch = Environment.GetEnvironmentVariable("KUBERNETES_BRANCH");

                Console.WriteLine($"Using Kubernetes branch {kubernetesBranch}, as set by the KUBERNETES_BRANCH environment variable");
            }

            const string outputDirectory = "../../../../../src/KubernetesClient/generated/";

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

            // We skip operations where the name of the class in the C# client could not be determined correctly.
            // That's usually because there are different version of the same object (e.g. for deployments).
            Collection<string> blacklistedOperations = new Collection<string>()
            {
                "watchAppsV1beta1NamespacedDeployment",
                "watchAppsV1beta2NamespacedDeployment",
                "watchExtensionsV1beta1NamespacedDeployment",
                "watchExtensionsV1beta1NamespacedNetworkPolicy",
                "watchPolicyV1beta1PodSecurityPolicy",
                "watchExtensionsV1beta1PodSecurityPolicy"
            };

            var watchOperations = swagger.Operations.Where(
                o => o.Path.Contains("/watch/")
                && o.Operation.ActualParameters.Any(p => p.Name == "name")
                && !blacklistedOperations.Contains(o.Operation.OperationId)).ToArray();

            // Register helpers used in the templating.
            Helpers.Register(nameof(ToXmlDoc), ToXmlDoc);
            Helpers.Register(nameof(ClassName), ClassName);
            Helpers.Register(nameof(MethodName), MethodName);
            Helpers.Register(nameof(GetDotNetName), GetDotNetName);
            Helpers.Register(nameof(GetDotNetType), GetDotNetType);
            Helpers.Register(nameof(GetPathExpression), GetPathExpression);

            // Render.
            Render.FileToFile("IKubernetes.Watch.cs.template", watchOperations, $"{outputDirectory}IKubernetes.Watch.cs");
            Render.FileToFile("Kubernetes.Watch.cs.template", watchOperations, $"{outputDirectory}Kubernetes.Watch.cs");
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

        static void MethodName(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperation)
            {
                context.Write(MethodName(arguments[0] as SwaggerOperation));
            }
        }

        static string MethodName(SwaggerOperation watchOperation)
        {
            var tag = watchOperation.Tags[0];
            tag = tag.Replace("_", string.Empty);

            var methodName = ToPascalCase(watchOperation.OperationId);

            // This tries to remove the version from the method name, e.g. watchCoreV1NamespacedPod => WatchNamespacedPod
            methodName = methodName.Replace(tag, string.Empty, StringComparison.OrdinalIgnoreCase);
            methodName += "Async";
            return methodName;
        }

        static void ClassName(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerOperation)
            {
                context.Write(ClassName(arguments[0] as SwaggerOperation));
            }
        }

        static string ClassName(SwaggerOperation watchOperation)
        {
            var groupVersionKind = (Dictionary<string, object>)watchOperation.ExtensionData["x-kubernetes-group-version-kind"];
            var group = (string)groupVersionKind["group"];
            var kind = (string)groupVersionKind["kind"];
            var version = (string)groupVersionKind["version"];

            var className = $"{ToPascalCase(version)}{kind}";
            return className;
        }

        static void GetDotNetType(RenderContext context, IList<object> arguments, IDictionary<string, object> options, RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is SwaggerParameter)
            {
                var parameter = arguments[0] as SwaggerParameter;
                context.Write(GetDotNetType(parameter.Type, parameter.Name, parameter.IsRequired));
            }
            else if(arguments != null && arguments.Count > 2 && arguments[0] != null && arguments[1] != null && arguments[2] != null && arguments[0] is JsonObjectType && arguments[1] is string && arguments[2] is bool)
            {
                context.Write(GetDotNetType((JsonObjectType)arguments[0], (string)arguments[1], (bool)arguments[2]));
            }
            else if(arguments != null && arguments.Count > 0 && arguments[0] != null)
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
            pathExpression = pathExpression.Replace("{namespace}", "{@namespace}");
            return pathExpression;
        }

        private static string ToPascalCase(string name)
        {
            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}
