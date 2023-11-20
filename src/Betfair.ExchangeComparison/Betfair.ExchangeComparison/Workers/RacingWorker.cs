using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Interfaces;

namespace Betfair.ExchangeComparison.Workers
{
    public class RacingWorker : BackgroundService
    {
        private readonly ICatalogService _catalogService;
        private readonly IScrapingOrchestratorRacing _scrapingOrchestrator;
        private readonly IScrapingControlRacing _scrapingControl;

        public RacingWorker(ICatalogService catalogService, IScrapingOrchestratorRacing scrapingOrchestrator, IScrapingControlRacing scrapingControl)
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

                            Console.WriteLine($"Racing Worker : ExecuteAsync() Waiting {delay / 60 / 1000} " +
                                $"minutes before trying again");
                        }
                    }
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}

