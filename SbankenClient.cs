using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using SbankenYnab.Credentials;
using SbankenYnab.Models;

namespace SbankenYnab
{
    class SbankenClient
    {
        private HttpClient _client;
        private readonly ILogger _logger;
        private const String _bankBasePath = "/exec.bank";

        public SbankenClient(ILogger<SbankenClient> logger)
        {
            _logger = logger;
        }

        public async Task Init()
        {
            /** Setup constants */
            var discoveryEndpoint = "https://auth.sbanken.no/identityserver";
            var apiBaseAddress = "https://api.sbanken.no";

            /** Initialize HttpClient */
            _client = new HttpClient()
            {
                BaseAddress = new Uri(apiBaseAddress),
                DefaultRequestHeaders =
                {
                    { "customerId",  SbankenCredentials.CustomerId }
                }
            };

            /**
             * Connect to Sbanken
             *
             * Here the application connect to the identity server endpoint to retrieve a access token.
             */

            // First: get the OpenId configuration from Sbanken.
            _logger.LogInformation("Getting disovery document from Sbanken...");
            var disco = await _client.GetDiscoveryDocumentAsync(discoveryEndpoint);
            if (disco.IsError) throw new Exception(disco.Error);

            // The application now knows how to talk to the token endpoint.

            // Second: the application authenticates against the token endpoint
            _logger.LogInformation("Getting access token from Sbanken...");
            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest{
                Address = disco.TokenEndpoint,
                ClientId = SbankenCredentials.ClientId,
                ClientSecret = SbankenCredentials.Secret
            });

            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

            // The application now has an access token.

            // Finally: Set the access token on the connecting client. 
            // It will be used with all requests against the API endpoints.
            _client.SetBearerToken(tokenResponse.AccessToken);
        }

        public async Task<Account> GetAccountByName(String name)
        {
            _logger.LogInformation("Gettin accounts from Sbanken...");

            var accountResponse = await _client.GetAsync($"{_bankBasePath}/api/v1/Accounts");

            if (!accountResponse.IsSuccessStatusCode) throw new Exception(accountResponse.ReasonPhrase);

            var accountResult = await accountResponse.Content.ReadAsStringAsync();
            var accountList = JsonConvert.DeserializeObject<AccountsList>(accountResult);

            _logger.LogInformation($"Found {accountList.AvailableItems} accounts. Trying to find account by name \"{name}\"...");

            var account = accountList.Items.Find(a => a.Name == name);

            if (account == null) throw new ArgumentException($"No account by name \"{name}\" was found.");

            _logger.LogInformation($"Found \"{name}\"");

            return account;
        }
    }
}