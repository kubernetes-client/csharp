namespace k8s.Models
{
    [JsonConverter(typeof(IntOrStringJsonConverter))]
    public partial class IntOrString
    {
        public static implicit operator IntOrString(int v)
        {
            return new IntOrString { Value = Convert.ToString(v) };
        }

        public static implicit operator IntOrString(long v)
        {
            return new IntOrString { Value = Convert.ToString(v) };
        }

        public static implicit operator string(IntOrString v)
        {
            return v?.Value;
        }

        public static implicit operator IntOrString(string v)
        {
            return new IntOrString { Value = v };
        }

        protected bool Equals(IntOrString other)
        {
            return string.Equals(Value, other?.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((IntOrString)obj);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }
}
