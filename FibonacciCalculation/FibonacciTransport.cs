namespace FibonacciCalculation
{
    /// <summary>
    /// Uses for for message exchange.
    /// </summary>
    public struct FibonacciTransport
    {
        public FibonacciTransport(byte[] previous, byte[] current)
        {
            Previous = previous;
            Current = current;
        }

        public byte[] Previous { get; set; }
        public byte[] Current { get; set; }

    }
}
