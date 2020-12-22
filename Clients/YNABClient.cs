using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SbankenYnab.Credentials;

namespace SbankenYnab.Clients
{
    public class YNABClient
    {
        private HttpClient _client;
        private readonly ILogger _logger;

        public YNABClient(ILogger<YNABClient> logger)
        {
            _logger = logger;
        }

        public void Init()
        {
            var apiBaseAddress = "https://api.youneedabudget.com";

            /** Initialize HttpClient */
            _client = new HttpClient()
            {
                BaseAddress = new Uri(apiBaseAddress),
                DefaultRequestHeaders = 
                {
                    { "Authorization", "Bearer " + YNABCredentials.AccessToken }
                }
            };
        }

        public async Task<Models.YNAB.Budget> GetBudgetByName(String name)
        {
            _logger.LogInformation("Getting budgets from YNAB...");

            var budgetResponse = await _client.GetAsync($"/v1/budgets");

            if (!budgetResponse.IsSuccessStatusCode) throw new Exception(budgetResponse.ReasonPhrase);

            var budgetResult = await budgetResponse.Content.ReadAsStringAsync();
            var budgetSummary = JsonConvert.DeserializeObject<Models.YNAB.BudgetSummaryResponse>(budgetResult);

            _logger.LogInformation($"Found {budgetSummary.Data.Budgets.Count} budgets. Trying to find budget by name \"{name}\"...");

            var budget = budgetSummary.Data.Budgets.Find(b => b.Name == name);

            if (budget == null) throw new ArgumentException($"No budget by name \"{name}\" was found.");

            _logger.LogInformation($"Found \"{name}\"");

            return budget;
        }

        public async Task<List<Models.YNAB.Account>> GetAccountsForBudget(String budgetId)
        {
            _logger.LogInformation($"Getting accounts for budget {budgetId}...");

            var accountResponse = await _client.GetAsync($"/v1/budgets/{budgetId}/accounts");

            if (!accountResponse.IsSuccessStatusCode) throw new Exception(accountResponse.ReasonPhrase);

            var accountResult = await accountResponse.Content.ReadAsStringAsync();
            var account = JsonConvert.DeserializeObject<Models.YNAB.AccountResponse>(accountResult);

            _logger.LogInformation($"Found {account.Data.Accounts.Count} accounts for budget {budgetId}.");

            return account.Data.Accounts;
        }
    }
}