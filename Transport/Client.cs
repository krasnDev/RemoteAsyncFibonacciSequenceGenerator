using EasyNetQ;
using FibonacciCalculation;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;

namespace Transport
{
    /// <summary>
    /// Provides initialization of Fibonacci sequence calculations. The calculated numbers are received by Rabbit, the next request to calculate the number is sent via HTTP Post.
    /// </summary>
    public class Client : IDisposable
    {
        private const string UriString = "http://localhost:7000/";
        private const string PostEndpoint = "/api/fib";
        private const string Message = "Task {ID} got\n{Value}";
        private const string StartingCalculationsMessage = "Starting {calcCount} calculations.";
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IBus bus;
        private readonly string subscriptionId = $"Fibonacci calculation {Environment.ProcessId}";
        private readonly FibonacciComputer fibonacciComputer = new();

        public Client(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, IBus bus)
        {
            logger = loggerFactory.CreateLogger(subscriptionId);
            this.httpClientFactory = httpClientFactory;
            this.bus = bus;
        }

        /// <summary>
        /// Starts <paramref name="calculationsCount"/> async calculations of Fibonacci sequence.
        /// </summary>
        /// <param name="calculationsCount">The number of calculations that need to be run.</param>
        public async Task InitCalculations(int calculationsCount)
        {
            await bus.PubSub.SubscribeAsync<TransportMessage<FibonacciTransport>>(subscriptionId, MessageHandle);
            Enumerable
                .Range(0, calculationsCount)
                .AsParallel()
                .Select(calcNum =>
                    Task.Run(() =>
                        MessageHandle(new TransportMessage<FibonacciTransport>(
                            calcNum + 1,
                            new FibonacciNumber(BigInteger.Zero, BigInteger.Zero).ToTransport()))))
                .ToArray();
            logger.LogInformation(StartingCalculationsMessage, calculationsCount);
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(UriString);
            return httpClient;
        }

        private async void MessageHandle(TransportMessage<FibonacciTransport> message)
        {
            try
            {
                var number = message.Value.FromTransport();
                logger.LogInformation(Message, message.Id, number.ToString());
                var nextNumber = fibonacciComputer.GetNext(number);
                using var httpClient = GetHttpClient();
                var fibTransport = nextNumber.ToTransport();
                var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new TransportMessage<FibonacciTransport>(message.Id, fibTransport));
                var byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using var response = await httpClient.PostAsync(PostEndpoint, byteContent);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    logger.LogError(error);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return;
            }
        }

        public void Dispose()
        {
            bus.Dispose();
        }
    }
}