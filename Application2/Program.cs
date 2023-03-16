
using FibonacciCalculation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
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
                await System.Text.Json.JsonSerializer
                    .DeserializeAsync<TransportMessage<FibonacciNumber>>(request.Body)
                    .AsTask()
                    .ContinueWith(t => server.PostHandler(t.Result));
            });

            app.Run();
        }
    }
}