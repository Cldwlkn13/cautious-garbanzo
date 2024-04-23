using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class CatalogueClient : MatchbookClient, ICatalogueClient
    {
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/edge/rest"; }

        public CatalogueClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) :
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<SportsResponse?> GetSports(string sessionToken, int offset)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"offset={offset}&per-page=20&order=name%20asc&status=active";
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}/lookups/sports?{query}"));
                var sportsResponse = await HandleResponse<SportsResponse>(message);
                return sportsResponse;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetSports Request Failed!");
                throw new InvalidDataException($"GetSports Request Failed!");
            }
        }

        public async Task<List<MatchbookSport>?> GetAccountSports(string sessionToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}/account/sports"));
                var sports = await HandleResponse<List<MatchbookSport>>(message);
                return sports;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetAccountSports Request Failed!");
                throw new InvalidDataException($"GetAccountSports Request Failed!");
            }
        }

        public async Task<EventsResponse> GetEvents(string sessionToken, int offset)
        {
            var BeforeEpoch = DateTime.Now.Date.AddDays(1).ToEpochTimeSeconds().ToString();
            var HorseRacingId = "24735152712200";

            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events?offset={offset}" +
                    "&per-page=20" +
                    $"&before={BeforeEpoch}" +
                    $"&sport-ids={HorseRacingId}" +
                    "&states=open" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&include-prices=true" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR" +
                    "&markets-limit=1";

                return new EventsResponse() { Events = new List<MatchbookEvent>() };

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var eventsResponse = await HandleResponse<EventsResponse>(message);
                eventsResponse.Events = eventsResponse.Events.FilterUki();
                eventsResponse.Events.ForEach(e => e.ApplyTicks());
                return eventsResponse;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetEvents Request Failed!");
                throw new InvalidDataException($"GetEvents Request Failed!");
            }
        }

        public async Task<MatchbookEvent> GetSingleEvent(string sessionToken, long eventId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events/{eventId}?" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&include-prices=true" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR" +
                    "&markets-limit=1";

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var @event = await HandleResponse<MatchbookEvent>(message);
                return @event;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetEvent {eventId} Request Failed!");
                throw new InvalidDataException($"GetEvent {eventId} Request Failed!");
            }
        }

        public async Task<MarketsResponse> GetMarkets(string sessionToken, long eventId, int offset)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events/{eventId}/markets?offset={offset}" +
                    "&per-page=3" +
                    "&states=open" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&include-prices=true" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR";

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var markets = await HandleResponse<MarketsResponse>(message);
                return markets;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetMarkets {eventId} Request Failed!");
                throw new InvalidDataException($"GetMarkets {eventId} Request Failed!");
            }
        }

        public async Task<MatchbookMarket> GetSingleMarket(string sessionToken, long eventId, long marketId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events/{eventId}/markets/{marketId}?" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&include-prices=true" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR";

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var market = await HandleResponse<MatchbookMarket>(message);
                return market;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetSingleMarket {eventId} {marketId} Request Failed!");
                throw new InvalidDataException($"GetSingleMarket {eventId} {marketId} Request Failed!");
            }
        }

        public async Task<RunnersResponse> GetRunners(string sessionToken, long eventId, long marketId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events/{eventId}/markets/{marketId}?" +
                    "&states=open" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&include-prices=true" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR";

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var runners = await HandleResponse<RunnersResponse>(message);
                return runners;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetRunners {eventId} {marketId} Request Failed!");
                throw new InvalidDataException($"GetRunners {eventId} {marketId} Request Failed!");
            }
        }

        public async Task<MatchbookRunner> GetSingleRunner(string sessionToken, long eventId, long marketId, long runnerId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events/{eventId}/markets/{marketId}/runners/{runnerId}?" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&include-prices=true" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR";

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var runner = await HandleResponse<MatchbookRunner>(message);
                return runner;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetSingleRunner {eventId} {marketId} {runnerId} Request Failed!");
                throw new InvalidDataException($"GetSingleRunner {eventId} {marketId} {runnerId} Request Failed!");
            }
        }

        public async Task<PricesResponse> GetPrices(string sessionToken, long eventId, long marketId, long runnerId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var query = $"/events/{eventId}/markets/{marketId}/runners/{runnerId}/prices?" +
                    "&exchange-type=back-lay" +
                    "&odds-type=DECIMAL" +
                    "&price-depth=3" +
                    "&price-mode=expanded" +
                    "&include-event-participants=true" +
                    "&exclude-mirrored-prices=false" +
                    "&currency=EUR";

                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}{query}"));
                var prices = await HandleResponse<PricesResponse>(message);
                return prices;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"GetPrices {eventId} {marketId} {runnerId} Request Failed!");
                throw new InvalidDataException($"GetPrices {eventId} {marketId} {runnerId} Request Failed!");
            }
        }
    }
}
