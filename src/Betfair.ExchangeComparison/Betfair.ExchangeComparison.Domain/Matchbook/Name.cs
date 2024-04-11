using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Name
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("title-id")]
        public string TitleId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
