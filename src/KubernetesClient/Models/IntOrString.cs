namespace k8s.Models
{
    [JsonConverter(typeof(IntOrStringJsonConverter))]
    public record IntOrString
    {
        [JsonPropertyName("value")]
        public string Value { get; init; }

        public static implicit operator IntOrString(int v)
        {
            return new IntOrString(Convert.ToString(v));
        }

        public static implicit operator IntOrString(long v)
        {
            return new IntOrString(Convert.ToString(v));
        }

        public static implicit operator string(IntOrString v)
        {
            return v?.Value;
        }

        public static implicit operator IntOrString(string v)
        {
            return new IntOrString(v);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
