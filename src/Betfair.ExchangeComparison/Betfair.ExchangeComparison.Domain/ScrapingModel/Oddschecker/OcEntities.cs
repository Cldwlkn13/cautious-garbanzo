using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker
{
    public class OcEntities
    {
        public OcEntities()
        {
        }

        [JsonProperty("entities")]
        public IEnumerable<OcMarketDefinition> OcMarketDefinitions { get; set; }
    }
}

