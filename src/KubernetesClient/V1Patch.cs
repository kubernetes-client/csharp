using System;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace k8s.Models
{
    internal class V1PathJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as V1Patch)?.Content);
        }

        // no read patch object supported at the moment
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(V1Patch);
        }
    }

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

        public V1Patch(IJsonPatchDocument jsonPatch) : this((object)jsonPatch)
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
