using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class BettingClient : MatchbookClient, IBettingClient
    {
        private string _sessionToken;
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/edge/rest/v2/offers"; }
        public string SessionToken
        {
            get => _sessionToken;
            set
            {
                _httpClient.HandleSessionTokenHeader(value);
                _sessionToken = value;
            }
        }

        public BettingClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) :
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<OffersResponse?> PostOffer(OffersRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));

            try
            {
                _httpClient.HandleSessionTokenHeader(SessionToken);
                var message = await _httpClient.PostAsync(new Uri($"{EndpointAddress}"), content);
                var offersResponse = await HandleResponse<OffersResponse>(message);

                return offersResponse;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"BettingClient:PostOffer Failed! Exception={exception.Message} Payload={json}");
                return null;
            }
        }

        public async Task<CancelledOfferResponse?> DeleteOffer(long runnerId)
        {
            try
            {
                _httpClient.HandleSessionTokenHeader(SessionToken);
                var query = $"?runner-ids={runnerId}";
                var message = await _httpClient.DeleteAsync(new Uri($"{EndpointAddress}{query}"));
                var offersResponse = await HandleResponse<CancelledOfferResponse>(message);

                return offersResponse;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"BettingClient:DeleteOffer Failed! Exception={exception.Message} RunnerId={runnerId}");
                return null;
            }
        }

        public async Task<OffersResponse?> GetOffers(long[] marketIds, int offset)
        {
            var idsAsString = String.Join(",", marketIds);
            try
            {
                _httpClient.HandleSessionTokenHeader(SessionToken);
                var query = $"?offset={offset}&" +
                    $"per-page=20" +
                    $"&market-ids={idsAsString}" +
                    "&include-edits=true" +
                    "&aggregation-type=summary";
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var offersResponse = await HandleResponse<OffersResponse>(message);
                return offersResponse;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"BettingClient:GetOffers {idsAsString} Failed! {exception.Message}");
                return null;
            }
        }

        public async Task<Offer?> GetOffer(long offerId)
        {
            try
            {
                _httpClient.HandleSessionTokenHeader(SessionToken);
                var query = $"/{offerId}?include-edits=false";
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var offersResponse = await HandleResponse<Offer>(message);
                return offersResponse;
            }
            catch 
            {
                Console.WriteLine($"BettingClient:GetOffer {offerId} Failed!");
                return null;
            }
        }

        public async Task<AggregatedMatchedBets?> GetAggregatedMatchedBets(long[] marketIds, int offset)
        {
            var idsAsString = String.Join(",", marketIds);
            try
            {
                _httpClient.HandleSessionTokenHeader(SessionToken);
                var message = await _httpClient.GetAsync(new Uri($"{DomainAddress}/edge/rest/v2/matched-bets/aggregated?offset={offset}" +
                    $"&per-page=20" +
                    $"&aggregation-type=average" +
                    $"&market-ids={idsAsString}"));

                var aggregatedMatchedBets = await HandleResponse<AggregatedMatchedBets>(message);
                return aggregatedMatchedBets;
            }
            catch 
            {
                Console.WriteLine($"BettingClient:GetAggregatedMatchedBets {idsAsString} Failed!");
                return null;
            }
        }

        public async Task<PositionsResponse?> GetPositions(long[] marketIds)
        {
            var idsAsString = String.Join(",", marketIds);
            try
            {
                _httpClient.HandleSessionTokenHeader(SessionToken);
                var message = await _httpClient.GetAsync(new Uri($"{DomainAddress}/edge/rest/account/positions?per-page=200" +
                    $"&market-ids={idsAsString}"));
                var positionsResponse = await HandleResponse<PositionsResponse>(message);
                return positionsResponse;
            }
            catch 
            {
                Console.WriteLine($"BettingClient:GetPositions {idsAsString} Failed!");
                return null;
            }
        }
    }
}
