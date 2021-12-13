namespace k8s.KubeConfigModels
{
    public class ExecCredentialResponse
    {
        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("status")]
        public IDictionary<string, string> Status { get; set; }
    }
}
