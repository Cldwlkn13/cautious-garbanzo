namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class CommonWilliamHillExtensions
    {
        /// <summary>
        /// Betfair to William Hill market names
        /// </summary>
        /// <returns></returns>
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

        public static string ValidateTrack(this string name)
        {
            var maps = TrackMaps();

            if (maps.TryGetValue(name, out string? value))
            {
                return value;
            }

            return name;
        }

        /// <summary>
        /// Betfair to William Hill track names
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> TrackMaps()
        {
            return new Dictionary<string, string>
            {
                { "Chelmsford City", "Chelmsford" }
            };
        }
    }
}

