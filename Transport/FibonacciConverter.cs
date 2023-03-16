using FibonacciCalculation;
using System.Numerics;

namespace Transport
{
    internal static class FibonacciConverter
    {
        /// <summary>
        /// Conversion from transport to calculation type.
        /// </summary>
        /// <param name="fibonacciTransport">Fibonacci transport object, received from transport.</param>
        /// <returns>Ready to calculation instance of the Fibonacci number class.</returns>
        public static FibonacciNumber FromTransport(this FibonacciTransport fibonacciTransport)
        {
            var current = new BigInteger(fibonacciTransport.Current);
            if (current.IsZero)
            {
                current = new BigInteger(1);
            }
            var previous = new BigInteger(fibonacciTransport.Previous);

            return new FibonacciNumber(previous, current);
        }

        /// <summary>
        /// Conversion from calculation to transport type.
        /// </summary>
        /// <param name="fibonacciNumber">Fibonacci number object, received from calculation.</param>
        /// <returns>Ready to transport instance of the Fibonacci number class.</returns>
        public static FibonacciTransport ToTransport(this FibonacciNumber fibonacciNumber)
        {
            return new FibonacciTransport(fibonacciNumber.Previous.ToByteArray(),
                                          fibonacciNumber.Current.ToByteArray());
        }
    }
}
