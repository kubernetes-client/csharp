using System;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace k8s.Models
{
    [JsonConverter(typeof(V1PathJsonConverter))]
    public partial class V1Patch
    {
        public enum PathType
        {
            JsonPatch,
            MergePatch,
            StrategicMergePatch,
        }

        public PathType Type { get; private set; }

        public V1Patch(IJsonPatchDocument jsonPatch)
            : this((object)jsonPatch)
        {
        }

        partial void CustomInit()
        {
            if (Content is IJsonPatchDocument)
            {
                Type = PathType.JsonPatch;
                return;
            }

            throw new NotSupportedException();
        }
    }
}
