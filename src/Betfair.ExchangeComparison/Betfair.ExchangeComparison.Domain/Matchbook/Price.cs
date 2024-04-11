using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Price
    {
        [JsonProperty("available-amount")]
        public double AvailableAmount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("odds-type")]
        public string OddsType { get; set; }

        [JsonProperty("odds")]
        public double Odds { get; set; }

        [JsonProperty("decimal-odds")]
        public double DecimalOdds { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("exchange-type")]
        public string ExchangeType { get; set; }

        public int Tick { get; set; }
    }
}
