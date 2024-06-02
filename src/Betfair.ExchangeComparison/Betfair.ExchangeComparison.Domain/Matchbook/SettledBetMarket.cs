using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class SettledBetMarket
    {
        public object id { get; set; }
        public string name { get; set; }

        [JsonProperty("event-id")]
        public object eventid { get; set; }

        [JsonProperty("event-name")]
        public string eventname { get; set; }

        [JsonProperty("sport-id")]
        public object sportid { get; set; }

        [JsonProperty("start-time")]
        public DateTime starttime { get; set; }

        [JsonProperty("settled-time")]
        public DateTime settledtime { get; set; }
        public double stake { get; set; }

        [JsonProperty("profit-and-loss")]
        public double profitandloss { get; set; }

        [JsonProperty("commission-type")]
        public string commissiontype { get; set; }
        public double commission { get; set; }

        [JsonProperty("net-profit-and-loss")]
        public double netprofitandloss { get; set; }
        public string product { get; set; }
        public List<SettledBetSelection> selections { get; set; }
    }
}
