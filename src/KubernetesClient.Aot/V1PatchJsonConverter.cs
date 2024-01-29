namespace k8s.Models
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal sealed class V1PatchJsonConverter : JsonConverter<V1Patch>
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        public override V1Patch Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, V1Patch value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var content = value?.Content;
            if (content is string s)
            {
                writer.WriteRawValue(s);
                return;
            }

            throw new NotSupportedException("only string json patch is supported");
        }
    }
}
