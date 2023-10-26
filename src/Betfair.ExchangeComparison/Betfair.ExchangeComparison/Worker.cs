using System;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;

namespace Betfair.ExchangeComparison
{
    public class Worker : BackgroundService
    {
        private readonly ICatalogService _catalogService;
        private readonly IScrapingOrchestrator _scrapingOrchestrator;
        private readonly IScrapingControl _scrapingControl;

        public Worker(ICatalogService catalogService, IScrapingOrchestrator scrapingOrchestrator, IScrapingControl scrapingControl)
        {
            _catalogService = catalogService;
            _scrapingOrchestrator = scrapingOrchestrator;
            _scrapingControl = scrapingControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = 1000;

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var provider in _scrapingControl.SwitchBoard.Keys)
                {
                    if (_scrapingControl.SwitchBoard[provider])
                    {
                        var catalog = _catalogService.UpdateMarketDetailCatalog(Sport.Racing, 1);

                        if (!catalog.Any())
                        {
                            catalog = _catalogService.UpdateMarketDetailCatalog(Sport.Racing, 2);
                        }

                        if (catalog.Any())
                        {
                            await _scrapingOrchestrator.Orchestrate(catalog, provider);

                            delay = 10000;
                        }
                        else
                        {
                            delay = 60000 * 60;

                            Console.WriteLine($"Worker : ExecuteAsync() Waiting {delay / 60 / 1000} " +
                                $"minutes before trying again");
                        }
                    }
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}

