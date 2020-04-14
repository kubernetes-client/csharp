using System.Runtime.Serialization;

namespace k8s.CustomResources
{
    public enum Scope
    {
        [EnumMember(Value = "Namespaced")] Namespaced,
        [EnumMember(Value = "Cluster")] Cluster
    }
}
