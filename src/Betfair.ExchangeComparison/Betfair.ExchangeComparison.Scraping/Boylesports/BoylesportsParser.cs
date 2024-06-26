﻿using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Extensions;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.Boylesports
{
    public class BoylesportsParser : IBoylesportsParser
    {
        public BoylesportsParser()
        {
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEwc compoundObj)
        {
            try
            {
                var document = html!.LoadDocument();

                var marketNode = GetOuterMarketNode(document);
                var marketNodeDocument = marketNode.InnerHtml.LoadDocument();

                var eachWayString = marketNodeDocument.GetInnerText("span", "id", "EW");
                var eachWayTerms = ExtractEachWayTerms(eachWayString);

                var innerMarketNode = GetInnerMarketNode(marketNodeDocument);
                var innerMarketNodeDocument = innerMarketNode.InnerHtml.LoadDocument();

                var oddsNodes = innerMarketNodeDocument.SelectManyNodesContains("a", "class", "odds");

                var scrapedRunners = new List<ScrapedRunner>();
                foreach (var oddsNode in oddsNodes)
                {
                    var name = oddsNode.Attributes["data-name"].Value.Replace("&#39;", "'");
                    var price = oddsNode.Attributes["data-price"].Value;
                    var isNr = oddsNode.Attributes["data-isnr"].Value;

                    if (name.Contains("1st") || name.Contains("2nd") || isNr == "True")
                    {
                        continue;
                    }
                    var scrapedRunner = new ScrapedRunner(name, new List<ScrapedPrice>
                    {
                        new ScrapedPrice(price, Bookmaker.Boylesports)
                    });
                    scrapedRunners.Add(scrapedRunner);
                }

                var scrapedMarket = new ScrapedMarket();
                scrapedMarket.Name = compoundObj.SportsbookMarket.marketName;
                scrapedMarket.ScrapedEachWayTerms = eachWayTerms;
                scrapedMarket.ScrapedRunners = scrapedRunners;

                var scrapedEvent = new ScrapedEvent();
                //scrapedEvent.MappedEventWithMarketDetail = compoundObj;
                scrapedEvent.BetfairName = compoundObj.EventWithCompetition.Event.Venue;
                scrapedEvent.StartTime = compoundObj.SportsbookMarket.marketStartTime
                    .ConvertUtcToBritishIrishLocalTime();
                scrapedEvent.ScrapedMarkets.Add(scrapedMarket);

                return scrapedEvent;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception={exception} on BuildScrapedEvent; {compoundObj.EventWithCompetition.Event.Name} " +
                    $"{compoundObj.SportsbookMarket.marketName} {compoundObj.SportsbookMarket.marketStartTime}");

                return new ScrapedEvent();
            }
        }

        private static HtmlNode GetOuterMarketNode(HtmlDocument document)
        {
            return document.DocumentNode.SelectSingleNode("//div[@id='RacingMarketSelections']");
        }

        private static HtmlNode GetInnerMarketNode(HtmlDocument document)
        {
            return document.DocumentNode.SelectSingleNode("//div[@class='rs-reorder-section']");
        }

        // (EW 1/5 1,2,3)
        private static List<ScrapedEachWayTerms> ExtractEachWayTerms(string? eachWayString)
        {
            var scrapedEachWayTerms = new ScrapedEachWayTerms();

            if (string.IsNullOrEmpty(eachWayString) ||
                !eachWayString.Contains("/") ||
                !eachWayString.Contains(",")) return new List<ScrapedEachWayTerms>();

            var splitAtFraction = eachWayString.Split("/");
            int.TryParse(splitAtFraction[1].Substring(0, 1), out var fraction);

            var splitPlaces = splitAtFraction[1].Split(",");
            var places = splitPlaces.Length;

            scrapedEachWayTerms.EachWayFraction = fraction;
            scrapedEachWayTerms.NumberOfPlaces = places;
            scrapedEachWayTerms.Bookmaker = Domain.Enums.Bookmaker.Boylesports;

            return new List<ScrapedEachWayTerms>() { scrapedEachWayTerms };
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            throw new NotImplementedException();
        }
    }
}

