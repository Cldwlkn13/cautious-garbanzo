﻿using Betfair.ExchangeComparison.Domain.ScrapingModel;

namespace Betfair.ExchangeComparison.Pages.Models
{
    public class CatalogViewModel
    {
        public List<MarketViewModel> Markets { get; set; }
        public List<BestRunner> BestWinRunners { get; set; }
        public List<BestRunner> BestEachWayRunners { get; set; }
        public string? Message { get; set; }
        public UsageModel UsageModel { get; set; }

        public CatalogViewModel()
        {
            Markets = new List<MarketViewModel>();
            BestWinRunners = new List<BestRunner>();
            BestEachWayRunners = new List<BestRunner>();
            UsageModel = new UsageModel();
        }
    }
}

