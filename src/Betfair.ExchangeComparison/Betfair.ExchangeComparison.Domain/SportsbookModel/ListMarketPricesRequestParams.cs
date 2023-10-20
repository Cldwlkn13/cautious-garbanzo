using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class ListMarketPricesRequestParams
    {
        [JsonProperty(PropertyName = "marketIds")]
        public IList<string> MarketIds { get; set; }
    }
}

