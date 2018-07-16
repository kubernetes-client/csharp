using System;

namespace Fractions.Formatter {
    /// <summary>
    /// Default <see cref="Fraction.ToString()"/> formatter.
    /// </summary>
    public class DefaultFractionFormatProvider : IFormatProvider {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static readonly IFormatProvider Instance = new DefaultFractionFormatProvider();

        object IFormatProvider.GetFormat(Type formatType) {
            return formatType == typeof (Fraction) 
                ? DefaultFractionFormatter.Instance
                : null;
        }
    }
}