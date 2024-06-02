using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class MarketProcessor : IMarketProcessor
	{
        private readonly IMappingService _mappingService;
        private readonly IRunnerProcessor _runnerProcessor;
        private readonly IPricingComparisonHandler _pricingComparisonHandler;

        private readonly List<string> Exceptions;

        public MarketProcessor(IMappingService mappingService, IRunnerProcessor runnerProcessor, IPricingComparisonHandler pricingComparisonHandler)
        {
            _mappingService = mappingService;
            _runnerProcessor = runnerProcessor;
            _pricingComparisonHandler = pricingComparisonHandler;

            Exceptions = new List<string>();
        }

        public async Task<MarketViewModel?> Process(BasePageModel basePageModel, EventWithCompetition ewc, MarketDetail marketDetail, 
            EventWithMarketBooks eventWithMarketBooks, bool hasEachWay, ScrapedEvent? mappedScrapedEvent = null, 
            MatchbookEvent? mappedMatchbookEvent = null)
        {
            try
            {
                if (!_mappingService.TryMapMarketsBooksToSportsbookMarketDetailObj(
                    eventWithMarketBooks, marketDetail, out MarketBooksByDateTime eventMarketBooks))
                {
                    //log problem here
                    return null;
                }

                var winOverround = marketDetail.WinOverround();
                var placeOverround = hasEachWay ? marketDetail.PlaceOverround() : 0;

                ScrapedMarket mappedScrapedMarket = mappedScrapedEvent != null ? 
                    MapScrapedMarket(basePageModel, mappedScrapedEvent, marketDetail, ewc) : 
                    new ScrapedMarket();

                MatchbookMarket mappedMatchbookWinMarket = mappedMatchbookEvent != null ?
                    MapMatchbookWinMarket(mappedMatchbookEvent) :
                    new MatchbookMarket();

                int numberOfPlaces = 0;
                int eachWayFraction = 0;
                if (hasEachWay)
                {
                    var terms = MapEachWayTerms(basePageModel, mappedScrapedMarket, marketDetail);
                    numberOfPlaces = terms.NumberOfPlaces;
                    eachWayFraction = terms.EachWayFraction;
                }

                if (!_mappingService.TryMapMarketBook(eventMarketBooks, marketDetail,
                    out MarketBook mappedWinMarketBook))
                {
                    //log problem here
                    return null;
                }

                MarketBook? mappedPlaceMarketBook = null;
                if (hasEachWay && numberOfPlaces > 0)
                {
                    if (!_mappingService.TryMapMarketBook(eventMarketBooks, numberOfPlaces, 
                        out mappedPlaceMarketBook))
                    {
                        //log problem here
                        return null;
                    }
                }

                var bestRunners = new List<BestRunner>();
                var bestEachWayRunners = new List<BestRunner>();
                var runnerPriceOverviews = new List<RunnerPriceOverview>(); 
                var runners = new List<RunnerViewModel>();

                foreach (var sportsbookRunner in marketDetail.runnerDetails)
                {
                    var runnerPriceOverview = await _runnerProcessor.Process(
                        basePageModel, ewc, mappedWinMarketBook, marketDetail, sportsbookRunner, hasEachWay,
                        mappedScrapedEvent, mappedScrapedMarket, mappedPlaceMarketBook, mappedMatchbookWinMarket);

                    if (runnerPriceOverview == null || runnerPriceOverview.Bookmaker == Bookmaker.Unknown) continue;

                    runnerPriceOverviews.Add(runnerPriceOverview);

                    runners.Add(new RunnerViewModel(runnerPriceOverview));

                    if (_pricingComparisonHandler.AssessIsBestRunnerWinOnly(runnerPriceOverview, out var bestRunner))
                    {
                        bestRunners.Add(bestRunner);
                    }

                    if (_pricingComparisonHandler.AssessIsBestRunnerEachWay(runnerPriceOverview, out var bestEachWayRunner))
                    {
                        bestEachWayRunners.Add(bestEachWayRunner);
                    }
                }

                var mvm = new MarketViewModel(ewc.Event);
                mvm.SportsbookMarket = marketDetail;
                mvm.Runners = runners.Any() ? runners.OrderBy(r => r.SportsbookRunner.winRunnerOdds.@decimal) : runners;
                mvm.WinOverround = winOverround;
                mvm.EachWayPlaceOverround = placeOverround;
                mvm.RunnerPriceOverviews = runnerPriceOverviews;
                mvm.BestRunners = bestRunners;
                mvm.BestEachWayRunners = bestEachWayRunners;
                mvm.TimeToStart = marketDetail.marketStartTime.TimeToStart();
                mvm.MappedMatchbookEvent = mappedMatchbookEvent;

                return mvm;
            }
            catch (System.Exception exception)
            {
                var exceptionLog = $"MARKET_PROCESSOR_EXCEPTION; " +
                   $"Exception={exception.Message}; " +
                   $"Market={marketDetail.marketName} {marketDetail.marketStartTime}; " +
                   $"Event={ewc.Event.Name}";

                if (!Exceptions.Contains(exceptionLog))
                {
                    Console.WriteLine(exceptionLog);
                    Exceptions.Add(exceptionLog);
                }

                return null;
            }
        }

        private ScrapedMarket MapScrapedMarket(BasePageModel basePageModel, ScrapedEvent mappedScrapedEvent, MarketDetail marketDetail, EventWithCompetition ewc)
        {
            var mappedScrapedMarket = new ScrapedMarket();
            if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
            {
                if (!_mappingService.TryMapScrapedMarket(mappedScrapedEvent, marketDetail, out mappedScrapedMarket))
                {
                    Console.WriteLine($"SCRAPED_MARKET_MAPPING_FAIL; " +
                        $"Event={ewc.Event.Name}; " +
                        $"Market={marketDetail.marketName} {marketDetail.marketStartTime}");
                }
            }
            return mappedScrapedMarket;
        }

        private static ScrapedEachWayTerms MapEachWayTerms(BasePageModel basePageModel, ScrapedMarket mappedScrapedMarket, MarketDetail marketDetail)
        {
            if (basePageModel.IsScrapableBookmaker.Contains(basePageModel.Bookmaker))
            {
                if (mappedScrapedMarket.ScrapedEachWayTerms != null &&
                    mappedScrapedMarket.ScrapedEachWayTerms.TryGetScrapedEachWayTermsByBookmaker(
                        basePageModel.Bookmaker, out var scrapedEachWayTerms))
                {
                    return scrapedEachWayTerms;
                }
            }

            return new ScrapedEachWayTerms(
                marketDetail.numberOfPlaces, 
                marketDetail.placeFractionDenominator, 
                basePageModel.Bookmaker);
        }

        private static MatchbookMarket MapMatchbookWinMarket(MatchbookEvent matchbookEvent)
        {
            return matchbookEvent.Markets?.FirstOrDefault(m => m.Name == "WIN") ??
                new MatchbookMarket()
                {
                    Runners = new List<MatchbookRunner>()
                };
        }
    }
}

