using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Offer
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("event-id")]
        public long EventId { get; set; }
        [JsonProperty("event-name")]
        public string EventName { get; set; }
        [JsonProperty("market-id")]
        public long MarketId { get; set; }
        [JsonProperty("market-name")]
        public string MarketName { get; set; }
        [JsonProperty("market-type")]
        public string MarketType { get; set; }
        [JsonProperty("product")]
        public string Product { get; set; }
        [JsonProperty("runner-id")]
        public long RunnerId { get; set; }
        [JsonProperty("runner-name")]
        public string RunnerName { get; set; }
        [JsonProperty("sport-id")]
        public long SportId { get; set; }
        [JsonProperty("exchange-type")]
        public string ExchangeType { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }
        [JsonProperty("odds")]
        public double Odds { get; set; }
        [JsonProperty("odds-type")]
        public string OddsType { get; set; }
        [JsonProperty("decimal-odds")]
        public double DecimalOdds { get; set; }
        [JsonProperty("stake")]
        public double Stake { get; set; }
        [JsonProperty("remaining")]
        public double Remaining { get; set; }
        [JsonProperty("potential-profit")]
        public double PotentialProfit { get; set; }
        [JsonProperty("remaining-potential-profit")]
        public double RemainingPotentialProfit { get; set; }
        [JsonProperty("commission-type")]
        public string CommisionType { get; set; }
        [JsonProperty("originator-commission-type")]
        public double OriginatorCommissionRate { get; set; }
        [JsonProperty("originator-commission-rate")]
        public double AcceptorCommissionRate { get; set; }
        [JsonProperty("commission-reserve")]
        public double CommisionReserve { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("created-at")]
        public string CreatedAt { get; set; }
        [JsonProperty("last-modified-time")]
        public DateTime LastModifiedTime { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("in-play")]
        public bool InPlay { get; set; }
        [JsonProperty("keep-in-play")]
        public bool KeepInPlay { get; set; }
        [JsonProperty("use-bonus")]
        public bool UseBonus { get; set; }
        [JsonProperty("matched-bets")]
        public List<MatchedBetInOffer>? MatchedBets { get; set; }
        [JsonProperty("offer-edits")]
        public List<OfferEdit>? OfferEdits { get; set; }
    }
}
