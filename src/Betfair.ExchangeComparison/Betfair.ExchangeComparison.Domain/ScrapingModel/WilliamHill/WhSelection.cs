namespace Betfair.ExchangeComparison.Domain.ScrapingModel.WilliamHill
{
    public class WhSelection
    {
        public bool active { get; set; }
        public bool displayed { get; set; }
        public int currentPriceNum { get; set; }
        public int currentPriceDen { get; set; }
        public string fbResult { get; set; }
        public int order { get; set; }
        public string id { get; set; }
        public string selectionId { get; set; }
        public string selectionName { get; set; }
        public bool settled { get; set; }
        public string status { get; set; }

        public override string ToString()
        {
            return $"{selectionName} {currentPriceNum}/{currentPriceDen}";
        }
    }
}

