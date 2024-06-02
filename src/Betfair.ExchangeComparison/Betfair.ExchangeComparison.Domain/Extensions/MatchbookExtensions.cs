using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class MatchbookExtensions
    {
        public static string HorseRacingId = "24735152712200";

        public static string CleanRunnerName(this string  runnerName)
        {
            var split = runnerName.Split(new char[] { ' ' }, 2);
            return split[1].Trim();
        }

        public static MatchbookMarket WinMarket(this MatchbookEvent matchbookEvent)
        {
            return matchbookEvent.Markets?.FirstOrDefault(m => m.Name == "WIN") ?? 
                new MatchbookMarket();
        }

        public static List<MatchbookEvent> FilterUki(this List<MatchbookEvent> matchbookEvents)
        {
            long UkCategoryId = 10812638253700;
            long IreCategoryId = 10812641776701;

            return matchbookEvents
                    .Where(e => e.CategoryId.Contains(UkCategoryId) ||
                                e.CategoryId.Contains(IreCategoryId))
                    .ToList();
        }

        public static int GetClosestTick(this double price)
        {
            var bestMatch = MatchbookOddsLadder.PriceToTick.OrderBy(e => Math.Abs(e.Key - price)).FirstOrDefault();
            return bestMatch.Value;
        }
        public static int GetClosestTick(this float price)
        {
            var bestMatch = MatchbookOddsLadder.PriceToTick.OrderBy(e => Math.Abs(e.Key - price)).FirstOrDefault();
            return bestMatch.Value;
        }
    }
}
