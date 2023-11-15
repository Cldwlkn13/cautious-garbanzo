using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker
{
    public class OcOdds
    {
        public OcOdds()
        {
        }

        [JsonProperty("betId")]
        public object BetId { get; set; }
        [JsonProperty("bookmakerCode")]
        public string BookmakerCode { get; set; }
        [JsonProperty("oddsDecimal")]
        public double OddsDecimal { get; set; }
        [JsonProperty("oddsFractional")]
        public string OddsFractional { get; set; }
        [JsonProperty("bookmakerSelectionId")]
        public string BookmakerSelectionId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("movement")]
        public string Movement { get; set; }
        [JsonProperty("betFeedTimestamp")]
        public object BetFeedTimestamp { get; set; }
        [JsonProperty("inOut")]
        public string InOut { get; set; }
        [JsonProperty("inOutChange")]
        public string InOutChange { get; set; }
    }
}

