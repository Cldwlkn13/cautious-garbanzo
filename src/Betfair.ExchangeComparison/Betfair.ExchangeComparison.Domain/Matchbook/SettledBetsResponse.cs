using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class SettledBetsResponse
    {
        public int offset { get; set; }

        [JsonProperty("per-page")]
        public int perpage { get; set; }
        public int total { get; set; }
        public string language { get; set; }
        public string currency { get; set; }

        [JsonProperty("odds-type")]
        public string oddstype { get; set; }

        [JsonProperty("profit-and-loss")]
        public double profitandloss { get; set; }
        public double commission { get; set; }

        [JsonProperty("net-profit-and-loss")]
        public double netprofitandloss { get; set; }
        public List<SettledBetMarket> markets { get; set; }
    }
}
