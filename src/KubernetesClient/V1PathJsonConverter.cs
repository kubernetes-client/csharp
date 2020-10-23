using System;
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
}
