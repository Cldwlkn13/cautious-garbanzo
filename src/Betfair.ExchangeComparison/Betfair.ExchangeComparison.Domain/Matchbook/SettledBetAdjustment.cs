using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class SettledBetAdjustment
    {
        public object id { get; set; }
        public string description { get; set; }
        public double amount { get; set; }
        public DateTime time { get; set; }

        [JsonProperty("odds-before")]
        public double oddsbefore { get; set; }

        [JsonProperty("odds-after")]
        public double oddsafter { get; set; }

        [JsonProperty("stake-before")]
        public double stakebefore { get; set; }

        [JsonProperty("stake-after")]
        public double stakeafter { get; set; }

        [JsonProperty("deducted-runner-id")]
        public object deductedrunnerid { get; set; }
    }
}
