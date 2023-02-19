using EasyNetQ;
using FibonacciCalculation;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Transport
{
    /// <summary>
    /// Provides initialization of Fibonacci sequence calculations. The calculated numbers are received by Rabbit, the next request to calculate the number is sent via HTTP Post.
    /// </summary>
    public class Client : IDisposable
    {
        private const string UriString = "http://localhost:7000/";
        private const string PostEndpoint = "/api/fib";
        private const string Message = "Task {ID} got {Value}";
        private const string StartingCalculationsMessage = "Starting {calcCount} calculations.";
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IBus bus;
        private readonly string subscriptionId = $"Fibonacci calculation {Environment.ProcessId}";

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
        public async void InitCalculations(int calculationsCount)
        {
            await bus.PubSub.SubscribeAsync<TransportMessage<FibonacciNumber>>(subscriptionId, MessageHandle);
            Enumerable
                .Range(0, calculationsCount)
                .AsParallel()
                .Select(calcNum =>
                    Task.Run(() => MessageHandle(new TransportMessage<FibonacciNumber>(calcNum + 1, new FibonacciNumber(0, 1)))))
                .ToArray();
            logger.LogInformation(StartingCalculationsMessage, calculationsCount);
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(UriString);
            return httpClient;
        }

        private async void MessageHandle(TransportMessage<FibonacciNumber> message)
        {
            try
            {
                logger.LogInformation(Message, message.Id, message.Value);
                using var httpClient = GetHttpClient();  
                using var response = await httpClient.PostAsJsonAsync(PostEndpoint, message);
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