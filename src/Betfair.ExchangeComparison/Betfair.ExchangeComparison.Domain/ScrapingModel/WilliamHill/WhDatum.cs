using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel.WilliamHill
{
    public class WhDatum
    {
        public string marketGroupId { get; set; }
        public string marketGroupType { get; set; }
        public string marketGroupName { get; set; }
        public List<WhEvent> events { get; set; }
        public string sort { get; set; }
    }
}

