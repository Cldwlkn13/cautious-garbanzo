using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Country
    {
        [JsonProperty("country-id")]
        public int CountryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country-code")]
        public string CountryCode { get; set; }
    }
}
