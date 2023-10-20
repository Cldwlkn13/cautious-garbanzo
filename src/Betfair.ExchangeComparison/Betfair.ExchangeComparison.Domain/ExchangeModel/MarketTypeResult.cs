using System;
using System.Linq;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Exchange.Model
{

    public class MarketTypeResult
    {
        [JsonProperty(PropertyName = "marketType")]
        public string marketType { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int marketCount { get; set; }
    }
}
