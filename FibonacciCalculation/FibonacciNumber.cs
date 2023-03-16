using System.Numerics;

namespace FibonacciCalculation
{
    /// <summary>
    /// Uses for next Fibonacci number calculation and for message exchange in Rabbit.
    /// </summary>
    /// <param name="Previous">Previous number in the Fibonacci sequence.</param>
    /// <param name="Current">Current number in the Fibonacci sequence.</param>
    public class FibonacciNumber
    {
        public FibonacciNumber(byte[] previous, byte[] current)
        {
            Previous = previous;
            Current = current;
        }

        public byte[] Previous { get; set; }
        public byte[] Current { get; set; }

        public override string ToString()
        {
            return $"Previous:\n{ new BigInteger(Previous)}\nCurrent:\n{new BigInteger(Current)}\n";
        }
    }


}