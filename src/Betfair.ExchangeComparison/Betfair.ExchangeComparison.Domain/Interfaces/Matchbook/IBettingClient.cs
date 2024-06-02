using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.Interfaces.Matchbook
{
    public interface IBettingClient : IMatchbookClient
    {
        Task<OffersResponse?> PostOffer(OffersRequest request);
        Task<CancelledOfferResponse?> DeleteOffer(long runnerId);
        Task<OffersResponse?> GetOffers(long[] marketIds, int offset);
        Task<Offer?> GetOffer(long offerId);
        Task<AggregatedMatchedBets?> GetAggregatedMatchedBets(long[] marketIds, int offset);
        Task<PositionsResponse?> GetPositions(long[] marketIds);
        public string SessionToken { get; set; }
    }
}
