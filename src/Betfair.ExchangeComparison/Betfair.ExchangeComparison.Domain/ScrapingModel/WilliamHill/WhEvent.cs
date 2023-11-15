namespace Betfair.ExchangeComparison.Domain.ScrapingModel.WilliamHill
{
    public class WhEvent
    {
        public bool active { get; set; }
        public bool displayed { get; set; }
        public string id { get; set; }
        public string eventId { get; set; }
        public string eventName { get; set; }
        public string eventPathname { get; set; }
        public DateTime startDateTime { get; set; }
        public object offTime { get; set; }
        public string status { get; set; }
        public bool streamingAvailable { get; set; }
        public bool isInPlay { get; set; }
        public bool byoMarketsEnabled { get; set; }
        public bool byoFreedomEnabled { get; set; }
        public int activeMarketsCount { get; set; }
        public List<WhMarket> markets { get; set; }
        public string tvChannelName { get; set; }

        public override string ToString()
        {
            return $"{eventName} - {startDateTime}";
        }
    }
}

