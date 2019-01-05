using System.Numerics;

namespace Fractions {
    public partial struct Fraction {
#pragma warning disable 1591
        public static bool operator ==(Fraction left, Fraction right) {
            return left.Equals(right);
        }

        public static bool operator !=(Fraction left, Fraction right) {
            return !left.Equals(right);
        }

        public static Fraction operator +(Fraction a, Fraction b) {
            return a.Add(b);
        }

        public static Fraction operator -(Fraction a, Fraction b) {
            return a.Subtract(b);
        }

        public static Fraction operator *(Fraction a, Fraction b) {
            return a.Multiply(b);
        }

        public static Fraction operator /(Fraction a, Fraction b) {
            return a.Divide(b);
        }

        public static Fraction operator %(Fraction a, Fraction b) {
            return a.Remainder(b);
        }

        public static bool operator <(Fraction a, Fraction b) {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Fraction a, Fraction b) {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(Fraction a, Fraction b) {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Fraction a, Fraction b) {
            return a.CompareTo(b) >= 0;
        }

        public static implicit operator Fraction(int value) {
            return new Fraction(value);
        }

        public static implicit operator Fraction(long value) {
            return new Fraction(value);
        }

        public static implicit operator Fraction(uint value) {
            return new Fraction(value);
        }

        public static implicit operator Fraction(ulong value) {
            return new Fraction(value);
        }

        public static implicit operator Fraction(BigInteger value) {
            return new Fraction(value);
        }

        public static explicit operator Fraction(double value) {
            return new Fraction(value);
        }

        public static explicit operator Fraction(decimal value) {
            return new Fraction(value);
        }

        public static explicit operator Fraction(string value) {
            return FromString(value);
        }

        public static explicit operator int(Fraction fraction) {
            return fraction.ToInt32();
        }

        public static explicit operator long(Fraction fraction) {
            return fraction.ToInt64();
        }

        public static explicit operator uint(Fraction fraction) {
            return fraction.ToUInt32();
        }

        public static explicit operator ulong(Fraction fraction) {
            return fraction.ToUInt64();
        }

        public static explicit operator decimal(Fraction fraction) {
            return fraction.ToDecimal();
        }

        public static explicit operator double(Fraction fraction) {
            return fraction.ToDouble();
        }

        public static explicit operator BigInteger(Fraction fraction) {
            return fraction.ToBigInteger();
        }
#pragma warning restore 1591
    }
}
