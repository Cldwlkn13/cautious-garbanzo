using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Extensions;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using HtmlAgilityPack;
using Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Racing
{
    public class OddscheckerParserRacing<T> : IOddscheckerParser<T>
    {
        public OddscheckerParserRacing()
        {
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj)
        {
            try
            {
                var document = html!.LoadDocument();

                var oddsTableNode = GetOddsTableNode(document);

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

                    scrapedRunners.Add(new ScrapedRunner(name, prices));
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

        private static HtmlNode GetOddsTableNode(HtmlDocument document)
        {
            return document.DocumentNode.SelectSingleNode("//div[@id='oddsTableContainer']");
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseLinksFromCompetitionPage(string html)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseLinksFromCompetitionPageSimple(string html)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParseLinksFromCompetitionPageComplex(string html)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html, EventByCountry ebc)
        {
            throw new NotImplementedException();
        }

        Dictionary<string, string> IOddscheckerParser<T>.ParseLinksFromCompetitionPageSimple(string html)
        {
            throw new NotImplementedException();
        }

        Dictionary<string, string> IOddscheckerParser<T>.ParseLinksFromCompetitionPageComplex(string html)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OcMarketDefinition> BuildMarketDefinitions(string html)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEventFromJson(IEnumerable<OcMarket> markets, EventByCountry ebc)
        {
            throw new NotImplementedException();
        }
    }
}

