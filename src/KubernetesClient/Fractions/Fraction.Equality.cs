
namespace Fractions {
    public partial struct Fraction
    {
        /// <summary>
        /// Tests if the calculated value of this fraction equals to the calculated value of <paramref name="other"/>.
        /// It does not matter if either of them is not normalized. Both values will be reduced (normalized) before performing
        /// the <see cref="object.Equals(object)"/> test.
        /// </summary>
        /// <param name="other">The fraction to compare with.</param>
        /// <returns><c>true</c> if both values are equivalent. (e.g. 2/4 is equivalent to 1/2. But 2/4 is not equivalent to -1/2)</returns>

        public bool IsEquivalentTo(Fraction other) {
            var a = Reduce();
            var b = other.Reduce();

            return a.Equals(b);
        }

        /// <summary>
        /// <para>Performs an exact comparison with <paramref name="other"/> using numerator and denominator.</para>
        /// <para>Warning: 1/2 is NOT equal to 2/4! -1/2 is NOT equal to 1/-2!</para>
        /// <para>If you want to test the calculated values for equality use <see cref="CompareTo(Fraction)"/> or
        /// <see cref="IsEquivalentTo"/> </para>
        /// </summary>
        /// <param name="other">The fraction to compare with.</param>
        /// <returns><c>true</c> if numerator and denominator of both fractions are equal.</returns>

        public bool Equals(Fraction other) {
            return other._denominator.Equals(_denominator) && other._numerator.Equals(_numerator);
        }

        /// <summary>
        /// <para>Performs an exact comparison with <paramref name="other"/> using numerator and denominator.</para>
        /// <para>Warning: 1/2 is NOT equal to 2/4! -1/2 is NOT equal to 1/-2!</para>
        /// <para>If you want to test the calculated values for equality use <see cref="CompareTo(Fraction)"/> or
        /// <see cref="IsEquivalentTo"/> </para>
        /// </summary>
        /// <param name="other">The fraction to compare with.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is type of <see cref="Fraction"/> and numerator and denominator of both are equal.</returns>

        public override bool Equals(object other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            return other is Fraction && Equals((Fraction)other);
        }

        /// <summary>
        /// Returns the hash code.
        /// </summary>
        /// <returns>
        /// A 32bit integer with sign. It has been constructed using the <see cref="Numerator"/> and the <see cref="Denominator"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>

        public override int GetHashCode() {
            unchecked {
                return (_denominator.GetHashCode() * 397) ^ _numerator.GetHashCode();
            }
        }
    }
}
