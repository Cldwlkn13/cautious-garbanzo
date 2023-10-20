using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class SportsbookMarketFilter : MarketFilter
    {
        [JsonProperty(PropertyName = "timeRange")]
        public TimeRange TimeRange { get; set; }

        [JsonProperty(PropertyName = "marketTypes")]
        public string[] MarketTypes { get; set; }
    }
}

