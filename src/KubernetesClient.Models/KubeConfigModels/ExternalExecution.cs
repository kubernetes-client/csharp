using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    public class ExternalExecution
    {
        [YamlMember(Alias = "apiVersion")]
        public string ApiVersion { get; set; }

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

        /// <summary>
        /// Text shown to the user when the executable doesn't seem to be present. Optional.
        /// </summary>
        [YamlMember(Alias = "installHint")]
        public string InstallHint { get; set; }

        /// <summary>
        /// Whether or not to provide cluster information to this exec plugin as a part of
        /// the KUBERNETES_EXEC_INFO environment variable. Optional.
        /// </summary>
        [YamlMember(Alias = "provideClusterInfo")]
        public bool ProvideClusterInfo { get; set; }
    }
}
