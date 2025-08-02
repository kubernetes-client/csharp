using YamlDotNet.Serialization;

namespace k8s.KubeConfigModels
{
    [YamlSerializable]
    public class ExecCredentialResponse
    {
        public class ExecStatus
        {
#nullable enable
            [JsonPropertyName("expirationTimestamp")]
            public DateTime? ExpirationTimestamp { get; set; }
            [JsonPropertyName("token")]
            public string? Token { get; set; }
            [JsonPropertyName("clientCertificateData")]
            public string? ClientCertificateData { get; set; }
            [JsonPropertyName("clientKeyData")]
            public string? ClientKeyData { get; set; }
#nullable disable

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(Token) ||
                        (!string.IsNullOrEmpty(ClientCertificateData) && !string.IsNullOrEmpty(ClientKeyData));
            }
        }

        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("status")]
        public ExecStatus Status { get; set; }
    }
}
