using System;
using System.Text;

namespace k8s
{
    internal static class Utilities
    {
        /// <summary>Given a <see cref="StringBuilder"/> that is building a query string, adds a parameter to it.</summary>
        public static void AddQueryParameter(StringBuilder sb, string key, string value)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            sb.Append(sb.Length != 0 ? '&' : '?').Append(Uri.EscapeDataString(key)).Append('=');
            if (!string.IsNullOrEmpty(value)) sb.Append(Uri.EscapeDataString(value));
        }
    }
}
