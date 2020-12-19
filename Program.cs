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
            /** Configure logger */
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.log")
                .CreateLogger();
            
            /** Set up dependency injection */
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            /** Get logger instance */
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            
            logger.LogInformation("*** START ***");

            if (args.Length != 2)
            {
                logger.LogError("Missing arguments.\nYou must supply two arguments when running this program.\nExample: dotnet run \"My account\" \"My budget\"");
                logger.LogInformation("*** EXIT ***");
                return;
            }

            var accountName = args[0];
            var budgetName = args[1];
            var sbankenClient = serviceProvider.GetService<SbankenClient>();

            try
            {
                sbankenClient.Init().Wait();

                var account = sbankenClient
                                .GetAccountByName(accountName)
                                .GetAwaiter()
                                .GetResult();

                var transactions = sbankenClient
                                    .GetTransactions(account.AccountId, fromDate: DateTime.Now, toDate: DateTime.Now)
                                    .GetAwaiter()
                                    .GetResult();
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
