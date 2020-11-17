using System;
using Newtonsoft.Json;

namespace k8s.Models
{
    [JsonConverter(typeof(V1PathJsonConverter))]
    public partial class V1Patch
    {
        public enum PatchType
        {
            JsonPatch,
            MergePatch,
            StrategicMergePatch,
        }

        public PatchType Type { get; private set; }

        public V1Patch(string body, PatchType type)
            : this(body)
        {
            this.Type = type;
        }

        partial void CustomInit()
        {
            if (Content is string)
            {
                return;
            }

            throw new NotSupportedException();
        }
    }
}
