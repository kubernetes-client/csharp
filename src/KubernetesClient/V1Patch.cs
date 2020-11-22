using System;
using Newtonsoft.Json;

namespace k8s.Models
{
    [JsonConverter(typeof(V1PathJsonConverter))]
    public partial class V1Patch
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
        }

        public PatchType Type { get; private set; }

        public V1Patch(object body, PatchType type)
        {
            Content = body;
            Type = type;
            CustomInit();
        }

        partial void CustomInit()
        {
            if (Content == null)
            {
                throw new ArgumentNullException(nameof(Content), "object must be set");
            }

            if (Type == PatchType.Unknown)
            {
                throw new ArgumentException("patch type must be set", nameof(Type));
            }
        }
    }
}
