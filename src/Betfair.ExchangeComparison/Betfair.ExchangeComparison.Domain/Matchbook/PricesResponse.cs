using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class PricesResponse
    {
        [JsonProperty("prices")]
        public List<Price> Prices { get; set; }
    }
}
