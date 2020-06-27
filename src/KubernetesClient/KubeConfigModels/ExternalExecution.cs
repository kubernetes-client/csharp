using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    public class ExternalExecution
    {
        [YamlMember(Alias = "apiVersion")] public string ApiVersion { get; set; }

        /// <summary>
        /// The command to execute. Required.
        /// </summary>
        [YamlMember(Alias = "command")]
        public string Command { get; set; }

        /// <summary>
        /// Environment variables to set when executing the plugin. Optional.
        /// </summary>
        [YamlMember(Alias = "env")]
        public IList<Dictionary<string, string>> EnvironmentVariables { get; set; }

        /// <summary>
        /// Arguments to pass when executing the plugin. Optional.
        /// </summary>
        [YamlMember(Alias = "args")]
        public IList<string> Arguments { get; set; }
    }
}
