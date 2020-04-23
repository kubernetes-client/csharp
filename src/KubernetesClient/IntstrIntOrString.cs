using System;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;

namespace k8s.Models
{
    public class IntOrStringYamlConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(IntstrIntOrString);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            if (parser.Current is YamlDotNet.Core.Events.Scalar scalar)
            {
                try
                {
                    if (string.IsNullOrEmpty(scalar.Value))
                    {
                        return null;
                    }

                    return new IntstrIntOrString(scalar.Value);
                }
                finally
                {
                    parser.MoveNext();
                }
            }

            throw new InvalidOperationException(parser.Current?.ToString());
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var obj = (IntstrIntOrString)value;
            emitter.Emit(new YamlDotNet.Core.Events.Scalar(obj.Value));
        }
    }

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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((IntstrIntOrString)obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}
