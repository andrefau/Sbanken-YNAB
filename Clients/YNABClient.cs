using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
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
    }
}