namespace k8s.Models
{
    public partial record V1Status
    {
        public sealed class V1StatusObjectViewConverter : JsonConverter<V1Status>
        {
            public override V1Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var ele = doc.RootElement.Clone();

                try
                {
#if NET8_0_OR_GREATER
                    return JsonSerializer.Deserialize(ele, StatusSourceGenerationContext.Default.V1Status);
#else
                    return ele.Deserialize<V1Status>();
#endif
                }
                catch (JsonException)
                {
                    // should be an object
                }

                return new V1Status { _original = ele, HasObject = true };
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
            return KubernetesJson.Deserialize<T>(_original);
#else
            return _original.Deserialize<T>();
#endif
        }
    }
}
