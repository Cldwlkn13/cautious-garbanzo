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
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var bm in _scrapingControl.SwitchBoard.Keys)
                {
                    if (_scrapingControl.SwitchBoard[bm])
                    {
                        var catalog = _catalogService.UpdateCatalog(Sport.Racing);

                        if (catalog.Any())
                        {
                            await _scrapingOrchestrator.Orchestrate(catalog, bm);
                        }
                        else
                        {
                            var wait = 60000 * 60;

                            Console.WriteLine($"Worker : ExecuteAsync() Waiting {wait / 60 / 1000} " +
                                $"minutes before trying again");

                            await Task.Delay(wait, stoppingToken);
                        }
                    }
                }

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}

