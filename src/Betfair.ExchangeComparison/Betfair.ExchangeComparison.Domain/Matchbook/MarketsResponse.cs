using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class MarketsResponse
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("per-page")]
        public int PerPage { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("markets")]
        public List<MatchbookMarket> Markets { get; set; }
    }
}
