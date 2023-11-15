using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker
{
    public class OcMarket
    {
        public OcMarket()
        {
        }

        [JsonProperty("marketId")]
        public long MarketId { get; set; }
        [JsonProperty("marketName")]
        public string MarketName { get; set; }
        [JsonProperty("subeventId")]
        public int SubeventId { get; set; }
        [JsonProperty("subeventName")]
        public string SubeventName { get; set; }
        [JsonProperty("subeventType")]
        public string SubeventType { get; set; }
        [JsonProperty("subeventStartTime")]
        public DateTime SubeventStartTime { get; set; }
        [JsonProperty("subeventEndTime")]
        public DateTime SubeventEndTime { get; set; }
        [JsonProperty("eventId")]
        public int SventId { get; set; }
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
        [JsonProperty("betTypeId")]
        public int BetTypeId { get; set; }
        [JsonProperty("marketTypeName")]
        public string MarketTypeName { get; set; }
        [JsonProperty("marketGroup")]
        public string MarketGroup { get; set; }
        [JsonProperty("priority")]
        public int Priority { get; set; }
        [JsonProperty("odds")]
        public List<OcOdds> Odds { get; set; }
        [JsonProperty("bets")]
        public List<OcBet> Bets { get; set; }
        [JsonProperty("eachWayTerms")]
        public List<object> EachWayTerms { get; set; }
    }
}

