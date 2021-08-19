using System.IO;
using System.Text;

namespace k8s.Util.Common
{
    /// <summary>
    /// Namespaces provides a set of helpers for operating namespaces.
    /// </summary>
    public class Namespaces
    {
        public const string NamespaceAll = "";

        public const string NamespaceDefault = "default";

        public const string NamespaceKubeSystem = "kube-system";

        public static string GetPodNamespace()
        {
            return File.ReadAllText(Config.ServiceAccountNamespacePath, Encoding.UTF8);
        }
    }
}
