using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace SbankenYnab
{
    class SbankenClient
    {
        private HttpClient _client;

        public async Task init()
        {
            /**
            * Client credentials and customerId
            * Here Oauth2 is being used with "client credentials": The "client" is the application, and we require a secret 
            * known only to the application.
            * Both the client id and the secret (password) are generated in Sbanken.
            * The customer id is your birth- and personal number (national identification number)
            */

            var clientId = "******************************";
            var secret = "******************************";
            var customerId = "*****************";

            /** Setup constants */
            var discoveryEndpoint = "https://auth.sbanken.no/identityserver";
            var apiBaseAddress = "https://api.sbanken.no";
            var bankBasePath = "/exec.bank";

            /** Initialize HttpClient */
            _client = new HttpClient()
            {
                BaseAddress = new Uri(apiBaseAddress),
                DefaultRequestHeaders =
                {
                    { "customerId", customerId }
                }
            };

            /**
             * Connect to Sbanken
             *
             * Here the application connect to the identity server endpoint to retrieve a access token.
             */

            // First: get the OpenId configuration from Sbanken.
            var disco = await _client.GetDiscoveryDocumentAsync(discoveryEndpoint);
            if (disco.IsError) throw new Exception(disco.Error);

            // The application now knows how to talk to the token endpoint.

            // Second: the application authenticates against the token endpoint
            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest{
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = secret
            });

            if (tokenResponse.IsError) throw new Exception(tokenResponse.ErrorDescription);

            // The application now has an access token.

            // Finally: Set the access token on the connecting client. 
            // It will be used with all requests against the API endpoints.
            _client.SetBearerToken(tokenResponse.AccessToken);
        }
    }
}