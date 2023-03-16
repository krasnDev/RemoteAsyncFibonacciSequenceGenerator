using EasyNetQ;
using FibonacciCalculation;
using Microsoft.Extensions.Logging;
namespace Transport
{
    /// <summary>
    /// Provides calculation of the next number in the Fibonacci sequence. The current number is received by the HTTP Post handler, the next calculated number is published via Rabbit. 
    /// </summary>
    public class Server : IDisposable
    {
        private const string postedMessage = "Got posted message from task {ID} :\n{Value}";
        private const string calculatedMessage = "Calculated next Fibonacci number for task {ID} :\n{Value}";
        private readonly ILogger logger;
        private readonly IBus bus;
        private readonly FibonacciComputer fibonacciComputer = new();

        public Server(ILoggerFactory loggerFactory, IBus bus)
        {
            logger = loggerFactory.CreateLogger(string.Empty);
            this.bus = bus;
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        /// <summary>
        /// HTTP Post handler.
        /// </summary>
        /// <param name="message">Instance of base message class <see cref="TransportMessage{T}"/> with current Fibonacci number.</param>
        public async Task PostHandler(TransportMessage<FibonacciNumber> message)
        {
            try
            {
                logger.LogInformation(postedMessage, message.Id, message.Value.ToString());
                var next = fibonacciComputer.GetNext(message.Value);
                logger.LogInformation(calculatedMessage, message.Id, next.ToString());
                await bus.PubSub.PublishAsync(new TransportMessage<FibonacciNumber>(message.Id, next)).ConfigureAwait(false);
            } catch(Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
