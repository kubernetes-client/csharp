using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using k8s.Models;
using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json.Converters;
using VersionConverter = k8s.Versioning.VersionConverter;

namespace k8s
{
    public static class Extensions
    {
        public static KubernetesEntityAttribute GetKubernetesTypeMetadata<T>(this T obj) where T : IKubernetesObject
        {
            return obj.GetType().GetKubernetesTypeMetadata();
        }

        public static KubernetesEntityAttribute GetKubernetesTypeMetadata(this Type currentType)
        {
            var attr = currentType.GetCustomAttribute<KubernetesEntityAttribute>();
            if (attr == null)
            {
                throw new InvalidOperationException($"Custom resource must have {nameof(KubernetesEntityAttribute)} applied to it");
            }

            return attr;
        }

        public static T Initialize<T>(this T obj) where T : IKubernetesObject
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

        internal static bool IsValidKubernetesName(this string value)
        {
            return !Regex.IsMatch(value, "^[a-z0-9-]+$");
        }

        // Convert the string to camel case.
        public static string ToCamelCase(this string value)
        {
            // If there are 0 or 1 characters, just return the string.
            if (value == null || value.Length < 2)
            {
                return value;
            }

            // Split the string into words.
            var words = value.Split(
                new char[0],
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            var result = words[0].ToLower();
            for (var i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        public static bool In<T>(this T obj, params T[] values) => values.Contains(obj);

    }
}
