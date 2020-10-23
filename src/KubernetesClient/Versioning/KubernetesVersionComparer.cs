using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace k8s.Versioning
{
    public class KubernetesVersionComparer : IComparer<string>
    {
        public static KubernetesVersionComparer Instance { get; private set; }
        private static readonly Regex _kubernetesVersionRegex;

        static KubernetesVersionComparer()
        {
            _kubernetesVersionRegex = new Regex(@"^v(?<major>[0-9]+)((?<stream>alpha|beta)(?<minor>[0-9]+))?$", RegexOptions.Compiled);
            Instance = new KubernetesVersionComparer();
        }

        internal KubernetesVersionComparer()
        {
        }

        public int Compare(string x, string y)
        {
            if (x == null || y == null)
            {
                return StringComparer.CurrentCulture.Compare(x, y);
            }

            var matchX = _kubernetesVersionRegex.Match(x);
            if (!matchX.Success)
            {
                return StringComparer.CurrentCulture.Compare(x, y);
            }

            var matchY = _kubernetesVersionRegex.Match(y);
            if (!matchY.Success)
            {
                return StringComparer.CurrentCulture.Compare(x, y);
            }

            var versionX = ExtractVersion(matchX);
            var versionY = ExtractVersion(matchY);
            return versionX.CompareTo(versionY);
        }

        private Version ExtractVersion(Match match)
        {
            var major = int.Parse(match.Groups["major"].Value);
            if (!Enum.TryParse<Stream>(match.Groups["stream"].Value, true, out var stream))
            {
                stream = Stream.Final;
            }

            int.TryParse(match.Groups["minor"].Value, out var minor);
            return new Version(major, (int)stream, minor);
        }

        private enum Stream
        {
            Alpha = 1,
            Beta = 2,
            Final = 3,
        }
    }
}
