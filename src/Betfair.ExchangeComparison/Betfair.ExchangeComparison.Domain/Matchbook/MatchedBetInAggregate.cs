using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class MatchedBetInAggregate
    {
        [JsonProperty("odds")]
        public double Odds { get; set; }
        [JsonProperty("odds-type")]
        public string OddsType { get; set; }
        [JsonProperty("decimal-odds")]
        public double DecimalOdds { get; set; }
        [JsonProperty("stake")]
        public double Stake { get; set; }
        [JsonProperty("potential-liability")]
        public double PotentialLiability { get; set; }
        [JsonProperty("event-id")]
        public long EventId { get; set; }
        [JsonProperty("event-name")]
        public string EventName { get; set; }
        [JsonProperty("market-id")]
        public long MarketId { get; set; }
        [JsonProperty("market-name")]
        public string MarketName { get; set; }
        [JsonProperty("runner-id")]
        public long RunnerId { get; set; }
        [JsonProperty("runner-name")]
        public string RunnerName { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }
    }
}
