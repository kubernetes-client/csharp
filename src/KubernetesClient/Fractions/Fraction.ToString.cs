using System;
using System.Globalization;
using Fractions.Formatter;

namespace Fractions {
    public partial struct Fraction {
        /// <summary>
        /// Returns the fraction as "numerator/denominator" or just "numerator" if the denominator has a value of 1.
        /// The returning value is culture invariant (<see cref="CultureInfo" />).
        /// </summary>
        /// <returns>"numerator/denominator" or just "numerator"</returns>

        public override string ToString() {
            return ToString("G", DefaultFractionFormatProvider.Instance);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// The returning value is culture invariant (<see cref="CultureInfo" />).
        /// See <see cref="ToString(string,IFormatProvider)"/> for all formatting options.
        /// </summary>
        /// <returns>"numerator/denominator" or just "numerator"</returns>

        public string ToString(string format) {
            return ToString(format, DefaultFractionFormatProvider.Instance);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified format. The numbers are however culture invariant.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the specified format.
        /// </returns>
        /// <param name="format">The format to use.
        /// <list type="table">
        /// <listheader><term>symbol</term><description>description</description></listheader>
        /// <item><term>G</term><description>General format: numerator/denominator</description></item>
        /// <item><term>n</term><description>Numerator</description></item>
        /// <item><term>d</term><description>Denominator</description></item>
        /// <item><term>z</term><description>The fraction as integer</description></item>
        /// <item><term>r</term><description>The positive remainder of all digits after the decimal point using the format: numerator/denominator or <see cref="string.Empty"/> if the fraction is a valid integer without digits after the decimal point.</description></item>
        /// <item><term>m</term><description>The fraction as mixed number e.g. "2 1/3" instead of "7/3"</description></item>
        /// </list>
        /// -or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable"/> implementation. </param>
        /// <param name="formatProvider">The provider to use to format the value. -or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string format, IFormatProvider formatProvider) {
            var formatter = formatProvider?.GetFormat(GetType()) as ICustomFormatter;

            return formatter != null
                ? formatter.Format(format, this, formatProvider)
                : DefaultFractionFormatter.Instance.Format(format, this, formatProvider);
        }
    }
}
