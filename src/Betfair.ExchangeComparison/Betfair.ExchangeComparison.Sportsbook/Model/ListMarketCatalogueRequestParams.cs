using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class ListMarketCatalogueRequestParams
    {
        [JsonProperty(PropertyName = "marketFilter")]
        public SportsbookMarketFilter MarketFilter { get; set; }

        [JsonProperty(PropertyName = "maxResults")]
        public string MaxResults { get; set; }
    }
}

