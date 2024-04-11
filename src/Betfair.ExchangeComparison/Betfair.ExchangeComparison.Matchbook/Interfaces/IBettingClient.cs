using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Matchbook.Interfaces
{
    public interface IBettingClient
    {
        Task<OffersResponse?> PostOffer(string sessionToken, OffersRequest request);
        Task<OffersResponse?> GetOffers(string sessionToken, long[] marketIds);
        Task<Offer?> GetOffer(string sessionToken, long offerId);
        Task<AggregatedMatchedBets?> GetAggregatedMatchedBets(string sessionToken, long[] marketIds, int offset);
        Task<PositionsResponse?> GetPositions(string sessionToken, long[] marketIds);
    }
}
