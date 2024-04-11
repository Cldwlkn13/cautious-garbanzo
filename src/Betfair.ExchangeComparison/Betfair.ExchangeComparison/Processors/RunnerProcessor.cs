using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Sportsbook.Model;
using System.Linq;

namespace Betfair.ExchangeComparison.Processors
{
    public class RunnerProcessor : IRunnerProcessor
    {
        private readonly IMappingService _mappingService;
        private readonly ICatalogService _catalogService;
        
        public RunnerProcessor(IMappingService mappingService, ICatalogService catalogService)
		{
			_mappingService = mappingService;
            _catalogService = catalogService;

        }

        public async Task<RunnerPriceOverview?> Process(BasePageModel basePageModel, EventWithCompetition @event, MarketBook mappedWinMarketBook, 
            MarketDetail marketDetail, RunnerDetail sportsbookRunner, bool hasEachWay,  ScrapedEvent? mappedScrapedEvent = null, 
            ScrapedMarket? mappedScrapedMarket = null, MarketBook? mappedPlaceMarketBook = null, MatchbookMarket? mappedMatchbookMarket = null)
        {
            try
            {
                if (sportsbookRunner.runnerStatus != "ACTIVE" ||
                    sportsbookRunner.winRunnerOdds == null)
                {
                    //log problem here
                    return null;
                }

                if (!_mappingService.TryMapRunner(mappedWinMarketBook, sportsbookRunner,
                    out var mappedExchangeWinRunner))
                {
                    //log problem here
                    return null;
                }

                var mappedMarketCatalogue = _catalogService.ExchangeMarketCatalogueStore[DateTime.Today][basePageModel.Sport]
                    .FirstOrDefault(x => x.MarketId == mappedWinMarketBook.MarketId);

                if(mappedMarketCatalogue == null)
                {
                    Console.WriteLine(
                        $"Could not map market Catalogue for " +
                        $"runner {mappedExchangeWinRunner.SelectionId}");

                    return null;
                }

                Runner? mappedExchangePlaceRunner = null;
                if (hasEachWay && mappedPlaceMarketBook != null)
                {
                    if (!_mappingService.TryMapRunner(mappedPlaceMarketBook, sportsbookRunner,
                        out mappedExchangePlaceRunner))
                    {
                        //log problem here
                        return null;
                    }
                }

                var scrapedRunnerIsValid = false;
                ScrapedRunner mappedScrapedRunner = new ScrapedRunner();
                if (mappedScrapedEvent != null && mappedScrapedMarket != null)
                {
                    scrapedRunnerIsValid = TryMapScrapedRunner(
                        basePageModel, mappedScrapedEvent, mappedScrapedMarket, sportsbookRunner,
                        out mappedScrapedRunner);
                }

                _ = TryMapMatchbookRunner(sportsbookRunner, mappedMatchbookMarket, out var mappedMatchbookRunner);   

                if (scrapedRunnerIsValid)
                {
                    return new RunnerPriceOverview(
                        basePageModel.Sport,
                        @event,
                        marketDetail,
                        mappedMarketCatalogue,
                        mappedScrapedMarket,
                        mappedExchangeWinRunner,
                        sportsbookRunner,
                        mappedScrapedRunner,
                        mappedExchangePlaceRunner,
                        basePageModel.Bookmaker,
                        mappedScrapedEvent,
                        mappedMatchbookRunner);
                }
                else if (!basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
                {
                    return new RunnerPriceOverview(
                        basePageModel.Sport,
                        @event,
                        marketDetail,
                        mappedMarketCatalogue,
                        sportsbookRunner,
                        mappedExchangeWinRunner,
                        mappedExchangePlaceRunner,
                        Bookmaker.BetfairSportsbook,
                        mappedMatchbookRunner); 
                }

                return new RunnerPriceOverview();   
            }
            catch (System.Exception exception)
            {
                Console.WriteLine($"RUNNER_PROCESSING_EXCEPTION; " +
                    $"Exception={exception.Message}; " +
                    $"Runner={sportsbookRunner.selectionName}; " +
                    $"Market={marketDetail.marketName} {marketDetail.marketStartTime}; " +
                    $"Event={@event.Event.Name}");

                return null;
            }
        }

        private bool TryMapScrapedRunner(BasePageModel basePageModel, ScrapedEvent mappedScrapedEvent, ScrapedMarket mappedScrapedMarket, 
            RunnerDetail sportsbookRunner, out ScrapedRunner result)
        {
            result = new ScrapedRunner();
            
            try
            {
                if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
                {
                    if (_mappingService.TryMapScrapedRunner(mappedScrapedMarket, sportsbookRunner, out var mappedScrapedRunner))
                    {
                        if (mappedScrapedEvent.ScrapedAt > DateTime.UtcNow.AddSeconds(-90))
                        {
                            result = mappedScrapedRunner;
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (System.Exception exception)
            {
                Console.WriteLine($"SCRAPE_MAPPING_EXCEPTION; " +
                    $"Exception={exception.Message}; " +
                    $"Runner={sportsbookRunner.selectionName}; ");

                return false;
            }
        }

        private bool TryMapMatchbookRunner(RunnerDetail sportsbookRunner, MatchbookMarket? matchbookMarket, out MatchbookRunner result)
        {
            if (matchbookMarket == null)
            {
                result = new MatchbookRunner();
                return false;
            }

            var mappedRunner = matchbookMarket.Runners.FirstOrDefault(r => 
            r.Name.CleanRunnerName().Replace(" ", "").ToLower() == sportsbookRunner.selectionName.Replace(" ", "").ToLower());

            if(mappedRunner ==  null)
            {
                result = new MatchbookRunner();
                return false;
            }

            result = mappedRunner;
            return true;
        }
	}
}

