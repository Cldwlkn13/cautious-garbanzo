using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class BettingClient : MatchbookClient, IBettingClient
    {
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/edge/rest/v2/offers"; }

        public BettingClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) :
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<OffersResponse?> PostOffer(string sessionToken, OffersRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));

            try
            {
                Console.WriteLine($"Post New Offer: {JsonConvert.SerializeObject(request)}");
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var message = await _httpClient.PostAsync(new Uri($"{EndpointAddress}"), content);
                var offersResponse = await HandleResponse<OffersResponse>(message);
                Console.WriteLine($"Post Offer Request Successful: {JsonConvert.SerializeObject(request)}");
                return offersResponse;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"BettingClient:PostOffer Failed! Exception={exception.Message} Payload={json}");
                return null;
            }
        }

        public async Task<OffersResponse?> GetOffers(string sessionToken, long[] marketIds)
        {
            var idsAsString = String.Join(",", marketIds);
            try
            {
                Console.WriteLine($"Getting Offers For Markets: {idsAsString}");
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = "?offset=0&" +
                    "per-page=20" +
                    $"&market-ids={idsAsString}" +
                    "&include-edits=true" +
                    "&aggregation-type=summary";
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var offersResponse = await HandleResponse<OffersResponse>(message);
                Console.WriteLine($"Get Offers Request Successful!");
                return offersResponse;
            }
            catch
            {
                Console.WriteLine($"BettingClient:GetOffers {idsAsString} Failed!");
                return null;
            }
        }

        public async Task<Offer?> GetOffer(string sessionToken, long offerId)
        {
            try
            {
                Console.WriteLine($"Getting Offer: {offerId}");
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/{offerId}?include-edits=false";
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var offersResponse = await HandleResponse<Offer>(message);
                Console.WriteLine($"Getting Offer Request Successful: {offerId}");
                return offersResponse;
            }
            catch 
            {
                Console.WriteLine($"BettingClient:GetOffer {offerId} Failed!");
                return null;
            }
        }

        public async Task<AggregatedMatchedBets?> GetAggregatedMatchedBets(string sessionToken, long[] marketIds, int offset)
        {
            var idsAsString = String.Join(",", marketIds);
            try
            {
                Console.WriteLine($"Getting AggregatedMatchedBets For Markets: {idsAsString}");
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var message = await _httpClient.GetAsync(new Uri($"{DomainAddress}/edge/rest/v2/matched-bets/aggregated?offset={offset}" +
                    $"&per-page=20" +
                    $"&aggregation-type=average" +
                    $"&market-ids={idsAsString}"));

                var aggregatedMatchedBets = await HandleResponse<AggregatedMatchedBets>(message);
                Console.WriteLine($"Get AggregatedMatchedBets Request Successful!");
                return aggregatedMatchedBets;
            }
            catch 
            {
                Console.WriteLine($"BettingClient:GetAggregatedMatchedBets {idsAsString} Failed!");
                return null;
            }
        }

        public async Task<PositionsResponse?> GetPositions(string sessionToken, long[] marketIds)
        {
            var idsAsString = String.Join(",", marketIds);
            try
            {
                Console.WriteLine($"Getting Positions For Markets: {idsAsString}");
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var message = await _httpClient.GetAsync(new Uri($"{DomainAddress}/edge/rest/account/positions?per-page=200" +
                    $"&market-ids={idsAsString}"));
                var positionsResponse = await HandleResponse<PositionsResponse>(message);
                Console.WriteLine($"Get Positions Request Successful!");
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
