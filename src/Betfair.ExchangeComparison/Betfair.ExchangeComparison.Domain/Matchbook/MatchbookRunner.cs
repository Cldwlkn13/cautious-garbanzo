using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class MatchbookRunner
    {
        [JsonProperty("withdrawn")]
        public bool Withdrawn { get; set; }

        [JsonProperty("prices")]
        public List<Price> Prices { get; set; }

        [JsonProperty("last-price-update-time")]
        public DateTime LastPriceUpdateTime { get; set; }

        [JsonProperty("event-id")]
        public long EventId { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("market-id")]
        public long MarketId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("event-participant-id")]
        public long EventParticipantId { get; set; }
    }
}
