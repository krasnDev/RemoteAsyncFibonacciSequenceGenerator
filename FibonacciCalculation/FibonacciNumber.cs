using System.Numerics;

namespace FibonacciCalculation
{
    /// <summary>
    /// Uses for next Fibonacci number calculation.
    /// </summary>
    /// <param name="Previous">Previous number in the Fibonacci sequence.</param>
    /// <param name="Current">Current number in the Fibonacci sequence.</param>
    public struct FibonacciNumber
    {
        public FibonacciNumber(BigInteger previous, BigInteger current)
        {
            Previous = previous;
            Current = current;
        }

        public BigInteger Previous { get; set; }
        public BigInteger Current { get; set; }

        public override string ToString()
        {
            return $"Previous:\n{Previous}\nCurrent:\n{Current}\n";
        }
    }


}