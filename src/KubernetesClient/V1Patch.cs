using System;
using Microsoft.AspNetCore.JsonPatch;
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

        public V1Patch(IJsonPatchDocument jsonPatch)
            : this((object)jsonPatch)
        {
        }

        public V1Patch(String body, PatchType type)
            : this(body)
        {
            this.Type = type;
        }

        partial void CustomInit()
        {
            if (Content is IJsonPatchDocument)
            {
                Type = PatchType.JsonPatch;
                return;
            }

            if (Content is String)
            {
                return;
            }

            throw new NotSupportedException();
        }
    }
}
