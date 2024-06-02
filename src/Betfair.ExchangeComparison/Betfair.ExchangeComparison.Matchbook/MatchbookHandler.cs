using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Exchange.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace Betfair.ExchangeComparison.Matchbook
{
    public class MatchbookHandler : IMatchbookHandler
    {
        private readonly ISessionClient _sessionClient;
        private readonly IAccountClient _accountClient;
        private readonly ICatalogueClient _catalogueClient;
        private readonly IBettingClient _bettingClient;
        private readonly IReportsClient _reportsClient;
        private readonly IOptions<MatchbookSettings> _settings;

        public MatchbookHandler(ISessionClient client, IAccountClient accountClient, ICatalogueClient catalogueClient,
            IBettingClient bettingClient, IReportsClient reportsClient, IOptions<MatchbookSettings> settings)
        {
            _sessionClient = client;
            _accountClient = accountClient;
            _catalogueClient = catalogueClient;
            _bettingClient = bettingClient;
            _reportsClient = reportsClient;
            _settings = settings;
        }

        //SESSION
        public string SessionToken { get; private set; }

        public async Task PostSessionToken()
        {
            SessionToken = await _sessionClient.PostSessionToken();
            UpdateClients(SessionToken);
        }

        public async Task GetSessionToken(bool refreshToken = true)
        {
            var result = await _sessionClient.GetSessionToken(refreshToken);
            if(result != SessionToken)
            {
                UpdateClients(result);
            }
            SessionToken = result;
        }

        public async Task DeleteSessionToken()
        {
            SessionToken = await _sessionClient.DeleteSessionToken(SessionToken);
        }

        private void UpdateClients(string token)
        {
            _accountClient.SessionToken = token;
            _bettingClient.SessionToken = token;
            _catalogueClient.SessionToken = token;
            _reportsClient.SessionToken = token;
        }

        //ACCOUNT
        public async Task<Account?> GetAccount()
        {
            try
            {
                return await _accountClient.GetAccount();
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"Account Request Failed. Exception={exception}");
                return null;
            }
        }

        public async Task<BalanceResponse?> GetBalance()
        {
            try
            {
                return await _accountClient.GetBalance();
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"Balance Request Failed. Exception={exception}");
                return null;
            }
        }

        //CATALOGUE
        public async Task<List<MatchbookSport>> GetSports()
        {
            var result = new List<MatchbookSport>();
            var offset = 0;
            var total = 0;
            try
            {
                await GetSessionToken(true);
                do
                {
                    var response = await _catalogueClient.GetSports(offset);
                    result.AddRange(response!.Sports);
                    total = response.Total;
                    offset += response.PerPage;
                }
                while (offset < total);

                return result;
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"Sports Request Failed. Exception={exception}");
                return result;
            }
        }

        public async Task<List<MatchbookSport>> GetAccountSports()
        {
            var result = new List<MatchbookSport>();

            try
            {
                await GetSessionToken(true);
                return await _catalogueClient.GetAccountSports() ??
                    throw new NullReferenceException($"API responded with NULL Account Sports");
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetAccountSports Request Failed. Exception={exception}");
                return result;
            }
        }

        public async Task<List<MatchbookEvent>> GetEvents()
        {
            var result = new List<MatchbookEvent>();
            var offset = 0;
            var total = 0;
            try
            {
                await GetSessionToken(true);
                do
                {
                    var response = await _catalogueClient.GetEvents(offset);
                    result.AddRange(response!.Events);
                    total = response.Total;
                    offset += response.PerPage;
                }
                while (offset < total);

                return result;
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetEvents Request Failed. Exception={exception}");
                return result;
            }
        }

        public async Task<MatchbookEvent> GetSingleEvent(long eventId)
        {
            try
            {
                await GetSessionToken(true);
                return await _catalogueClient.GetSingleEvent(eventId);
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetSingleEvent {eventId} Request Failed. Exception={exception}");
                return new MatchbookEvent();
            }
        }

        public async Task<List<MatchbookMarket>> GetMarkets(long eventId)
        {
            var result = new List<MatchbookMarket>();
            var offset = 0;
            var total = 0;
            try
            {
                await GetSessionToken(true);
                do
                {
                    var response = await _catalogueClient.GetMarkets(eventId, offset);
                    result.AddRange(response!.Markets);
                    total = response.Total;
                    offset += response.PerPage;
                }
                while (offset < total);

                return result;
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetMarkets {eventId} Request Failed. Exception={exception}");
                return result;
            }
        }

        public async Task<MatchbookMarket> GetSingleMarket(long eventId, long marketId)
        {
            try
            {
                await GetSessionToken(true);
                return await _catalogueClient.GetSingleMarket(eventId, marketId);
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetSingleMarket {eventId} {marketId} Request Failed. Exception={exception}");
                return new MatchbookMarket();
            }
        }

        public async Task<List<MatchbookRunner>> GetRunners(long eventId, long marketId)
        {
            try
            {
                await GetSessionToken(true);
                var response = await _catalogueClient.GetRunners(eventId, marketId);
                return response.Runners;
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetRunners {eventId} {marketId} Request Failed. Exception={exception}");
                return new List<MatchbookRunner>();
            }
        }

        public async Task<MatchbookRunner> GetSingleRunner(long eventId, long marketId, long runnerId)
        {
            try
            {
                await GetSessionToken(true);
                return await _catalogueClient.GetSingleRunner(eventId, marketId, runnerId);
            }
            catch (InvalidDataException exception)
            {
                Console.WriteLine($"GetSingleRunner {eventId} {marketId} {runnerId} Request Failed. Exception={exception}");
                return new MatchbookRunner();
            }
        }

        public async Task<List<Price>> GetPrices(long eventId, long marketId, long runnerId)
        {
            try
            {
                await GetSessionToken(true);
                var response = await _catalogueClient.GetPrices(eventId, marketId, runnerId);
                return response.Prices;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"GetPrices {eventId} {marketId} {runnerId} Request Failed. Exception={exception}");
                return new List<Price>();
            }
        }

        //BETTING
        public async Task<OffersResponse?> PostOffer(OffersRequest request)
        {
            try
            {
                await GetSessionToken(true);
                var response = await _bettingClient.PostOffer(request) ??
                        throw new NullReferenceException($"MatchbookHandler:Post Offer Failed! " +
                        $"{JsonConvert.SerializeObject(request)}");

                return response!;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<List<Offer>> DeleteOffers(long runnerId)
        {
            var result = new List<Offer>();
            try
            {
                await GetSessionToken(true);

  
                var response = await _bettingClient.DeleteOffer(runnerId) ??
                       throw new NullReferenceException($"Could not delete Offers");

                result.AddRange(response.Offers);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Offer>> GetOffers(long[] marketIds)
        {

            var result = new List<Offer>();
            var offset = 0;
            try
            {
                await GetSessionToken(true);

                int responseCount;
                do
                {
                    var response = await _bettingClient.GetOffers(marketIds, offset) ??
                            throw new NullReferenceException($"Could not load Offers");

                    result.AddRange(response.Offers);
                    responseCount = response.Offers.Count();

                    offset += 20;
                }
                while (responseCount > 0);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Offer> GetOffer(long offerId)
        {
            try
            {
                await GetSessionToken(true);
                var response = await _bettingClient.GetOffer(offerId) ??
                        throw new NullReferenceException($"Could not load Offer");

                return response!;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<AggregatedMatchedBets>> GetAggregatedMatchedBets(long[] marketIds)
        {
            var idsAsString = String.Join(",", marketIds);

            var result = new List<AggregatedMatchedBets>();
            try
            {
                await GetSessionToken(true);
                int offset = 0;
                int total = 0;
                do
                {
                    var response = await _bettingClient.GetAggregatedMatchedBets(marketIds, offset) ??
                        throw new NullReferenceException($"Could not load AggregatedMatchedBets");

                    result.Add(response);

                    offset += 20;
                    total = response.Total;
                }
                while (offset < total);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Position>> GetPositions(long[] marketIds)
        {
            var idsAsString = String.Join(",", marketIds);

            var result = new List<Position>();
            var offset = 0;
            var total = 0;
            try
            {
                await GetSessionToken(true);
                do
                {
                    var response = await _bettingClient.GetPositions(marketIds) ??
                        throw new NullReferenceException($"Could not load Positions");

                    result.AddRange(response!.Positions);
                    total = response.Total;
                    offset += response.PerPage;
                }
                while (offset < total);

                return result;
            }
            catch 
            {
                throw;
            }
        }

        //REPORTS
        public async Task<List<SettledBetMarket>> GetSettledBets(string sportId, TimeRange timeRange)
        {
            var AfterEpoch = timeRange.From.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var BeforeEpoch = timeRange.To.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var result = new List<SettledBetMarket>();
            var offset = 0;
            var total = 0;
            try
            {
                await GetSessionToken(true);
                do
                {
                    var response = await _reportsClient.GetSettledBets(sportId, BeforeEpoch, AfterEpoch, offset) ??
                        throw new NullReferenceException($"Could not load Settled Bets");

                    result.AddRange(response!.markets);
                    total = response.total;
                    offset += response.perpage;
                }
                while (offset < total);

                return result;
            }
            catch
            {
                throw;
            }
        }

    }
}

