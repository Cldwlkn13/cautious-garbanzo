using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class MatchedBetInOffer
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("offer-id")]
        public long OfferId { get; set; }
        [JsonProperty("odds")]
        public double Odds { get; set; }
        [JsonProperty("odds-type")]
        public string OddsType { get; set; }
        [JsonProperty("decimal-odds")]
        public double DecimalOdds { get; set; }
        [JsonProperty("stake")]
        public double Stake { get; set; }
        [JsonProperty("potential-profit")]
        public double PotentialProfit { get; set; }
        [JsonProperty("commission")]
        public double Commision { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("created-at")]
        public string CreatedAt { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("in-play")]
        public bool InPlay { get; set; }
    }
}
