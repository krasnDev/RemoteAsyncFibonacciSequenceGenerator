using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Transport;

namespace Application1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
            {
                services.AddHttpClient();
                services.AddLogging();
                services.RegisterEasyNetQ("host=localhost");
                services.AddTransient<Client>();
            }).Build();
            host.Start();

            using var client = host.Services.GetRequiredService<Client>();

            if (args.Length <= 0 || !int.TryParse(args[0], out int calcCount))
            {
                Console.WriteLine("Please enter number of calculations:");
                while (!int.TryParse(Console.ReadLine(), out calcCount))
                {
                    Console.WriteLine("Invalid input. An integer is required.");
                }
            }

            client.InitCalculations(calcCount);
            Console.ReadLine();
        }
    }
}