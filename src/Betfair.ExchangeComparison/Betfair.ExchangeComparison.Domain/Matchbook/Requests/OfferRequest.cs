using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook.Requests
{
    public class OfferRequest
    {
        [JsonProperty("runner-id")]
        public long RunnerId { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }
        [JsonProperty("odds")]
        public float Odds { get; set; }
        [JsonProperty("stake")]
        public float Stake { get; set; }
        [JsonProperty("keep-in-play")]
        public bool KeepInPlay { get; set; }
    }
}
