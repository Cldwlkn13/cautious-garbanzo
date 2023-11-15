using System;
namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class CommonWilliamHillExtensions
    {
        public static Dictionary<string, string> WilliamHillMarketTypeMaps()
        {
            return new Dictionary<string, string>
            {
                { "90 Minutes", "Match Odds" },
                { "Total Match Goals Over/Under 1.5 Goals", "Over/Under Total Goals 1.5" },
                { "Total Match Goals Over/Under 2.5 Goals", "Over/Under Total Goals 2.5"},
                { "Total Match Goals Over/Under 3.5 Goals", "Over/Under Total Goals 3.5" },
                { "Both Teams To Score", "Both Teams To Score" }
            };
        }
    }
}

