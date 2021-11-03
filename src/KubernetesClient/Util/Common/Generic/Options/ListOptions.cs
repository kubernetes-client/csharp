using Newtonsoft.Json;

namespace k8s.Util.Common.Generic.Options
{
    public class ListOptions
    {
        public int? TimeoutSeconds { get; private set; }

        public int Limit { get; private set; }

        public string FieldSelector { get; private set; }

        public string LabelSelector { get; private set; }

        public string ResourceVersion { get; private set; }

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
