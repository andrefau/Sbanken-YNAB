using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SbankenYnab
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.log")
                .CreateLogger();
            
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            
            logger.LogInformation("*** START ***");

            if (args.Length != 2)
            {
                logger.LogError("Missing arguments.\nYou must supply two arguments when running this program.\nExample: dotnet run \"My account\" \"My budget\"");
                logger.LogInformation("*** EXIT ***");
                return;
            }

            var client = serviceProvider.GetService<SbankenClient>();

            try
            {
                client.init().Wait();
            } 
            catch (Exception ex) 
            {
                logger.LogError(ex, ex.Message);
                logger.LogInformation("*** EXIT ***");
                return;
            }

            logger.LogInformation("*** EXIT ***");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog())
                    .AddTransient<SbankenClient>();
                    
        }
    }
}
