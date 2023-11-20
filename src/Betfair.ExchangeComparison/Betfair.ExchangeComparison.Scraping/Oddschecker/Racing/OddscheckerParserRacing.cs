using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Extensions;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using HtmlAgilityPack;
using Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Racing
{
    public class OddscheckerParserRacing : IOddscheckerParserRacing
    {
        public OddscheckerParserRacing()
        {
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEwc marketDetailsWithEwc)
        {
            try
            {
                var document = html!.LoadDocument();

                var oddsTableNode = GetOddsTableNode(document);

                if (oddsTableNode == null) return new ScrapedEvent();

                var tblFoot = oddsTableNode.SelectSingleNode("//tr[@id='etfEW']");

                var eachWayTerms = new List<ScrapedEachWayTerms>();

                if (tblFoot != null)
                {
                    var ewData = tblFoot.SelectNodes("td");

                    foreach (var data in ewData)
                    {
                        if (data.HasAttributes && data.Attributes.Contains("data-bk"))
                        {
                            var bk = data.Attributes["data-bk"].Value;
                            var places = 0;
                            var eachWayFraction = 0;
                            if (data.Attributes.Contains("data-ew-div"))
                            {
                                var ewFraction = data.Attributes["data-ew-div"].Value;
                                var ewFractionSplit = ewFraction.Contains("/") ?
                                    ewFraction.Split("/") :
                                    new string[2] { "1", "1" };

                                int.TryParse(ewFractionSplit[1], out eachWayFraction);
                            }

                            if (data.Attributes.Contains("data-ew-places"))
                            {
                                var ewPlaces = data.Attributes["data-ew-places"].Value;
                                int.TryParse(ewPlaces, out places);
                            }

                            if (bk.TryMapBookmaker(out var bookmaker))
                            {
                                eachWayTerms.Add(new ScrapedEachWayTerms(places, eachWayFraction, bookmaker));
                            }
                        }
                    }
                }

                var rows = oddsTableNode.SelectManyNodesContains("tr", "class", "diff-row evTabRow bc");

                var scrapedRunners = new List<ScrapedRunner>();

                foreach (var row in rows)
                {
                    if (!row.HasAttributes)
                    {
                        continue;
                    }

                    if (!row.Attributes.Contains("data-bname"))
                    {
                        continue;
                    }

                    var name = row.Attributes["data-bname"].Value;

                    var dataRow = row.SelectNodes("td");

                    var prices = new List<ScrapedPrice>();

                    foreach (var data in dataRow)
                    {
                        if (!data.HasAttributes)
                        {
                            continue;
                        }

                        if (!data.Attributes.Contains("data-bk"))
                        {
                            continue;
                        }

                        var bk = data.Attributes["data-bk"].Value;

                        if (!bk.TryMapBookmaker(out var bookmaker))
                        {
                            continue;
                        }

                        if (!data.Attributes.Contains("data-odig"))
                        {
                            continue;
                        }

                        if (!double.TryParse(data.Attributes["data-odig"].Value, out var price))
                        {
                            continue;
                        }

                        prices.Add(new ScrapedPrice(price, bookmaker));
                    }

                    var mappedRunnerDetail = marketDetailsWithEwc.SportsbookMarket.runnerDetails
                        .FirstOrDefault(r => r.selectionName.ToLower() == name.ToLower());

                    var scrapedRunner = new ScrapedRunner(name, prices);
                    scrapedRunner.MappedRunnerDetail = mappedRunnerDetail != null ? mappedRunnerDetail : new RunnerDetail();
                    scrapedRunners.Add(scrapedRunner);
                }

                var scrapedMarket = new ScrapedMarket();
                scrapedMarket.Name = marketDetailsWithEwc.SportsbookMarket.marketName;
                scrapedMarket.ScrapedEachWayTerms = eachWayTerms;
                scrapedMarket.ScrapedRunners = scrapedRunners;
                scrapedMarket.MappedMarketDetail = marketDetailsWithEwc.SportsbookMarket;

                var scrapedEvent = new ScrapedEvent();
                scrapedEvent.MappedEventWithCompetition = marketDetailsWithEwc.EventWithCompetition;
                scrapedEvent.ScrapedEventName = marketDetailsWithEwc.EventWithCompetition.Event.Name;
                scrapedEvent.BetfairName = marketDetailsWithEwc.EventWithCompetition.Event.Name;
                scrapedEvent.StartTime = marketDetailsWithEwc.SportsbookMarket.marketStartTime
                    .ConvertUtcToBritishIrishLocalTime();
                scrapedEvent.ScrapedMarkets.Add(scrapedMarket);

                return scrapedEvent;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception={exception} on BuildScrapedEvent; {marketDetailsWithEwc.EventWithCompetition.Event.Name} " +
                    $"{marketDetailsWithEwc.SportsbookMarket.marketName} {marketDetailsWithEwc.SportsbookMarket.marketStartTime}");

                return new ScrapedEvent();
            }
        }

        private static HtmlNode GetOddsTableNode(HtmlDocument document)
        {
            return document.DocumentNode.SelectSingleNode("//div[@id='oddsTableContainer']");
        }

        public Dictionary<string, string> ParseLinksFromCompetitionPageSimple(string html)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ParseLinksFromCompetitionPageComplex(string html)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OcMarketDefinition> BuildMarketDefinitions(string html)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html, EventByCountry ebc)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEventFromJson(IEnumerable<OcMarket> markets, EventByCountry ebc)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            throw new NotImplementedException();
        }
    }
}

