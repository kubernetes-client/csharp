using System.Collections.Generic;
using Newtonsoft.Json;

namespace k8s.KubeConfigModels
{
    public class ExecCredentialResponse
    {
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("status")]
        public IDictionary<string, string> Status { get; set; }
    }
}
