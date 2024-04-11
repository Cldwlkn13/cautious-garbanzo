using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class MatchbookEvent
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sport-id")]
        public long SportId { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("in-running-flag")]
        public bool InRunningFlag { get; set; }

        [JsonProperty("allow-live-betting")]
        public bool AllowLiveBetting { get; set; }

        [JsonProperty("category-id")]
        public List<long> CategoryId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("event-participants")]
        public List<EventParticipant> EventParticipants { get; set; }

        [JsonProperty("markets")]
        public List<MatchbookMarket> Markets { get; set; }

        [JsonProperty("meta-tags")]
        public List<MetaTag> MetaTags { get; set; }

        [JsonProperty("race-length")]
        public string RaceLength { get; set; }

        [JsonProperty("race-type")]
        public string RaceType { get; set; }
    }
}
