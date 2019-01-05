using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Fractions;
using Newtonsoft.Json;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace k8s.Models
{
    internal class QuantityConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var q = (ResourceQuantity)value;

            if (q != null)
            {
                serializer.Serialize(writer, q.ToString());
                return;
            }

            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return new ResourceQuantity(serializer.Deserialize<string>(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

    /// <summary>
    ///     port https://github.com/kubernetes/apimachinery/blob/master/pkg/api/resource/quantity.go to c#
    ///     Quantity is a fixed-point representation of a number.
    ///     It provides convenient marshaling/unmarshaling in JSON and YAML,
    ///     in addition to String() and Int64() accessors.
    ///     The serialization format is:
    ///     quantity        ::= signedNumber suffix
    ///     (Note that suffix may be empty, from the "" case in decimalSI.)
    ///     digit           ::= 0 | 1 | ... | 9
    ///     digits          ::= digit | digitdigits
    ///     number          ::= digits | digits.digits | digits. | .digits
    ///     sign            ::= "+" | "-"
    ///     signedNumber    ::= number | signnumber
    ///     suffix          ::= binarySI | decimalExponent | decimalSI
    ///     binarySI        ::= Ki | Mi | Gi | Ti | Pi | Ei
    ///     (International System of units; See: http:///physics.nist.gov/cuu/Units/binary.html)
    ///     decimalSI       ::= m | "" | k | M | G | T | P | E
    ///     (Note that 1024 = 1Ki but 1000 = 1k; I didn't choose the capitalization.)
    ///     decimalExponent ::= "e" signedNumber | "E" signedNumber
    ///     No matter which of the three exponent forms is used, no quantity may represent
    ///     a number greater than 2^63-1 in magnitude, nor may it have more than 3 decimal
    ///     places. Numbers larger or more precise will be capped or rounded up.
    ///     (E.g.: 0.1m will rounded up to 1m.)
    ///     This may be extended in the future if we require larger or smaller quantities.
    ///     When a Quantity is parsed from a string, it will remember the type of suffix
    ///     it had, and will use the same type again when it is serialized.
    ///     Before serializing, Quantity will be put in "canonical form".
    ///     This means that Exponent/suffix will be adjusted up or down (with a
    ///     corresponding increase or decrease in Mantissa) such that:
    ///     a. No precision is lost
    ///     b. No fractional digits will be emitted
    ///     c. The exponent (or suffix) is as large as possible.
    ///     The sign will be omitted unless the number is negative.
    ///     Examples:
    ///     1.5 will be serialized as "1500m"
    ///     1.5Gi will be serialized as "1536Mi"
    ///     NOTE: We reserve the right to amend this canonical format, perhaps to
    ///     allow 1.5 to be canonical.
    ///     TODO: Remove above disclaimer after all bikeshedding about format is over,
    ///     or after March 2015.
    ///     Note that the quantity will NEVER be internally represented by a
    ///     floating point number. That is the whole point of this exercise.
    ///     Non-canonical values will still parse as long as they are well formed,
    ///     but will be re-emitted in their canonical form. (So always use canonical
    ///     form, or don't diff.)
    ///     This format is intended to make it difficult to use these numbers without
    ///     writing some sort of special handling code in the hopes that that will
    ///     cause implementors to also use a fixed point implementation.
    /// </summary>
    [JsonConverter(typeof(QuantityConverter))]
    public partial class ResourceQuantity : IYamlConvertible
    {
        public enum SuffixFormat
        {
            DecimalExponent,
            BinarySI,
            DecimalSI
        }

        public static readonly decimal MaxAllowed = (decimal)BigInteger.Pow(2, 63) - 1;

        private static readonly char[] SuffixChars = "eEinumkKMGTP".ToCharArray();
        private Fraction _unitlessValue;

        public ResourceQuantity(decimal n, int exp, SuffixFormat format)
        {
            _unitlessValue = Fraction.FromDecimal(n) * Fraction.Pow(10, exp);
            Format = format;
        }

        public SuffixFormat Format { get; private set; }

        public string CanonicalizeString()
        {
            return CanonicalizeString(Format);
        }

        public override string ToString()
        {
            return CanonicalizeString();
        }

        protected bool Equals(ResourceQuantity other)
        {
            return Format == other.Format && _unitlessValue.Equals(other._unitlessValue);
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
            return Equals((ResourceQuantity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Format * 397) ^ _unitlessValue.GetHashCode();
            }
        }

        //
        // CanonicalizeString = go version CanonicalizeBytes
        // CanonicalizeBytes returns the canonical form of q and its suffix (see comment on Quantity).
        //
        // Note about BinarySI:
        // * If q.Format is set to BinarySI and q.Amount represents a non-zero value between
        //   -1 and +1, it will be emitted as if q.Format were DecimalSI.
        // * Otherwise, if q.Format is set to BinarySI, fractional parts of q.Amount will be
        //   rounded up. (1.1i becomes 2i.)
        public string CanonicalizeString(SuffixFormat suffixFormat)
        {
            if (suffixFormat == SuffixFormat.BinarySI)
            {
                if (-1024 < _unitlessValue && _unitlessValue < 1024)
                {
                    return Suffixer.AppendMaxSuffix(_unitlessValue, SuffixFormat.DecimalSI);
                }

                if (HasMantissa(_unitlessValue))
                {
                    return Suffixer.AppendMaxSuffix(_unitlessValue, SuffixFormat.DecimalSI);
                }
            }

            return Suffixer.AppendMaxSuffix(_unitlessValue, suffixFormat);
        }

        // ctor
        partial void CustomInit()
        {
            if (Value == null)
            {
                // No value has been defined, initialize to 0.
                _unitlessValue = new Fraction(0);
                Format = SuffixFormat.BinarySI;
                return;
            }

            var value = Value.Trim();

            var si = value.IndexOfAny(SuffixChars);
            if (si == -1)
            {
                si = value.Length;
            }

            var literal = Fraction.FromString(value.Substring(0, si));
            var suffixer = new Suffixer(value.Substring(si));

            _unitlessValue = literal.Multiply(Fraction.Pow(suffixer.Base, suffixer.Exponent));
            Format = suffixer.Format;

            if (Format == SuffixFormat.BinarySI && _unitlessValue > Fraction.FromDecimal(MaxAllowed))
            {
                _unitlessValue = Fraction.FromDecimal(MaxAllowed);
            }
        }

        private static bool HasMantissa(Fraction value)
        {
            if (value.IsZero)
            {
                return false;
            }

            return BigInteger.Remainder(value.Numerator, value.Denominator) > 0;
        }

        /// <inheritdoc/>
        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            if (expectedType != typeof(ResourceQuantity))
            {
                throw new ArgumentOutOfRangeException(nameof(expectedType));
            }

            if (parser.Current is Scalar)
            {
                Value = ((Scalar)parser.Current).Value;
                parser.MoveNext();
                CustomInit();
            }
        }

        /// <inheritdoc/>
        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new Scalar(this.ToString()));
        }

        public static implicit operator decimal(ResourceQuantity v)
        {
            return v._unitlessValue.ToDecimal();
        }

        public static implicit operator ResourceQuantity(decimal v)
        {
            return new ResourceQuantity(v, 0, SuffixFormat.DecimalExponent);
        }

        #region suffixer

        private class Suffixer
        {
            private static readonly IReadOnlyDictionary<string, (int, int)> BinSuffixes =
                new Dictionary<string, (int, int)>
                {
                    // Don't emit an error when trying to produce
                    // a suffix for 2^0.
                    {"", (2, 0)},
                    {"Ki", (2, 10)},
                    {"Mi", (2, 20)},
                    {"Gi", (2, 30)},
                    {"Ti", (2, 40)},
                    {"Pi", (2, 50)},
                    {"Ei", (2, 60)}
                };

            private static readonly IReadOnlyDictionary<string, (int, int)> DecSuffixes =
                new Dictionary<string, (int, int)>
                {
                    {"n", (10, -9)},
                    {"u", (10, -6)},
                    {"m", (10, -3)},
                    {"", (10, 0)},
                    {"k", (10, 3)},
                    {"M", (10, 6)},
                    {"G", (10, 9)},
                    {"T", (10, 12)},
                    {"P", (10, 15)},
                    {"E", (10, 18)}
                };

            public Suffixer(string suffix)
            {
                // looked up
                {
                    if (DecSuffixes.TryGetValue(suffix, out var be))
                    {
                        (Base, Exponent) = be;
                        Format = SuffixFormat.DecimalSI;

                        return;
                    }
                }

                {
                    if (BinSuffixes.TryGetValue(suffix, out var be))
                    {
                        (Base, Exponent) = be;
                        Format = SuffixFormat.BinarySI;

                        return;
                    }
                }

                if (char.ToLower(suffix[0]) == 'e')
                {
                    Base = 10;
                    Exponent = int.Parse(suffix.Substring(1));
                    Format = SuffixFormat.DecimalExponent;
                    return;
                }

                throw new ArgumentException("unable to parse quantity's suffix");
            }

            public SuffixFormat Format { get; }

            public int Base { get; }
            public int Exponent { get; }


            public static string AppendMaxSuffix(Fraction value, SuffixFormat format)
            {
                if (value.IsZero)
                {
                    return "0";
                }

                switch (format)
                {
                    case SuffixFormat.DecimalExponent:
                        {
                            var minE = -9;
                            var lastv = Roundup(value * Fraction.Pow(10, -minE));

                            for (var exp = minE;; exp += 3)
                            {
                                var v = value * Fraction.Pow(10, -exp);
                                if (HasMantissa(v))
                                {
                                    break;
                                }

                                minE = exp;
                                lastv = v;
                            }


                            if (minE == 0)
                            {
                                return $"{(decimal) lastv}";
                            }

                            return $"{(decimal) lastv}e{minE}";
                        }

                    case SuffixFormat.BinarySI:
                        return AppendMaxSuffix(value, BinSuffixes);
                    case SuffixFormat.DecimalSI:
                        return AppendMaxSuffix(value, DecSuffixes);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(format), format, null);
                }
            }

            private static string AppendMaxSuffix(Fraction value, IReadOnlyDictionary<string, (int, int)> suffixes)
            {
                var min = suffixes.First();
                var suffix = min.Key;
                var lastv = Roundup(value * Fraction.Pow(min.Value.Item1, -min.Value.Item2));

                foreach (var kv in suffixes.Skip(1))
                {
                    var v = value * Fraction.Pow(kv.Value.Item1, -kv.Value.Item2);
                    if (HasMantissa(v))
                    {
                        break;
                    }

                    suffix = kv.Key;
                    lastv = v;
                }

                return $"{(decimal) lastv}{suffix}";
            }

            private static Fraction Roundup(Fraction lastv)
            {
                var round = BigInteger.DivRem(lastv.Numerator, lastv.Denominator, out var remainder);
                if (!remainder.IsZero)
                {
                    lastv = round + 1;
                }
                return lastv;
            }
        }

        #endregion
    }
}
