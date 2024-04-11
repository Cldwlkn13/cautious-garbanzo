using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class AggregatedMatchedBets
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("exchange-type")]
        public string ExchangeType { get; set; }
        [JsonProperty("odds-type")]
        public string OddsType { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("matched-bets")]
        public List<MatchedBetInAggregate> MatchedBets { get; set; }
    }
}
