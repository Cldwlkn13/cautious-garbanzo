using System;
using Betfair.ExchangeComparison.Exchange.Model;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Sportsbook.Model
{
    public class ListCompetitionsRequestParams
    {
        [JsonProperty(PropertyName = "marketFilter")]
        public SportsbookMarketFilter MarketFilter { get; set; }
    }
}

