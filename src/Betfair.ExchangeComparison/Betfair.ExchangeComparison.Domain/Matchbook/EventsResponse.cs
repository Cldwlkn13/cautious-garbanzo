using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class EventsResponse
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("per-page")]
        public int PerPage { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("events")]
        public List<MatchbookEvent> Events { get; set; }
    }
}
