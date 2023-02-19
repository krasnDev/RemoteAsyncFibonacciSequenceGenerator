
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

            app.MapPost("/api/fib", server.PostHandler);

            app.Run();
        }
    }
}