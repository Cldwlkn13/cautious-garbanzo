using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class AccountClient : MatchbookClient, IAccountClient
    {
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/edge/rest/account"; }

        public AccountClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) :
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<Account> GetAccount(string sessionToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
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

        public async Task<BalanceResponse> GetBalance(string sessionToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
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
