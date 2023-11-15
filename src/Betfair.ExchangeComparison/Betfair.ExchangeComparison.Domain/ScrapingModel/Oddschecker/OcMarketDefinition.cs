using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker
{
    public class OcMarketDefinition
    {
        public OcMarketDefinition()
        {
        }

        [JsonProperty("ocMarketId")]
        public long OcMarketId { get; set; }
        [JsonProperty("ocMarketSiblingId")]
        public int OcMarketSiblingId { get; set; }
        [JsonProperty("marketName")]
        public string OcMarketName { get; set; }
        [JsonProperty("marketTypeName")]
        public string OcMarketTypeName { get; set; }
        [JsonProperty("marketFilters")]
        public List<string> OcMarketFilters { get; set; }
    }
}

