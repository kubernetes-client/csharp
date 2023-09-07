namespace k8s.Models
{
    internal sealed class ResourceQuantityJsonConverter : JsonConverter<ResourceQuantity>
    {
        // https://github.com/kubernetes/apimachinery/blob/4b14f804a0babdcc58e695d72f77ad29f536511e/pkg/api/resource/quantity.go#L683
        public override ResourceQuantity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return new ResourceQuantity(null);
                case JsonTokenType.Number:
                    if (reader.TryGetDouble(out var val))
                    {
                        return new ResourceQuantity(Convert.ToString(val));
                    }

                    return reader.GetDecimal();
                default:
                    return new ResourceQuantity(reader.GetString());
            }
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
