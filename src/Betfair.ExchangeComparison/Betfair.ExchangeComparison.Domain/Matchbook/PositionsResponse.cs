using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class PositionsResponse
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("per-page")]
        public int PerPage { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("positions")]
        public List<Position> Positions { get; set; }
    }
}
