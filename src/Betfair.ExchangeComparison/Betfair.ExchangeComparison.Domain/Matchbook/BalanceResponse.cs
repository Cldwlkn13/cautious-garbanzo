using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class BalanceResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("exposure")]
        public double Exposure { get; set; }

        [JsonProperty("commission-reserve")]
        public double CommissionReserve { get; set; }

        [JsonProperty("free-funds")]
        public double FreeFunds { get; set; }
    }
}
