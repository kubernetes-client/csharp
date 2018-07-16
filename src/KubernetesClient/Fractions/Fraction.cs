using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;
using Fractions.TypeConverters;

namespace Fractions {
    /// <summary>
    /// A mathematical fraction. A rational number written as a/b (a is the numerator and b the denominator). 
    /// The data type is not capable to store NaN (not a number) or infinite.
    /// </summary>
    [TypeConverter(typeof (FractionTypeConverter))]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Fraction : IEquatable<Fraction>, IComparable, IComparable<Fraction>, IFormattable {
        private static readonly BigInteger MIN_DECIMAL = new BigInteger(decimal.MinValue);
        private static readonly BigInteger MAX_DECIMAL = new BigInteger(decimal.MaxValue);
        private static readonly Fraction _zero = new Fraction(BigInteger.Zero, BigInteger.Zero, FractionState.IsNormalized);
        private static readonly Fraction _one = new Fraction(BigInteger.One, BigInteger.One, FractionState.IsNormalized);
        private static readonly Fraction _minus_one = new Fraction(BigInteger.MinusOne, BigInteger.One, FractionState.IsNormalized);

        private readonly BigInteger _denominator;
        private readonly BigInteger _numerator;
        private readonly FractionState _state;
        
        /// <summary>
        /// The numerator.
        /// </summary>
        
        public BigInteger Numerator => _numerator;

        /// <summary>
        /// The denominator
        /// </summary>
        
        public BigInteger Denominator => _denominator;

        /// <summary>
        /// <c>true</c> if the value is positive (greater than or equal to 0).
        /// </summary>
        
        public bool IsPositive => _numerator.Sign == 1 && _denominator.Sign == 1 ||
                                  _numerator.Sign == -1 && _denominator.Sign == -1;

        /// <summary>
        /// <c>true</c> if the value is negative (lesser than 0).
        /// </summary>
        
        public bool IsNegative => _numerator.Sign == -1 && _denominator.Sign == 1 ||
                                  _numerator.Sign == 1 && _denominator.Sign == -1;

        /// <summary>
        /// <c>true</c> if the fraction has a real (calculated) value of 0.
        /// </summary>
        
        public bool IsZero => _numerator.IsZero || _denominator.IsZero;

        /// <summary>
        /// The fraction's state.
        /// </summary>
        
        public FractionState State => _state;

        /// <summary>
        /// A fraction with the reduced/simplified value of 0.
        /// </summary>
        
        public static Fraction Zero => _zero;

        /// <summary>
        /// A fraction with the reduced/simplified value of 1.
        /// </summary>
        
        public static Fraction One => _one;

        /// <summary>
        /// A fraction with the reduced/simplified value of -1.
        /// </summary>
        
        public static Fraction MinusOne => _minus_one;
    }
}