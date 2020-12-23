using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
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

            var budgetResponse = await _client.GetAsync($"/v1/budgets/?include_accounts=true");

            if (!budgetResponse.IsSuccessStatusCode) throw new Exception(budgetResponse.ReasonPhrase);

            var budgetResult = await budgetResponse.Content.ReadAsStringAsync();
            var budgetSummary = JsonConvert.DeserializeObject<Models.YNAB.BudgetSummaryResponse>(budgetResult);

            _logger.LogInformation($"Found {budgetSummary.Data.Budgets.Count} budgets. Trying to find budget by name \"{name}\"...");

            var budget = budgetSummary.Data.Budgets.Find(b => b.Name == name);

            if (budget == null) throw new ArgumentException($"No budget by name \"{name}\" was found.");

            _logger.LogInformation($"Found \"{name}\"");

            return budget;
        }

        public async Task AddTransactions(Models.YNAB.Budget budget, List<Models.Sbanken.Transaction> sbankenTransactions, String ynabAccountName = null)
        {
            _logger.LogInformation($"Attempting to add transactions to budget {budget.Id}...");

            Models.YNAB.Account account = null;

            if (ynabAccountName == null) {
                _logger.LogInformation("Attempting to find default account...");
                account = budget.Accounts[0];
            } else {
                _logger.LogInformation($"Attempting to find account by name {ynabAccountName}...");
                account = budget.Accounts.Find(a => a.Name == ynabAccountName);
            }

            if (account == null) throw new ArgumentException("No YNAB account was found.");

            _logger.LogInformation($"Found YNAB account {account.Name}");

            var ynabTransactions = new List<Models.YNAB.Transaction>();

            sbankenTransactions.ForEach(transaction => {
                var amount = transaction.Amount * 1000;
                var date = transaction.AccountingDate.ToString("yyyy-MM-dd");

                var ynabTransaction = new Models.YNAB.Transaction()
                {
                    account_id = account.Id,
                    amount = (int)amount,
                    date = date,
                    memo = $"Import: {transaction.Text}",
                    cleared = "uncleared",
                    import_id = $"YNAB:{amount.ToString()}:{date}:1"
                };

                ynabTransactions.Add(ynabTransaction);
            });

            _logger.LogInformation($"Readied {ynabTransactions.Count} transactions for import...");

            var data = new { transactions = ynabTransactions };
            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _client.PostAsync($"/v1/budgets/{budget.Id}/transactions", stringContent);

            if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);

            _logger.LogInformation("Successfully added transactions to YNAB!");
        }
    }
}