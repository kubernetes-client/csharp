using System;
using System.Linq;
using System.Text;

namespace k8s
{
    internal static class Utilities
    {
        /// <summary>Given a <see cref="StringBuilder"/> that is building a query string, adds a parameter to it.</summary>
        public static void AddQueryParameter(StringBuilder sb, string key, long? value, bool includeIfDefault = false) =>
            AddQueryParameter(sb, key, value?.ToString(), includeIfDefault);

        public static void AddQueryParameter(StringBuilder sb, string key, int? value, bool includeIfDefault = false) =>
            AddQueryParameter(sb, key, value?.ToString(), includeIfDefault);

        public static void AddQueryParameter(StringBuilder sb, string key, bool? value, bool includeIfDefault = false) =>
            AddQueryParameter(sb, key, value?.ToString().ToLower(), includeIfDefault);

        public static void AddQueryParameter(StringBuilder sb, string key, string value, bool includeIfDefault = false)
        {
            if (!includeIfDefault && string.IsNullOrEmpty(value))
            {
                return;
            }

            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            sb.Append(sb.Length != 0 ? '&' : '?').Append(Uri.EscapeDataString(key)).Append('=');
            if (!string.IsNullOrEmpty(value))
            {
                sb.Append(Uri.EscapeDataString(value));
            }
        }
    }
}
