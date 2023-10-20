using System;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Interfaces;

namespace Betfair.ExchangeComparison
{
    public class Worker : BackgroundService
    {
        private readonly ICatalogService _catalogService;
        private readonly IScrapingHandler _scrapingHandler;

        public Worker(ICatalogService catalogService, IScrapingHandler scrapingHandler)
        {
            _catalogService = catalogService;
            _scrapingHandler = scrapingHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var catalog = _catalogService.GetCompoundCatalog(Sport.Racing);

            while (!stoppingToken.IsCancellationRequested)
            {
                catalog = _catalogService.UpdateCompoundCatalog(Sport.Racing);

                await _scrapingHandler.Handle(catalog);

                Thread.Sleep(10000);
            }
        }
    }
}

