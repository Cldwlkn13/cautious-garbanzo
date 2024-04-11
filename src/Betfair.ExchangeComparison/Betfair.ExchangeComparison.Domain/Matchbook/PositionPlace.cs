using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class PositionPlace
    {
        [JsonProperty("event-id")]
        public long EventId { get; set; }
        [JsonProperty("market-id")]
        public long MarketId { get; set; }
        [JsonProperty("runner-id")]
        public long RunnerId { get; set; }
        [JsonProperty("pnlOnWin")]
        public double WinPnl { get; set; }
        [JsonProperty("pnlOnLose")]
        public double LosePnl { get; set; }
    }
}
