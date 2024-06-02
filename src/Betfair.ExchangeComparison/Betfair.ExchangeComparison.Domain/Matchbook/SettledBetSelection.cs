using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class SettledBetSelection
    {
        [JsonProperty("runner-id")]
        public object runnerid { get; set; }

        [JsonProperty("runner-name")]
        public string runnername { get; set; }
        public string side { get; set; }
        public double odds { get; set; }
        public double stake { get; set; }

        [JsonProperty("profit-and-loss")]
        public double profitandloss { get; set; }
        public double commission { get; set; }

        [JsonProperty("net-profit-and-loss")]
        public double netprofitandloss { get; set; }
        public List<SettledBet> settledbets { get; set; }
    }
}
