using System;

namespace Fractions.Extensions {
    internal static class MathExt {
        /// <summary>
        /// Checks for an even number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns><c>true</c> if the number is even.</returns>
        public static bool IsEven(this long number) {
            return (number & 1) == 0;
        }

        /// <summary>
        /// Checks for an odd number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns><c>true</c> if the number is odd.</returns>
        public static bool IsOdd(this long number) {
            return (number & 1) != 0;
        }

        /// <summary>
        /// Get the greatest common divisor (GCD) of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <returns>The largest positive integer that divides <paramref name="a"/> and <paramref name="b"/> without a remainder.</returns>
        public static long GreatestCommonDivisor(long a, long b) {
            a = Math.Abs(a);
            b = Math.Abs(b);

            if (a == 0) {
                // ggT(0, b) = b
                // Denn alles teilt durch 0.
                return b;
            }

            if (b == 0) {
                // ggT(a, 0) = a
                return a;
            }

            if (a == 1 || b == 1) {
                // trivial
                return 1;
            }

            return a == b
                ? a // Beide Zahlen sind identisch, wir haben bereits den ggT gefunden.
                : BinaryGreatestCommonDivisorAlgorithm(a, b);
        }

        private static long BinaryGreatestCommonDivisorAlgorithm(long a, long b) {

            // Solange 'a' und 'b' beide gerade Zahlen sind, teile die Zahlen durch 2
            // und merke wie oft dies möglich war in 'k'.
            int k;
            for (k = 0; (a | b).IsEven(); ++k) {
                a >>= 1; // a = (a / 2);
                b >>= 1; // b = (b / 2);
            }

            // Teile 'a' solange durch 2 bis die Zahl ungerade ist.
            while (a.IsEven()) {
                a >>= 1; // a = (a / 2);
            }

            // Ab hier ist 'a' definitiv ungerade. Für 'b' muss dies allerdings noch nicht gelten!
            do {
                // Teile 'b' solange durch 2 bis die Zahl ungerade ist.
                while (b.IsEven()) {
                    b >>= 1; // b = (b / 2);
                }

                // 'a' und 'b' sind hier beide ungerade. Falls 'a' >= 'b'
                // muss der Inhalt beider Variablen geswappt werden,
                // damit die notwendige Subtraktion durchgeführt werden
                // kann.
                if (a > b) {
                    var temp = b;
                    b = a;
                    a = temp;
                }

                b = b - a;

            } while (b != 0);

            return a << k; // a * 2^k
        }

        /// <summary>
        /// Get the least common multiple (LCM) of <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The first number.</param>
        /// <param name="b">The second number.</param>
        /// <returns>The smallest positive integer that is divisible by both <paramref name="a"/> and <paramref name="b"/> or 0 if either <paramref name="a"/> or <paramref name="b"/> is 0</returns>
        /// <exception cref="ArgumentException">If <paramref name="a"/> and <paramref name="b"/> are 0</exception>
        public static long LeastCommonMultiple(long a, long b) {
            if (a == 0 && b == 0) {
                throw new ArgumentException("The least common multiple is not defined if both numbers are zero.");
            }

            a = Math.Abs(a);
            b = Math.Abs(b);

            if (a == b) {
                return a;
            }

            // Es gilt LCM(a,b) = (|a*b|) / GCD(a,b)

            var gcd = GreatestCommonDivisor(a, b);
            return a / gcd * b;
        }

        /// <summary>
        /// Returns <c>true</c> if there are remaining digits after the decimal point.
        /// </summary>
        /// <param name="remainingDigits">A <see cref="double"/> value with possible remaining digits</param>
        /// <returns><c>true</c> if <paramref name="remainingDigits"/> has digits after the decimal point</returns>
        
        public static bool RemainingDigitsAfterTheDecimalPoint(double remainingDigits) {
            return Math.Abs(remainingDigits - Math.Floor(remainingDigits)) > double.Epsilon;
        }
    }
}
