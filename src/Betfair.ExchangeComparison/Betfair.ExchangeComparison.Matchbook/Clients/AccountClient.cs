using Betfair.ExchangeComparison.Domain.Matchbook;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Domain.Extensions;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class AccountClient : MatchbookClient, IAccountClient
    {
        private string _sessionToken; 
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/edge/rest/account"; }
        public string SessionToken
        {
            get => _sessionToken; 
            set { 
                _httpClient.HandleSessionTokenHeader(value); 
                _sessionToken = value; 
            } 
        }

        public AccountClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) :
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<Account> GetAccount()
        {
            try
            {
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}"));
                var accountResponse = await HandleResponse<Account>(message);
                return accountResponse;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Account Request Failed!");
                throw new InvalidDataException($"Account Request Failed!");
            }
        }

        public async Task<BalanceResponse> GetBalance()
        {
            try
            {
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}/balance"));
                var balance = await HandleResponse<BalanceResponse>(message);
                return balance;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Account Request Failed!");
                throw new InvalidDataException($"Account Request Failed!");
            }
        }
    }
}
