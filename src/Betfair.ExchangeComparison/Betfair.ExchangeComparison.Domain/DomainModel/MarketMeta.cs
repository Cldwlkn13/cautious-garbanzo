using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class MarketMeta
    {
        public int RunnerCount { get; set; }
        public int Furlongs { get; set; }
        public bool IsJumps { get; set; }
        public bool IsHurdle { get; set; }
        public bool IsChase { get; set; }
        public bool IsNationalHuntFlat { get; set; }
        public bool IsTurf { get; set; }
        public bool IsHandicap { get; set; }
        public bool IsMaidenOrNovice { get; set; }
        public double TotalMarketTraded { get; set; }
        public double FavouritePrice { get; set; }
    }
}
