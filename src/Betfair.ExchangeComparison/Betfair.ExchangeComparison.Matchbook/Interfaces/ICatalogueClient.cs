using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Matchbook.Interfaces
{
    public interface ICatalogueClient
    {
        Task<SportsResponse?> GetSports(string sessionToken, int offset);
        Task<List<MatchbookSport>?> GetAccountSports(string sessionToken);
        Task<EventsResponse> GetEvents(string sessionToken, int offset);
        Task<MatchbookEvent> GetSingleEvent(string sessionToken, long eventId);
        Task<MarketsResponse> GetMarkets(string sessionToken, long eventId, int offset);
        Task<MatchbookMarket> GetSingleMarket(string sessionToken, long eventId, long marketId);
        Task<RunnersResponse> GetRunners(string sessionToken, long eventId, long marketId);
        Task<MatchbookRunner> GetSingleRunner(string sessionToken, long eventId, long marketId, long runnerId);
        Task<PricesResponse> GetPrices(string sessionToken, long eventId, long marketId, long runnerId);
    }
}
