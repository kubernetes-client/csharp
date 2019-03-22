using System;
using System.Numerics;

namespace k8s.Internal.Fractions {
    internal partial struct Fraction
    {
        /// <summary>
        /// Create a fraction with <paramref name="numerator"/>, <paramref name="denominator"/> and the fraction' <paramref name="state"/>.
        /// Warning: if you use unreduced values combined with a state of <see cref="FractionState.IsNormalized"/>
        /// you will get wrong results when working with the fraction value.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <param name="state"></param>
        private Fraction(BigInteger numerator, BigInteger denominator, FractionState state) {
            _numerator = numerator;
            _denominator = denominator;
            _state = state;
        }

        /// <summary>
        /// Creates a normalized (reduced/simplified) fraction using <paramref name="numerator"/> and <paramref name="denominator"/>.
        /// </summary>
        /// <param name="numerator">Numerator</param>
        /// <param name="denominator">Denominator</param>
        public Fraction(BigInteger numerator, BigInteger denominator)
            : this(numerator, denominator, true) { }

        /// <summary>
        /// Creates a normalized (reduced/simplified) or unnormalized fraction using <paramref name="numerator"/> and <paramref name="denominator"/>.
        /// </summary>
        /// <param name="numerator">Numerator</param>
        /// <param name="denominator">Denominator</param>
        /// <param name="normalize">If <c>true</c> the fraction will be created as reduced/simplified fraction.
        /// This is recommended, especially if your applications requires that the results of the equality methods <see cref="object.Equals(object)"/>
        /// and <see cref="IComparable.CompareTo"/> are always the same. (1/2 != 2/4)</param>
        public Fraction(BigInteger numerator, BigInteger denominator, bool normalize) {
            if (normalize) {
                this = GetReducedFraction(numerator, denominator);
                return;
            }

            _state = numerator.IsZero && denominator.IsZero
                ? FractionState.IsNormalized
                : FractionState.Unknown;

            _numerator = numerator;
            _denominator = denominator;
        }

        /// <summary>
        /// Creates a normalized fraction using a signed 32bit integer.
        /// </summary>
        /// <param name="numerator">integer value that will be used for the numerator. The denominator will be 1.</param>
        public Fraction(int numerator) {
            _numerator = new BigInteger(numerator);
            _denominator = numerator != 0 ? BigInteger.One : BigInteger.Zero;
            _state = FractionState.IsNormalized;
        }

        /// <summary>
        /// Creates a normalized fraction using a signed 64bit integer.
        /// </summary>
        /// <param name="numerator">integer value that will be used for the numerator. The denominator will be 1.</param>
        public Fraction(long numerator) {
            _numerator = new BigInteger(numerator);
            _denominator = numerator != 0 ? BigInteger.One : BigInteger.Zero;
            _state = FractionState.IsNormalized;
        }

        /// <summary>
        /// Creates a normalized fraction using a unsigned 32bit integer.
        /// </summary>
        /// <param name="numerator">integer value that will be used for the numerator. The denominator will be 1.</param>
        public Fraction(uint numerator) {
            _numerator = new BigInteger(numerator);
            _denominator = numerator != 0 ? BigInteger.One : BigInteger.Zero;
            _state = FractionState.IsNormalized;
        }


        /// <summary>
        /// Creates a normalized fraction using a unsigned 64bit integer.
        /// </summary>
        /// <param name="numerator">integer value that will be used for the numerator. The denominator will be 1.</param>
        public Fraction(ulong numerator) {
            _numerator = new BigInteger(numerator);
            _denominator = numerator != 0 ? BigInteger.One : BigInteger.Zero;
            _state = FractionState.IsNormalized;
        }

        /// <summary>
        /// Creates a normalized fraction using a big integer.
        /// </summary>
        /// <param name="numerator">big integer value that will be used for the numerator. The denominator will be 1.</param>
        public Fraction(BigInteger numerator) {
            _numerator = numerator;
            _denominator = numerator.IsZero ? BigInteger.Zero : BigInteger.One;
            _state = FractionState.IsNormalized;
        }


        /// <summary>
        /// Creates a normalized fraction using a 64bit floating point value (double).
        /// The value will not be rounded therefore you will probably get huge numbers as numerator und denominator.
        /// <see cref="double"/> values are not able to store simple rational numbers like 0.2 or 0.3 - so please
        /// don't be worried if the fraction looks weird. For more information visit
        /// http://en.wikipedia.org/wiki/Floating_point
        /// </summary>
        /// <param name="value">Floating point value.</param>
        public Fraction(double value) {
            this = FromDouble(value);
        }

        /// <summary>
        /// Creates a normalized fraction using a 128bit decimal value (decimal).
        /// </summary>
        /// <param name="value">Floating point value.</param>
        public Fraction(decimal value) {
            this = FromDecimal(value);
        }
    }
}
