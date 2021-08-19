using Newtonsoft.Json;

namespace k8s.Util.Common.Generic.Options
{
    public class ListOptions
    {
        [JsonProperty("timeoutSeconds")]
        public int? TimeoutSeconds { get; private set; }

        [JsonProperty("limit")]
        public int Limit { get; private set; }

        [JsonProperty("fieldSelector")]
        public string FieldSelector { get; private set; }

        [JsonProperty("labelSelector")]
        public string LabelSelector { get; private set; }

        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; private set; }

        [JsonProperty("continue")]
        public string Continue { get; private set; }

        public ListOptions(int? timeoutSeconds = default, int limit = default, string fieldSelector = default, string labelSelector = default, string resourceVersion = default,
            string @continue = default)
        {
            TimeoutSeconds = timeoutSeconds;
            Limit = limit;
            FieldSelector = fieldSelector;
            LabelSelector = labelSelector;
            ResourceVersion = resourceVersion;
            Continue = @continue;
        }
    }
}
