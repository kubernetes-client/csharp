namespace k8s.KubeConfigModels
{
    public class ExecCredentialResponse
    {
        public class ExecStatus
        {
            #nullable enable
            public DateTime? Expiry { get; set; }
            public string? Token { get; set; }
            public string? ClientCertificateData { get; set; }
            public string? ClientKeyData { get; set; }
            #nullable disable

            public bool IsValid()
            {
                return (!string.IsNullOrEmpty(Token) ||
                        (!string.IsNullOrEmpty(ClientCertificateData) && !string.IsNullOrEmpty(ClientKeyData)));
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
