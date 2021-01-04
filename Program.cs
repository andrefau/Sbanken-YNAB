using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SbankenYnab.Clients;
using Serilog;

namespace SbankenYnab
{
    class Program
    {
        static void Main(string[] args)
        {
            /** Configure logger */
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            /** Set up dependency injection */
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            /** Get logger instance */
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            
            logger.LogInformation("*** START ***");

            if (args.Length < 2)
            {
                var errorMessage = @"Missing arguments. 
                    You must supply at least two arguments when running this program.
                    Example: dotnet run 'My account' 'My budget'
                    You can also supply an optional third argument that speciefies the account in YNAB.
                    Example: dotnet run 'My account' 'My budget' 'My YNAB account'";

                logger.LogError(errorMessage);
                logger.LogInformation("*** EXIT ***\n");
                return;
            }

            var accountName = args[0];
            var budgetName = args[1];
            var ynabAccountName = args.Length >= 3 ? args[2] : null;
            var sbankenClient = serviceProvider.GetService<SbankenClient>();
            var ynabClient = serviceProvider.GetService<YNABClient>();

            try
            {
                sbankenClient.Init().Wait();
                ynabClient.Init();

                var sbankenAccount = sbankenClient
                                .GetAccountByName(accountName)
                                .GetAwaiter()
                                .GetResult();

                var transactions = sbankenClient
                                    .GetTransactions(sbankenAccount.AccountId, fromDate: DateTime.Now.AddDays(-7), toDate: DateTime.Now)
                                    .GetAwaiter()
                                    .GetResult();

                if (transactions == null || transactions.Count == 0)
                {
                    logger.LogInformation("No transactions found.\n*** EXIT ***\n");
                    return;
                }

                var budget = ynabClient.GetBudgetByName(budgetName)
                                .GetAwaiter()
                                .GetResult();

                ynabClient.AddTransactions(budget, transactions, ynabAccountName)
                    .GetAwaiter()
                    .GetResult();
            } 
            catch (Exception ex) 
            {
                logger.LogError(ex, ex.Message);
                logger.LogInformation("*** EXIT ***\n");
                return;
            }

            logger.LogInformation("*** EXIT ***\n");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog())
                    .AddTransient<SbankenClient>()
                    .AddTransient<YNABClient>();
                    
        }
    }
}
