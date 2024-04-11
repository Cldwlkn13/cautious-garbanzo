using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class OfferEdit
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("offer-id")]
        public long OfferId { get; set; }
        [JsonProperty("runner-id")]
        public long RunnerId { get; set; }
        [JsonProperty("odds-type")]
        public string OddsType { get; set; }
        [JsonProperty("odds-before")]
        public double OddsBefore { get; set; }
        [JsonProperty("decimal-odds-before")]
        public double DecimalOddsBefore { get; set; }
        [JsonProperty("odds-after")]
        public double OddsAfter { get; set; }
        [JsonProperty("decimal-odds-after")]
        public double DecimalOddsAfter { get; set; }
        [JsonProperty("stake-before")]
        public double StakeBefore { get; set; }
        [JsonProperty("stake-after")]
        public double StakeAfter { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("edit-time")]
        public bool EditTime { get; set; }
    }
}
