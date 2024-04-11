using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class ScrapingProcessorFootball : IScrapingProcessor<SportFootball>
    {
        private readonly IScrapingOrchestratorFootball _scrapingOrchestrator;
        private readonly IScrapingControlFootball _scrapingControl;

        public ScrapingProcessorFootball(IScrapingOrchestratorFootball scrapingOrchestrator, IScrapingControlFootball scrapingControl)
        {
            _scrapingOrchestrator = scrapingOrchestrator;
            _scrapingControl = scrapingControl;
        }

        public void ProcessStartStops(BasePageModel baseModel)
        {
            foreach (var prv in _scrapingControl.SwitchBoard.Keys)
            {
                if (prv == baseModel.Provider)
                {
                    _scrapingControl.UpdateExpiry(prv);
                }
                else if (_scrapingControl.SwitchBoard[prv])
                {
                    _scrapingControl.Stop(prv);
                }
            }

            if (baseModel.IsScrapableBookmaker.Contains(baseModel.Bookmaker) && !_scrapingControl.SwitchBoard[baseModel.Provider])
            {
                _scrapingControl.Start(baseModel.Provider);
            }
        }

        public bool TryProcessScrapedEvents(BasePageModel basePageModel, out List<ScrapedEvent> result)
        {
            result = new List<ScrapedEvent>();

            if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
            {
                _scrapingOrchestrator.TryGetScrapedEvents(
                    basePageModel.Provider, DateTime.Today, out var scrapedEvents);

                if (!scrapedEvents.Any())
                {
                    return false;
                }

                result = scrapedEvents;
            }

            return true;
        }
    }
}

