using System;

namespace k8s.Internal.Fractions {
    /// <summary>
    /// Exception that will be thrown if an argument contains not a number (NaN) or is infinite.
    /// </summary>
    internal class InvalidNumberException : ArithmeticException {
#pragma warning disable 1591
        public InvalidNumberException() {}
        public InvalidNumberException(string message) : base(message) {}
        public InvalidNumberException(string message, Exception innerException) : base(message, innerException) {}
#pragma warning restore 1591
    }
}
