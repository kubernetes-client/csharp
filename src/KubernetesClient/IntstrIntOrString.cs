using System;
using Newtonsoft.Json;

namespace k8s.Models
{
    [JsonConverter(typeof(IntOrStringConverter))]
    public partial class IntstrIntOrString
    {
        public static implicit operator int(IntstrIntOrString v)
        {
            return int.Parse(v.Value);
        }

        public static implicit operator IntstrIntOrString(int v)
        {
            return new IntstrIntOrString(Convert.ToString(v));
        }

        public static implicit operator string(IntstrIntOrString v)
        {
            return v.Value;
        }

        public static implicit operator IntstrIntOrString(string v)
        {
            return new IntstrIntOrString(v);
        }

        protected bool Equals(IntstrIntOrString other)
        {
            return string.Equals(Value, other.Value);
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((IntstrIntOrString)obj);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }
}
