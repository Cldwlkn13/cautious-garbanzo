using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class CommonOddscheckerExtensions
    {
        public static bool TryGetScrapedPriceByBookmaker(this IEnumerable<ScrapedPrice> scrapedPrices, Bookmaker bookmaker, out ScrapedPrice result)
        {
            result = new ScrapedPrice(0, bookmaker);

            var scrapedPrice = scrapedPrices.FirstOrDefault(s => s.Bookmaker == bookmaker);

            if (scrapedPrice == null) return false;

            result = scrapedPrice;
            return true;
        }

        public static bool TryGetScrapedEachWayTermsByBookmaker(this IEnumerable<ScrapedEachWayTerms> scrapedEachWayTerms, Bookmaker bookmaker, out ScrapedEachWayTerms result)
        {
            result = new ScrapedEachWayTerms(0, 0, bookmaker);

            var scrapedTerms = scrapedEachWayTerms.FirstOrDefault(s => s.Bookmaker == bookmaker);

            if (scrapedTerms == null) return false;

            result = scrapedTerms;
            return true;
        }
    }
}

