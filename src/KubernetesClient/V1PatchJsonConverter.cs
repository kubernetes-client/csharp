namespace k8s.Models
{
    internal class V1PatchJsonConverter : JsonConverter<V1Patch>
    {
        public override V1Patch Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, V1Patch value, JsonSerializerOptions options)
        {
            var content = value?.Content;
            if (content is string s)
            {
                writer.WriteRawValue(s);
                return;
            }

            JsonSerializer.Serialize(writer, content, options);
        }
    }
}
