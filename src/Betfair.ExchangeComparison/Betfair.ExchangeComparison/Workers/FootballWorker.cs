using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Interfaces;

namespace Betfair.ExchangeComparison.Workers
{
    public class FootballWorker : BackgroundService
    {
        private readonly ICatalogService _catalogService;
        private readonly IScrapingOrchestratorFootball _scrapingOrchestrator;
        private readonly IScrapingControlFootball _scrapingControl;

        public FootballWorker(ICatalogService catalogService, IScrapingOrchestratorFootball scrapingOrchestrator, IScrapingControlFootball scrapingControl)
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
                        if (DateTime.UtcNow > _scrapingControl.Expiries[provider])
                        {
                            Console.WriteLine($"Scraping Token for {provider} has expired!");
                            _scrapingControl.Stop(provider);
                            continue;
                        }

                        var catalog = _catalogService.UpdateMarketDetailCatalogGroupByEvent(Sport.Football, 1);

                        if (!catalog.Any())
                        {
                            catalog = _catalogService.UpdateMarketDetailCatalogGroupByEvent(Sport.Football, 2);
                        }

                        if (catalog.Any())
                        {
                            await _scrapingOrchestrator.Orchestrate(catalog, provider);

                            delay = 60000;
                        }
                        else
                        {
                            delay = 60000 * 60;

                            Console.WriteLine($"Football Worker : ExecuteAsync() Waiting {delay / 60 / 1000} " +
                                $"minutes before trying again");
                        }
                    }
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}

