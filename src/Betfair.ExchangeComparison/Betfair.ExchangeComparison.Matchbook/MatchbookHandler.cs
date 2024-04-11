using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using Exception = System.Exception;

namespace Betfair.ExchangeComparison.Matchbook
{
    public class MatchbookHandler : IMatchbookHandler
    {
        private readonly ISessionClient _sessionClient;
        private readonly IAccountClient _accountClient;
        private readonly ICatalogueClient _catalogueClient;
        private readonly IBettingClient _bettingClient;
        private readonly IOptions<MatchbookSettings> _settings;

        public MatchbookHandler(ISessionClient client, IAccountClient accountClient, ICatalogueClient catalogueClient,
            IBettingClient bettingClient, IOptions<MatchbookSettings> settings)
        {
            _sessionClient = client;
            _accountClient = accountClient;
            _catalogueClient = catalogueClient;
            _bettingClient = bettingClient;
            _settings = settings;
        }

        //SESSION
        public string SessionToken { get; private set; }

        public async Task PostSessionToken()
        {
            SessionToken = await _sessionClient.PostSessionToken();
        }

        public async Task GetSessionToken(bool refreshToken = true)
        {
            SessionToken = await _sessionClient.GetSessionToken(refreshToken);
        }

        public async Task DeleteSessionToken()
        {
            SessionToken = await _sessionClient.DeleteSessionToken(SessionToken);
        }

        //ACCOUNT
        public async Task<Account?> GetAccount()
        {
            try
            {
                return await _accountClient.GetAccount(SessionToken);
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
                return await _accountClient.GetBalance(SessionToken);
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
                do
                {
                    var response = await _catalogueClient.GetSports(SessionToken, offset);
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
                return await _catalogueClient.GetAccountSports(SessionToken) ??
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
                do
                {
                    var response = await _catalogueClient.GetEvents(SessionToken, offset);
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
                return await _catalogueClient.GetSingleEvent(SessionToken, eventId);
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
                do
                {
                    var response = await _catalogueClient.GetMarkets(SessionToken, eventId, offset);
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
                return await _catalogueClient.GetSingleMarket(SessionToken, eventId, marketId);
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
                var response = await _catalogueClient.GetRunners(SessionToken, eventId, marketId);
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
                return await _catalogueClient.GetSingleRunner(SessionToken, eventId, marketId, runnerId);
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
                var response = await _catalogueClient.GetPrices(SessionToken, eventId, marketId, runnerId);
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
                var response = await _bettingClient.PostOffer(SessionToken, request) ??
                        throw new NullReferenceException($"MatchbookHandler:Post Offer Failed! " +
                        $"{JsonConvert.SerializeObject(request)}");

                return response!;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<List<Offer>> GetOffers(long[] marketIds)
        {
            var idsAsString = String.Join(",", marketIds); 

            try
            {
                var response = await _bettingClient.GetOffers(SessionToken, marketIds) ??
                        throw new NullReferenceException($"Could not load Offers");

                return response!.Offers;
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
                var response = await _bettingClient.GetOffer(SessionToken, offerId) ??
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
                int offset = 0;
                int total = 0;
                do
                {
                    var response = await _bettingClient.GetAggregatedMatchedBets(SessionToken, marketIds, offset) ??
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
                do
                {
                    var response = await _bettingClient.GetPositions(SessionToken, marketIds) ??
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
    }
}

