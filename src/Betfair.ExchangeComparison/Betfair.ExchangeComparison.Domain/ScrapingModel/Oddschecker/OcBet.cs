using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker
{
    public class OcBet
    {
        public OcBet()
        {
        }

        [JsonProperty("marketId")]
        public object MarketId { get; set; }
        [JsonProperty("betId")]
        public object BetId { get; set; }
        [JsonProperty("betName")]
        public string BetName { get; set; }
        [JsonProperty("bestOddsBookmakerCodes")]
        public List<string> BestOddsBookmakerCodes { get; set; }
        [JsonProperty("bestOddsDecimal")]
        public double BestOddsDecimal { get; set; }
        [JsonProperty("bestOddsFractional")]
        public string BestOddsFractional { get; set; }
        [JsonProperty("line")]
        public string Line { get; set; }
    }
}

