﻿using System;
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
                                    .GetTransactions(sbankenAccount.AccountId, fromDate: DateTime.Now, toDate: DateTime.Now)
                                    .GetAwaiter()
                                    .GetResult();

                var budget = ynabClient.GetBudgetByName(budgetName)
                                .GetAwaiter()
                                .GetResult();

                var ynabAccounts = ynabClient.GetAccountsForBudget(budget.Id)
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
                    .AddTransient<SbankenClient>()
                    .AddTransient<YNABClient>();
                    
        }
    }
}
