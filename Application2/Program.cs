
using FibonacciCalculation;
using Transport;

namespace Application2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.RegisterEasyNetQ("host=localhost");
            builder.Services.AddLogging();
            builder.Services.AddTransient<Server>();
            var app = builder.Build();

            using var server = app.Services.GetRequiredService<Server>();

            app.MapPost("/api/fib", async (HttpRequest request) =>
            {
                try
                {
                    await System.Text.Json.JsonSerializer
                        .DeserializeAsync<TransportMessage<FibonacciTransport>>(request.Body)
                        .AsTask()
                        .ContinueWith(t => server.PostHandler(t.Result));
                }catch(Exception ex)
                {
                    app.Logger.LogError(ex.ToString());
                }
            });

            app.Run();
        }
    }
}