using System;
using Newtonsoft.Json;

namespace k8s.Models
{
    internal class IntOrStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (value as IntstrIntOrString)?.Value;

            if (int.TryParse(s, out var intv))
            {
                serializer.Serialize(writer, intv);
                return;
            }

            serializer.Serialize(writer, s);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return (IntstrIntOrString)serializer.Deserialize<string>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(string);
        }
    }
}
