using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.Interfaces.Matchbook
{
    public interface ICatalogueClient : IMatchbookClient
    {
        Task<SportsResponse?> GetSports(int offset);
        Task<List<MatchbookSport>?> GetAccountSports();
        Task<EventsResponse> GetEvents(int offset);
        Task<MatchbookEvent> GetSingleEvent(long eventId);
        Task<MarketsResponse> GetMarkets(long eventId, int offset);
        Task<MatchbookMarket> GetSingleMarket(long eventId, long marketId);
        Task<RunnersResponse> GetRunners(long eventId, long marketId);
        Task<MatchbookRunner> GetSingleRunner(long eventId, long marketId, long runnerId);
        Task<PricesResponse> GetPrices(long eventId, long marketId, long runnerId);
        public string SessionToken { get; set; }
    }
}
