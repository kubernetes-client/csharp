#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
#endif

namespace k8s.Models
{
    public partial record V1Status
    {
        public sealed class V1StatusObjectViewConverter : JsonConverter<V1Status>
        {
            public override V1Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var obj = JsonElement.ParseValue(ref reader);

                try
                {
#if NET8_0_OR_GREATER
                    return obj.Deserialize((JsonTypeInfo<V1Status>)options.GetTypeInfo(typeof(V1Status)));
#else
                    return obj.Deserialize<V1Status>();
#endif
                }
                catch (JsonException)
                {
                    // should be an object
                }

                return new V1Status { _original = obj, HasObject = true };
            }

            public override void Write(Utf8JsonWriter writer, V1Status value, JsonSerializerOptions options)
            {
                throw new NotImplementedException(); // will not send v1status to server
            }
        }

        private JsonElement _original;

        public bool HasObject { get; private set; }

        public T ObjectView<T>()
        {
#if NET8_0_OR_GREATER
            return _original.Deserialize<T>((JsonTypeInfo<T>)KubernetesJson.JsonSerializerOptions.GetTypeInfo(typeof(T)));
#else
            return _original.Deserialize<T>();
#endif
        }
    }
}
