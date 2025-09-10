namespace k8s.Models
{
    [JsonConverter(typeof(V1PatchJsonConverter))]
    public record V1Patch
    {
        public enum PatchType
        {
            /// <summary>
            /// not set, this is not allowed
            /// </summary>
            Unknown,

            /// <summary>
            /// content type application/json-patch+json
            /// </summary>
            JsonPatch,

            /// <summary>
            /// content type application/merge-patch+json
            /// </summary>
            MergePatch,

            /// <summary>
            /// content type application/strategic-merge-patch+json
            /// </summary>
            StrategicMergePatch,

            /// <summary>
            /// content type application/apply-patch+yaml
            /// </summary>
            ApplyPatch,
        }

        [JsonPropertyName("content")]
        [JsonInclude]
        public object Content { get; private set; }

        public PatchType Type { get; private set; }

        public V1Patch(object body, PatchType type)
        {
            if (type == PatchType.Unknown)
            {
                throw new ArgumentException("patch type must be set", nameof(type));
            }

            Content = body ?? throw new ArgumentNullException(nameof(body), "object must be set");
            Type = type;
        }
    }
}
