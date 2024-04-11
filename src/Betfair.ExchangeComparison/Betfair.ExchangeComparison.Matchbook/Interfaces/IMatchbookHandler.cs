using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;

namespace Betfair.ExchangeComparison.Matchbook.Interfaces
{
    public interface IMatchbookHandler
    {
        //SESSION
        public string SessionToken { get; }
        Task PostSessionToken();
        Task GetSessionToken(bool refreshToken = true);
        Task DeleteSessionToken();

        //ACCOUNT
        Task<Account?> GetAccount();
        Task<BalanceResponse?> GetBalance();

        //CATALOGUE
        Task<List<MatchbookSport>> GetSports();
        Task<List<MatchbookSport>> GetAccountSports();
        Task<List<MatchbookEvent>> GetEvents();
        Task<MatchbookEvent> GetSingleEvent(long eventId);
        Task<List<MatchbookMarket>> GetMarkets(long eventId);
        Task<MatchbookMarket> GetSingleMarket(long eventId, long marketId);
        Task<List<MatchbookRunner>> GetRunners(long eventId, long marketId);
        Task<MatchbookRunner> GetSingleRunner(long eventId, long marketId, long runnerId);
        Task<List<Price>> GetPrices(long eventId, long marketId, long runnerId);

        //BETTING
        Task<OffersResponse?> PostOffer(OffersRequest request);
        Task<List<Offer>> GetOffers(long[] marketIds);
        Task<Offer> GetOffer(long offerId);
        Task<List<AggregatedMatchedBets>> GetAggregatedMatchedBets(long[] marketIds);
        Task<List<Position>> GetPositions(long[] marketIds);

        //REPORTS
    }
}
