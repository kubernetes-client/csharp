using System;
using System.Globalization;
using System.Numerics;
using Fractions.Extensions;

namespace Fractions {
    public partial struct Fraction {
        /// <summary>
        /// Converts a string to a fraction. Example: "3/4" or "4.5" (the decimal separator character is depending on the system culture).
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="fractionString">A fraction or a (decimal) number. The numerator and denominator must be separated with a '/' (slash) character.</param>
        /// <returns>A normalized <see cref="Fraction"/></returns>
        public static Fraction FromString(string fractionString) {
            return FromString(fractionString, NumberStyles.Number, null);
        }

        /// <summary>
        /// Converts a string to a fraction. Example: "3/4" or "4.5" (the decimal separator character depends on <paramref name="formatProvider"/>).
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="fractionString">A fraction or a (decimal) number. The numerator and denominator must be separated with a '/' (slash) character.</param>
        /// <param name="formatProvider">Provides culture specific information that will be used to parse the <paramref name="fractionString"/>.</param>
        /// <returns>A normalized <see cref="Fraction"/></returns>
        public static Fraction FromString(string fractionString, IFormatProvider formatProvider) {
            return FromString(fractionString, NumberStyles.Number, formatProvider);
        }

        /// <summary>
        /// Converts a string to a fraction. Example: "3/4" or "4.5" (the decimal separator character depends on <paramref name="formatProvider"/>).
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="fractionString">A fraction or a (decimal) number. The numerator and denominator must be separated with a '/' (slash) character.</param>
        /// <param name="numberStyles">A bitwise combination of number styles that are allowed in <paramref name="fractionString"/>.</param>
        /// <param name="formatProvider">Provides culture specific information that will be used to parse the <paramref name="fractionString"/>.</param>
        /// <returns>A normalized <see cref="Fraction"/></returns>
        public static Fraction FromString(string fractionString, NumberStyles numberStyles, IFormatProvider formatProvider) {
            if (fractionString == null) {
                throw new ArgumentNullException(nameof(fractionString));
            }

            if (!TryParse(fractionString, numberStyles, formatProvider, true, out Fraction fraction)) {
                throw new FormatException(string.Format("The string '{0}' cannot be converted to fraction.", fractionString));
            }

            return fraction;
        }

        /// <summary>
        /// Try to convert a string to a fraction. Example: "3/4" or "4.5" (the decimal separator character depends on the system's culture).
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="fractionString">A fraction or a (decimal) number. The numerator and denominator must be separated with a '/' (slash) character.</param>
        /// <param name="fraction">A normalized <see cref="Fraction"/> if the method returns with <c>true</c>. Otherwise the value is invalid.</param>
        /// <returns>
        /// <para><c>true</c> if <paramref name="fractionString"/> was well formed. The parsing result will be written to <paramref name="fraction"/>. </para>
        /// <para><c>false</c> if <paramref name="fractionString"/> was invalid.</para></returns>
        public static bool TryParse(string fractionString, out Fraction fraction) {
            return TryParse(fractionString, NumberStyles.Number, null, true, out fraction);
        }

        /// <summary>
        /// Try to convert a string to a fraction. Example: "3/4" or "4.5" (the decimal separator character depends on <paramref name="formatProvider"/>).
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="fractionString">A fraction or a (decimal) number. The numerator and denominator must be separated with a '/' (slash) character.</param>
        /// <param name="numberStyles">A bitwise combination of number styles that are allowed in <paramref name="fractionString"/>.</param>
        /// <param name="formatProvider">Provides culture specific information that will be used to parse the <paramref name="fractionString"/>.</param>
        /// <param name="fraction">A normalized <see cref="Fraction"/> if the method returns with <c>true</c>. Otherwise the value is invalid.</param>
        /// <returns>
        /// <para><c>true</c> if <paramref name="fractionString"/> was well formed. The parsing result will be written to <paramref name="fraction"/>. </para>
        /// <para><c>false</c> if <paramref name="fractionString"/> was invalid.</para>
        /// </returns>
        public static bool TryParse(string fractionString, NumberStyles numberStyles, IFormatProvider formatProvider, out Fraction fraction) {
            return TryParse(fractionString, numberStyles, formatProvider, true, out fraction);
        }

        /// <summary>
        /// Try to convert a string to a fraction. Example: "3/4" or "4.5" (the decimal separator character depends on <paramref name="formatProvider"/>).
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="fractionString">A fraction or a (decimal) number. The numerator and denominator must be separated with a '/' (slash) character.</param>
        /// <param name="numberStyles">A bitwise combination of number styles that are allowed in <paramref name="fractionString"/>.</param>
        /// <param name="formatProvider">Provides culture specific information that will be used to parse the <paramref name="fractionString"/>.</param>
        /// <param name="normalize">If <c>true</c> the parsed fraction will be reduced.</param>
        /// <param name="fraction">A <see cref="Fraction"/> if the method returns with <c>true</c>. Otherwise the value is invalid.</param>
        /// <returns>
        /// <para><c>true</c> if <paramref name="fractionString"/> was well formed. The parsing result will be written to <paramref name="fraction"/>. </para>
        /// <para><c>false</c> if <paramref name="fractionString"/> was invalid.</para>
        /// </returns>
        public static bool TryParse(string fractionString, NumberStyles numberStyles, IFormatProvider formatProvider, bool normalize, out Fraction fraction) {
            if (fractionString == null) {
                return CannotParse(out fraction);
            }

            var components = fractionString.Split('/');
            if (components.Length == 1) {
                return TryParseSingleNumber(components[0], numberStyles, formatProvider, out fraction);
            }

            if (components.Length >= 2) {
                var numeratorString = components[0];
                var denominatorString = components[1];

                var withoutDecimalpoint = numberStyles & ~NumberStyles.AllowDecimalPoint;
                if (!BigInteger.TryParse(
                        value: numeratorString,
                        style: withoutDecimalpoint,
                        provider: formatProvider,
                        result: out BigInteger numerator)
                    || !BigInteger.TryParse(
                        value: denominatorString,
                        style: withoutDecimalpoint,
                        provider: formatProvider,
                        result: out BigInteger denominator)) {
                    return CannotParse(out fraction);
                }
                fraction = new Fraction(numerator, denominator, normalize);
                return true;
            }

            // Technically it should not be possible to reach this line of code..
            return CannotParse(out fraction);
        }

        /// <summary>
        /// Try to convert a single number to a fraction. Example 34 or 4.5 (depending on the supplied culture used in <paramref name="formatProvider"/>)
        /// If the number contains a decimal separator it will be parsed as <see cref="decimal"/>.
        /// </summary>
        /// <param name="number">A (decimal) number</param>
        /// <param name="numberStyles">A bitwise combination of number styles that are allowed in <paramref name="number"/>.</param>
        /// <param name="formatProvider">Provides culture specific information that will be used to parse the <paramref name="number"/>.</param>
        /// <param name="fraction">A <see cref="Fraction"/> if the method returns with <c>true</c>. Otherwise the value is invalid.</param>
        /// <returns>
        /// <para><c>true</c> if <paramref name="number"/> was well formed. The parsing result will be written to <paramref name="fraction"/>. </para>
        /// <para><c>false</c> if <paramref name="number"/> was invalid.</para>
        /// </returns>
        private static bool TryParseSingleNumber(string number, NumberStyles numberStyles, IFormatProvider formatProvider, out Fraction fraction) {
            var numberFormatInfo = NumberFormatInfo.GetInstance(formatProvider);

            if (number.Contains(numberFormatInfo.NumberDecimalSeparator)) {
                if (!decimal.TryParse(number, numberStyles, formatProvider, out decimal decimalNumber)) {
                    return CannotParse(out fraction);
                }
                fraction = FromDecimal(decimalNumber);
                return true;
            }

            var withoutDecimalpoint = numberStyles & ~NumberStyles.AllowDecimalPoint;
            if (!BigInteger.TryParse(number, withoutDecimalpoint, formatProvider, out BigInteger numerator)) {
                return CannotParse(out fraction);
            }
            fraction = new Fraction(numerator);
            return true;
        }

        /// <summary>
        /// Returns false. <paramref name="fraction"/> contains an invalid value.
        /// </summary>
        /// <param name="fraction">Returns <c>default()</c> of <see cref="Fraction"/></param>
        /// <returns><c>false</c></returns>
        private static bool CannotParse(out Fraction fraction) {
            fraction = default(Fraction);
            return false;
        }

        /// <summary>
        /// Converts a floating point value to a fraction. The value will not be rounded therefore you will probably 
        /// get huge numbers as numerator und denominator. <see cref="double"/> values are not able to store simple rational
        /// numbers like 0.2 or 0.3 - so please don't be worried if the fraction looks weird. For more information visit 
        /// http://en.wikipedia.org/wiki/Floating_point
        /// </summary>
        /// <param name="value">A floating point value.</param>
        /// <returns>A fraction</returns>
        /// <exception cref="InvalidNumberException">If <paramref name="value"/> is NaN (not a number) or infinite.</exception>
        public static Fraction FromDouble(double value) {
            if (double.IsNaN(value) || double.IsInfinity(value)) {
                throw new InvalidNumberException();
            }

            // No rounding here! It will convert the actual number that is stored as double! 
            // See http://www.mpdvc.de/artikel/FloatingPoint.htm

            const ulong SIGN_BIT = 0x8000000000000000;
            const ulong EXPONENT_BITS = 0x7FF0000000000000;
            const ulong MANTISSA = 0x000FFFFFFFFFFFFF;
            const ulong MANTISSA_DIVISOR = 0x0010000000000000;
            const ulong K = 1023;
            var one = BigInteger.One;

            // value = (-1 * sign)   *   (1 + 2^(-1) + 2^(-2) .. + 2^(-52))   *   2^(exponent-K)
            var valueBits = unchecked((ulong) BitConverter.DoubleToInt64Bits(value));

            if (valueBits == 0) {
                // See IEEE 754
                return Zero;
            }

            var isNegative = (valueBits & SIGN_BIT) == SIGN_BIT;
            var mantissaBits = valueBits & MANTISSA;

            // (exponent-K)
            var exponent = (int) (((valueBits & EXPONENT_BITS) >> 52) - K);

            // (1 + 2^(-1) + 2^(-2) .. + 2^(-52))
            var mantissa = new Fraction(mantissaBits + MANTISSA_DIVISOR, MANTISSA_DIVISOR);

            // 2^exponent
            var factor = exponent < 0
                ? new Fraction(one, one << Math.Abs(exponent))
                : new Fraction(one << exponent);

            var result = mantissa * factor;

            return isNegative
                ? result.Invert()
                : result;
        }

        /// <summary>
        /// Converts a floating point value to a fraction. The value will be rounded if possible.
        /// </summary>
        /// <param name="value">A floating point value.</param>
        /// <returns>A fraction</returns>
        /// <exception cref="InvalidNumberException">If <paramref name="value"/> is NaN (not a number) or infinite.</exception>
        public static Fraction FromDoubleRounded(double value) {
            if (double.IsNaN(value) || double.IsInfinity(value)) {
                throw new InvalidNumberException();
            }

            // Null?
            if (Math.Abs(value - 0.0) < double.Epsilon) {
                return Zero;
            }

            // Inspired from Syed Mehroz Alam <smehrozalam@yahoo.com> http://www.geocities.ws/smehrozalam/source/fractioncs.txt
            // .. who got it from http://ebookbrowse.com/confrac-pdf-d13212190 
            // or ftp://89.25.159.69/knm/ksiazki/reszta/homepage.smc.edu.kennedy_john/CONFRAC.pdf
            var sign = Math.Sign(value);
            var absoluteValue = Math.Abs(value);

            var numerator = new BigInteger(absoluteValue);
            var denominator = 1.0;
            var remainingDigits = absoluteValue;
            var previousDenominator = 0.0;
            var breakCounter = 0;

            while (MathExt.RemainingDigitsAfterTheDecimalPoint(remainingDigits)
                   && Math.Abs(absoluteValue - (double) numerator / denominator) > double.Epsilon) {

                remainingDigits = 1.0 / (remainingDigits - Math.Floor(remainingDigits));

                var tmp = denominator;

                denominator = Math.Floor(remainingDigits) * denominator + previousDenominator;
                numerator = new BigInteger(absoluteValue * denominator + 0.5);

                previousDenominator = tmp;

                // See http://www.ozgrid.com/forum/archive/index.php/t-22530.html
                if (++breakCounter > 594) {
                    break;
                }
            }

            return new Fraction(
                sign < 0 ? BigInteger.Negate(numerator) : numerator,
                new BigInteger(denominator),
                true);
        }

        /// <summary>
        /// Converts a decimal value in a fraction. The value will not be rounded.
        /// </summary>
        /// <param name="value">A decimal value.</param>
        /// <returns>A fraction.</returns>
        public static Fraction FromDecimal(decimal value) {
            if (value == decimal.Zero) {
                return _zero;
            }

            if (value == decimal.One) {
                return _one;
            }

            if (value == decimal.MinusOne) {
                return _minus_one;
            }

            var bits = decimal.GetBits(value);
            var low = BitConverter.GetBytes(bits[0]);
            var middle = BitConverter.GetBytes(bits[1]);
            var high = BitConverter.GetBytes(bits[2]);
            var scale = BitConverter.GetBytes(bits[3]);


            var exp = scale[2];
            bool positiveSign = (scale[3] & 0x80) == 0;

            // value = 0x00,high,middle,low / 10^exp
            var numerator = new BigInteger(new byte[] {
                low[0], low[1], low[2], low[3],
                middle[0], middle[1], middle[2], middle[3],
                high[0], high[1], high[2], high[3],
                0x00
            });
            var denominator = BigInteger.Pow(10, exp);

            return positiveSign
                ? new Fraction(numerator, denominator, true)
                : new Fraction(BigInteger.Negate(numerator), denominator, true);
        }
    }
}
