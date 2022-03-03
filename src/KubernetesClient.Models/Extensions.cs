using System.Reflection;
using System.Text.RegularExpressions;
using k8s.Models;

namespace k8s
{
    public static class Extensions
    {
        public static KubernetesEntityAttribute GetKubernetesTypeMetadata<T>(this T obj)
            where T : IKubernetesObject
            =>
            obj.GetType().GetKubernetesTypeMetadata();
        public static KubernetesEntityAttribute GetKubernetesTypeMetadata(this Type currentType)
        {
            var attr = currentType.GetCustomAttribute<KubernetesEntityAttribute>();
            if (attr == null)
            {
                throw new InvalidOperationException($"Custom resource must have {nameof(KubernetesEntityAttribute)} applied to it");
            }

            return attr;
        }

        public static T Initialize<T>(this T obj)
            where T : IKubernetesObject
        {
            var metadata = obj.GetKubernetesTypeMetadata();
            obj.ApiVersion = !string.IsNullOrEmpty(metadata.Group) ? $"{metadata.Group}/{metadata.ApiVersion}" : metadata.ApiVersion;
            obj.Kind = metadata.Kind ?? obj.GetType().Name;
            if (obj is IMetadata<V1ObjectMeta> withMetadata && withMetadata.Metadata == null)
            {
                withMetadata.Metadata = new V1ObjectMeta();
            }

            return obj;
        }

        internal static bool IsValidKubernetesName(this string value) => !Regex.IsMatch(value, "^[a-z0-9-]+$");
    }
}
