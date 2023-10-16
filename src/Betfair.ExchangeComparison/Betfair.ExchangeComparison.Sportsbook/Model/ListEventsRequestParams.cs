using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class ListEventsRequestParams
    {
        [JsonProperty(PropertyName = "marketFilter")]
        public SportsbookMarketFilter MarketFilter { get; set; }
    }
}

