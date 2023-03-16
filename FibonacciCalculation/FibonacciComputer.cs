using System.Numerics;

namespace FibonacciCalculation
{
    /// <summary>
    /// Provides a method to get next number in the Fibonacci sequence
    /// </summary>
    public class FibonacciComputer
    {
        /// <summary>
        /// Calculates the next Fibonacci number as <paramref name="fibonacci"/> previous + current.
        /// </summary>
        /// <param name="fibonacci">Instance of <see cref="FibonacciNumber"/> with previous and current numbers.</param>
        /// <returns>The next <see cref="FibonacciNumber"/>.</returns>
        public FibonacciNumber GetNext(FibonacciNumber fibonacci)
        {
            return new FibonacciNumber(fibonacci.Current, fibonacci.Previous + fibonacci.Current);
        } 
    }
}