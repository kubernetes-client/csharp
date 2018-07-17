using System;
using System.Numerics;

namespace Fractions {
    public partial struct Fraction
    {
        /// <summary>
        /// Compares the calculated value with the supplied <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Fraction that shall be compared with.</param>
        /// <returns>
        /// Less than 0 if <paramref name="other"/> is greater.
        /// Zero (0) if both calculated values are equal.
        /// Greater then zero (0) if <paramref name="other"/> less.</returns>
        /// <exception cref="ArgumentException">If <paramref name="other"/> is not of type <see cref="Fraction"/>.</exception>
        public int CompareTo(object other) {
            if (other == null) {
                return 1;
            }

            if (other.GetType() != typeof(Fraction)) {
                throw new ArgumentException(
                    string.Format("The comparing instance must be of type {0}. The supplied argument is of type {1}", GetType(), other.GetType()), nameof(other));
            }

            return CompareTo((Fraction)other);
        }

        /// <summary>
        /// Compares the calculated value with the supplied <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Fraction that shall be compared with.</param>
        /// <returns>
        /// Less than 0 if <paramref name="other"/> is greater.
        /// Zero (0) if both calculated values are equal.
        /// Greater then zero (0) if <paramref name="other"/> less.</returns>
        
        public int CompareTo(Fraction other) {
            if (_denominator == other._denominator) {
                return _numerator.CompareTo(other._numerator);
            }

            if (IsZero != other.IsZero) {
                if (IsZero) {
                    return other.IsPositive ? -1 : 1;
                }
                return IsPositive ? 1 : -1;
            }

            var gcd = BigInteger.GreatestCommonDivisor(_denominator, other._denominator);

            var thisMultiplier = BigInteger.Divide(_denominator, gcd);
            var otherMultiplier = BigInteger.Divide(other._denominator, gcd);

            var a = BigInteger.Multiply(_numerator, otherMultiplier);
            var b = BigInteger.Multiply(other._numerator, thisMultiplier);

            return a.CompareTo(b);
        }
    }
}
