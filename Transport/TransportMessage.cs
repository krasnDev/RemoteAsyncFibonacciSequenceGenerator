namespace Transport
{
    /// <summary>
    /// Base type for message exchange in Rabbit.
    /// </summary>
    /// <typeparam name="T">Type which need to send.</typeparam>
    public class TransportMessage<T> where T : struct
    {
        /// <summary>
        /// Creates an instance of message class.
        /// </summary>
        /// <param name="id">Numeric task identifier.</param>
        /// <param name="value">Instanse of <see cref="T"/>.</param>
        public TransportMessage(int id, T value) 
        {
            Id= id;
            Value = value;
        }
        public int Id { get;}
        public T Value { get;}
    }
}