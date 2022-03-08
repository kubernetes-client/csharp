namespace k8s.Models
{
    internal sealed class QuantityConverter : JsonConverter<ResourceQuantity>
    {
        public override ResourceQuantity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new ResourceQuantity(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, ResourceQuantity value, JsonSerializerOptions options)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStringValue(value?.ToString());
        }
    }
}
