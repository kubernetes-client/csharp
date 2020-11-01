using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    /// <summary>
    /// Relates nicknames to cluster information.
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Gets or sets the cluster information.
        /// </summary>
        [YamlMember(Alias = "cluster")]
        public ClusterEndpoint ClusterEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the nickname for this Cluster.
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
    }
}
