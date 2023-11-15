using System;
namespace Betfair.ExchangeComparison.Scraping.Extensions
{
    public static class WilliamHillMappingExtensions
    {
        public static IEnumerable<string> WilliamHillMarketGroups()
        {
            return new List<string>
            {
                "match-betting",
                "total-match-goals-overunder-15",
                "total-match-goals-overunder-25",
                "total-match-goals-overunder-35",
                "both-teams-to-score"
            };
        }
    }
}

