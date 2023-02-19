namespace FibonacciCalculation
{
    /// <summary>
    /// Uses for next Fibonacci number calculation and for message exchange in Rabbit.
    /// </summary>
    /// <param name="Previous">Previous number in the Fibonacci sequence.</param>
    /// <param name="Current">Current number in the Fibonacci sequence.</param>
    public record FibonacciNumber(long Previous, long Current);
}