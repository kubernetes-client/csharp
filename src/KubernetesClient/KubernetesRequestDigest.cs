// Derived from
// https://github.com/kubernetes-client/java/blob/master/util/src/main/java/io/kubernetes/client/apimachinery/KubernetesResource.java
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

namespace k8s
{
    public class KubernetesRequestDigest
    {
        private static Regex resourcePattern =
            new Regex(@"^/(api|apis)(/\S+)?/v\d\w*/\S+", RegexOptions.Compiled);

        public string Path { get; }
        public bool IsNonResourceRequest { get; }
        public string ApiGroup { get; }
        public string ApiVersion { get; }
        public string Kind { get; }
        public string Verb { get; }

        public KubernetesRequestDigest(string urlPath, bool isNonResourceRequest, string apiGroup, string apiVersion, string kind, string verb)
        {
            this.Path = urlPath;
            this.IsNonResourceRequest = isNonResourceRequest;
            this.ApiGroup = apiGroup;
            this.ApiVersion = apiVersion;
            this.Kind = kind;
            this.Verb = verb;
        }

        public static KubernetesRequestDigest Parse(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string urlPath = request.RequestUri.AbsolutePath;
            if (!IsResourceRequest(urlPath))
            {
                return NonResource(urlPath);
            }

            try
            {
                string apiGroup;
                string apiVersion;
                string kind;

                var parts = urlPath.Split('/');
                var namespaced = urlPath.IndexOf("/namespaces/", StringComparison.Ordinal) != -1;

                if (urlPath.StartsWith("/api/v1", StringComparison.Ordinal))
                {
                    apiGroup = "";
                    apiVersion = "v1";

                    if (namespaced)
                    {
                        kind = parts[5];
                    }
                    else
                    {
                        kind = parts[3];
                    }
                }
                else
                {
                    apiGroup = parts[2];
                    apiVersion = parts[3];
                    if (namespaced)
                    {
                        kind = parts[6];
                    }
                    else
                    {
                        kind = parts[4];
                    }
                }

                return new KubernetesRequestDigest(
                    urlPath,
                    false,
                    apiGroup,
                    apiVersion,
                    kind,
                    HasWatchParameter(request) ? "WATCH" : request.Method.ToString());
            }
            catch (Exception)
            {
                return NonResource(urlPath);
            }
        }

        private static KubernetesRequestDigest NonResource(string urlPath)
        {
            KubernetesRequestDigest digest = new KubernetesRequestDigest(urlPath, true, "nonresource", "na", "na", "na");
            return digest;
        }

        public static bool IsResourceRequest(string urlPath)
        {
            return resourcePattern.Matches(urlPath).Count > 0;
        }

        private static bool HasWatchParameter(HttpRequestMessage request)
        {
            return !string.IsNullOrEmpty(HttpUtility.ParseQueryString(request.RequestUri.Query).Get("watch"));
        }
    }
}
