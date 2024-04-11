using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Address
    {
        [JsonProperty("address-id")]
        public int AddressId { get; set; }

        [JsonProperty("address-line-1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("address-line-2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("region-name")]
        public string RegionName { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("post-code")]
        public string PostCode { get; set; }
    }
}
