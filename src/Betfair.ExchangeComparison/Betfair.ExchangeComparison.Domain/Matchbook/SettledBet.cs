using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class SettledBet
    {
        public long id { get; set; }

        [JsonProperty("offer-id")]
        public long offerid { get; set; }
        public double odds { get; set; }
        public double stake { get; set; }
        public bool adjusted { get; set; }
        public bool originator { get; set; }

        [JsonProperty("in-play")]
        public bool inplay { get; set; }

        [JsonProperty("submitted-time")]
        public DateTime submittedtime { get; set; }

        [JsonProperty("matched-time")]
        public DateTime matchedtime { get; set; }

        [JsonProperty("settled-time")]
        public DateTime settledtime { get; set; }
        public string result { get; set; }

        [JsonProperty("profit-and-loss")]
        public double profitandloss { get; set; }

        [JsonProperty("commission-type")]
        public string commissiontype { get; set; }
        public double commission { get; set; }

        [JsonProperty("net-profit-and-loss")]
        public double netprofitandloss { get; set; }

        [JsonProperty("offer-reference-id")]
        public string offerreferenceid { get; set; }

        [JsonProperty("use-bonus")]
        public bool usebonus { get; set; }
        public List<SettledBetAdjustment> adjustments { get; set; }
    }
}
