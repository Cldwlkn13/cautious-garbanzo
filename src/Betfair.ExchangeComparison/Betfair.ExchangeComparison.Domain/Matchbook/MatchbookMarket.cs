using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class MatchbookMarket
    {
        [JsonProperty("live")]
        public bool Live { get; set; }

        [JsonProperty("event-id")]
        public long EventId { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("market-type")]
        public string MarketType { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("in-running-flag")]
        public bool InRunningFlag { get; set; }

        [JsonProperty("allow-live-betting")]
        public bool AllowLiveBetting { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("back-overround")]
        public double BackOverround { get; set; }

        [JsonProperty("lay-overround")]
        public double LayOverround { get; set; }

        [JsonProperty("number-of-winners")]
        public int NumberOfWinners { get; set; }

        [JsonProperty("runners")]
        public List<MatchbookRunner> Runners { get; set; }
    }
}
