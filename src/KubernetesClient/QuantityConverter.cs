using System;
using Newtonsoft.Json;

namespace k8s.Models
{
    internal class QuantityConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var q = (ResourceQuantity)value;

            if (q != null)
            {
                serializer.Serialize(writer, q.ToString());
                return;
            }

            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return new ResourceQuantity(serializer.Deserialize<string>(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
