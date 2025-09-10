namespace k8s.Models
{
    [JsonConverter(typeof(IntOrStringJsonConverter))]
    public struct IntOrString
    {
        [JsonPropertyName("value")]
        public string Value { get; private init; }

        public static implicit operator IntOrString(int v)
        {
            return Convert.ToString(v);
        }

        public static implicit operator IntOrString(long v)
        {
            return Convert.ToString(v);
        }

        public static implicit operator string(IntOrString v)
        {
            return v.Value;
        }

        public static implicit operator IntOrString(string v)
        {
            return new IntOrString { Value = v };
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
