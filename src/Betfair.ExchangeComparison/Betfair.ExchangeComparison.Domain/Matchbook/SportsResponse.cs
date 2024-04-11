using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class SportsResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("per-page")]
        public int PerPage { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("sports")]
        public List<MatchbookSport> Sports { get; set; }
    }
}
