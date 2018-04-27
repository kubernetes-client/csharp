using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KubernetesWatchGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Download the latest spec
            const string kubernetesBranch = "v1.10.0";
            const string outputDirectory = "../../../../../src/KubernetesClient/generated/";

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

            var specUrl = $"https://raw.githubusercontent.com/kubernetes/kubernetes/{kubernetesBranch}/api/openapi-spec/swagger.json";
            var specPath = $"{kubernetesBranch}-swagger.json";

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

            var swagger = await SwaggerDocument.FromFileAsync(specPath);

            var watchOperations = swagger.Operations.Where(
                o => o.Path.Contains("/watch/")
                && o.Operation.ActualParameters.Any(p => p.Name == "name")
                && !blacklistedOperations.Contains(o.Operation.OperationId)).ToArray();

            using (Stream interfaceFile = File.Open($"{outputDirectory}IKubernetes.Watch.cs", FileMode.Create, FileAccess.ReadWrite))
            using (StreamWriter interfaceWriter = new StreamWriter(interfaceFile, Encoding.UTF8))
            using (Stream classFile = File.Open($"{outputDirectory}Kubernetes.Watch.cs", FileMode.Create, FileAccess.ReadWrite))
            using (StreamWriter classWriter = new StreamWriter(classFile, Encoding.UTF8))
            {
                interfaceWriter.WriteLine("using k8s.Models;");
                interfaceWriter.WriteLine("using System; ");
                interfaceWriter.WriteLine("using System.Collections.Generic; ");
                interfaceWriter.WriteLine("using System.Threading; ");
                interfaceWriter.WriteLine("using System.Threading.Tasks; ");
                interfaceWriter.WriteLine("");
                interfaceWriter.WriteLine("namespace k8s");
                interfaceWriter.WriteLine("{");
                interfaceWriter.WriteLine("    public partial interface IKubernetes");
                interfaceWriter.WriteLine("    {");

                classWriter.WriteLine("using k8s.Models;");
                classWriter.WriteLine("using System; ");
                classWriter.WriteLine("using System.Collections.Generic; ");
                classWriter.WriteLine("using System.Threading; ");
                classWriter.WriteLine("using System.Threading.Tasks; ");
                classWriter.WriteLine("");
                classWriter.WriteLine("namespace k8s");
                classWriter.WriteLine("{");
                classWriter.WriteLine("    public partial class Kubernetes");
                classWriter.WriteLine("    {");

                foreach (var watchOperation in watchOperations)
                {
                    var groupVersionKind = (Dictionary<string, object>)watchOperation.Operation.ExtensionData["x-kubernetes-group-version-kind"];
                    var group = (string)groupVersionKind["group"];
                    var kind = (string)groupVersionKind["kind"];
                    var version = (string)groupVersionKind["version"];
                    var tag = watchOperation.Operation.Tags[0];
                    tag = tag.Replace("_", string.Empty);

                    var className = $"{ToPascalCase(version)}{kind}";
                    var methodName = ToPascalCase(watchOperation.Operation.OperationId);

                    // This tries to remove the version from the method name, e.g. watchCoreV1NamespacedPod => WatchNamespacedPod
                    methodName = methodName.Replace(tag, string.Empty, StringComparison.OrdinalIgnoreCase);
                    methodName += "Async";

                    // Generate the signature and the method definition in the interface
                    string signature = $"Task<Watcher<{className}>> {methodName}(";
                    bool firstParam = true;

                    interfaceWriter.WriteLine("        /// <summary>");

                    using (StringReader reader = new StringReader(watchOperation.Operation.Description))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            interfaceWriter.WriteLine($"        /// {line}");
                        }
                    }

                    interfaceWriter.WriteLine("        /// </summary>");

                    foreach (var param in watchOperation.Operation.ActualParameters.OrderByDescending(p => p.IsRequired).ThenBy(p => p.Name))
                    {
                        if (param.Name == "watch")
                        {
                            continue;
                        }

                        interfaceWriter.WriteLine($"        /// <param name=\"{param.Name}\">");
                        using (StringReader reader = new StringReader(param.Description))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                interfaceWriter.WriteLine($"        /// {line}");
                            }
                        }

                        interfaceWriter.WriteLine("        /// </param>");

                        if (firstParam)
                        {
                            firstParam = false;
                        }
                        else
                        {
                            signature += ", ";
                        }

                        signature += $"{GetDotNetType(param.Type, param.Name, param.IsRequired)} {GetDotNetName(param.Name)}";

                        if (!param.IsRequired)
                        {
                            signature += " = null";
                        }
                    }

                    interfaceWriter.WriteLine(
@"        /// <param name=""customHeaders"">
        /// The headers that will be added to request.
        /// </param>
        /// <param name=""onEvent"" >
        /// The action to invoke when the server sends a new event.
        /// </param>
        /// <param name=""onError"" >
        /// The action to invoke when an error occurs.
        /// </param>
        /// <param name=""cancellationToken"" >
        /// A <see cref=""CancellationToken""/> which can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref=""Task"" /> which represents the asynchronous operation, and returns a new watcher.
        /// </returns>");
                    signature += $", Dictionary<string, List<string>> customHeaders = null, Action<WatchEventType, {className}> onEvent = null, Action<Exception> onError = null, CancellationToken cancellationToken = default(CancellationToken))";

                    interfaceWriter.WriteLine($"        {signature};");
                    interfaceWriter.WriteLine();

                    string pathExpression = watchOperation.Path;
                    pathExpression = pathExpression.Replace("{namespace}", "{@namespace}");

                    // Generate the method implementation in the class.
                    classWriter.WriteLine("        /// <inheritdoc/>");
                    classWriter.WriteLine($"        public {signature}");
                    classWriter.WriteLine("        {");
                    classWriter.WriteLine($"            string path = $\"{pathExpression}\";");
                    classWriter.WriteLine($"            return WatchObjectAsync<{className}>(path: path, @continue: @continue, fieldSelector: fieldSelector, includeUninitialized: includeUninitialized, labelSelector: labelSelector, limit: limit, pretty: pretty, timeoutSeconds: timeoutSeconds, resourceVersion: resourceVersion, customHeaders: customHeaders, onEvent: onEvent, onError: onError, cancellationToken: cancellationToken);");
                    classWriter.WriteLine("        }");
                    classWriter.WriteLine();
                }

                interfaceWriter.WriteLine("    }");
                interfaceWriter.WriteLine("}");

                classWriter.WriteLine("    }");
                classWriter.WriteLine("}");
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

        private static string ToPascalCase(string name)
        {
            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}
