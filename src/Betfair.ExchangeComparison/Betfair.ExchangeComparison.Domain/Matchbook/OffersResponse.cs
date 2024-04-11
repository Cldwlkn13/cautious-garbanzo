using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class OffersResponse
    {
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("exchange-type")]
        public string ExchangeType { get; set; }
        [JsonProperty("odds-type")]
        public string OddsType { get; set; }
        [JsonProperty("offers")]
        public List<Offer> Offers { get; set; }
    }
}
