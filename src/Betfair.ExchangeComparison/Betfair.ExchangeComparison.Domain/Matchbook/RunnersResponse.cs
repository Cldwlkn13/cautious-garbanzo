using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class RunnersResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("runners")]
        public List<MatchbookRunner> Runners { get; set; }
    }
}
