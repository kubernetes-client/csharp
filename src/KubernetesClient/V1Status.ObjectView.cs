using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace k8s.Models
{
    public partial class V1Status
    {
        internal class V1StatusObjectViewConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var obj = JToken.Load(reader);

                try
                {
                    return obj.ToObject(objectType);
                }
                catch (JsonException)
                {
                    // should be an object
                }

                return new V1Status
                {
                    _original = obj,
                    HasObject = true
                };
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(V1Status) == objectType;
            }
        }

        private JToken _original;

        public bool HasObject { get; private set; }

        public T ObjectView<T>()
        {
            return _original.ToObject<T>();
        }
    }
}
