using System;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Scraping.Extensions
{
    public static class OddscheckerMappingExtensions
    {
        public static bool TryMapBookmaker(this string bk, out Bookmaker result)
        {
            return OddscheckerBookmakerMappings().TryGetValue(bk, out result);
        }

        public static Dictionary<string, Bookmaker> OddscheckerBookmakerMappings()
        {
            return new Dictionary<string, Bookmaker>
            {
                { "WH", Bookmaker.WilliamHill },
                { "BY", Bookmaker.Boylesports },
                { "LD", Bookmaker.Ladbrokes }
            };
        }
    }
}

