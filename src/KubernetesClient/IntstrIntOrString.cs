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
            return (IntstrIntOrString) serializer.Deserialize<string>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(string);
        }
    }

    [JsonConverter(typeof(IntOrStringConverter))]
    public partial class IntstrIntOrString
    {
        public static implicit operator int(IntstrIntOrString v)
        {
            return int.Parse(v.Value);
        }

        public static implicit operator IntstrIntOrString(int v)
        {
            return new IntstrIntOrString(Convert.ToString(v));
        }

        public static implicit operator string(IntstrIntOrString v)
        {
            return v.Value;
        }

        public static implicit operator IntstrIntOrString(string v)
        {
            return new IntstrIntOrString(v);
        }

        protected bool Equals(IntstrIntOrString other)
        {
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntstrIntOrString) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}
