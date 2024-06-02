using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class ReportsClient : MatchbookClient, IReportsClient
    {
        private string _sessionToken;
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/edge/rest"; }
        public string SessionToken
        {
            get => _sessionToken;
            set
            {
                _httpClient.HandleSessionTokenHeader(value);
                _sessionToken = value;
            }
        }

        public ReportsClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) :
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<SettledBetsResponse> GetSettledBets(string sportId, string beforeEpoch, string afterEpoch, int offset)
        {
            try
            {
                var query = $"?offset={offset}" +
                    $"&per-page=20" +
                    $"&sport-ids={sportId}" +
                    $"&after={afterEpoch}" +
                    $"&before={beforeEpoch}";
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}/reports/v2/bets/settled{query}"));
                var settledBetsResponse = await HandleResponse<SettledBetsResponse>(message);
                return settledBetsResponse; 
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetSettledBets Request Failed!");
                throw new InvalidDataException($"GetSettledBets Request Failed!");
            }
        }
    }
}
