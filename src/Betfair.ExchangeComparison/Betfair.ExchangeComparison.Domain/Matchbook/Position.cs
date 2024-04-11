using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Position
    {
        [JsonProperty("event-id")]
        public long EventId { get; set; }
        [JsonProperty("market-id")]
        public long MarketId { get; set; }
        [JsonProperty("runner-id")]
        public long RunnerId { get; set; }
        [JsonProperty("potential-loss")]
        public double PotentialLoss { get; set; }
    }
}
