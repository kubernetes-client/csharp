using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace k8s.Tests.Utils
{
    [DebuggerStepThrough]
    public class TestResource
    {
        private sealed class KeyVersionEqualityComparer : IEqualityComparer<TestResource>
        {
            public bool Equals(TestResource x, TestResource y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Key == y.Key && x.Version == y.Version;
            }

            public int GetHashCode(TestResource obj)
            {
                unchecked
                {
                    return (obj.Key * 397) ^ obj.Version;
                }
            }
        }

        public static IEqualityComparer<TestResource> KeyVersionComparer { get; } = new KeyVersionEqualityComparer();

        protected bool Equals(TestResource other)
        {
            return Value == other.Value && Key == other.Key && Version == other.Version;
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

            return Equals((TestResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Key;
                hashCode = (hashCode * 397) ^ Version;
                return hashCode;
            }
        }


        public TestResource(int key, int version = 1, string value = "test")
        {
            Value = value;
            Version = version;
            Key = key;
        }

        public string Value { get; }
        public int Key { get; }
        public int Version { get; }

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(Value)}: {Value}, {nameof(Version)} {Version}";
        }
    }
}
