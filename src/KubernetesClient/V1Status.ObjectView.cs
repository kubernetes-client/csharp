namespace k8s.Models
{
    public partial class V1Status
    {
        internal class V1StatusObjectViewConverter : JsonConverter<V1Status>
        {
            public override V1Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var obj = JsonElement.ParseValue(ref reader);

                try
                {
                    return obj.Deserialize<V1Status>();
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
            return _original.Deserialize<T>();
        }
    }
}
