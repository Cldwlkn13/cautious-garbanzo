using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class CountryMapping
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public Dictionary<Sport, Dictionary<string, string>> CompetitionMaps { get; set; }
    }
}

