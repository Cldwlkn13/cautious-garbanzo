using System;
namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class Rule4Deduction
    {
        public double deduction { get; set; }
        public DateTime timeFrom { get; set; }
        public DateTime timeTo { get; set; }
        public string deductionPriceType { get; set; }
    }
}

