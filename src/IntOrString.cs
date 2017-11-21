using System;
using Newtonsoft.Json;

namespace k8s.Models
{
    internal class IntOrStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (value as IntOrString)?.Value;

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
            return (IntOrString) serializer.Deserialize<string>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(string);
        }
    }

    [JsonConverter(typeof(IntOrStringConverter))]
    public partial class IntOrString
    {
        public static implicit operator int(IntOrString v)
        {
            return int.Parse(v.Value);
        }

        public static implicit operator IntOrString(int v)
        {
            return new IntOrString(Convert.ToString(v));
        }

        public static implicit operator string(IntOrString v)
        {
            return v.Value;
        }

        public static implicit operator IntOrString(string v)
        {
            return new IntOrString(v);
        }
    }
}
