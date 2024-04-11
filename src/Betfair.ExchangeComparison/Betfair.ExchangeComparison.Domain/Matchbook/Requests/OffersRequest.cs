using Newtonsoft.Json;
using System.Text.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook.Requests
{
    public class OffersRequest
    {
        [JsonProperty("odds-type")]
        public string OddsType { get; set; } = "DECIMAL";
        [JsonProperty("exchange-type")]
        public string ExchangeType { get; set; } = "back-lay";
        [JsonProperty("offers")]
        public List<OfferRequest> Offers { get; set; }
    }
}
