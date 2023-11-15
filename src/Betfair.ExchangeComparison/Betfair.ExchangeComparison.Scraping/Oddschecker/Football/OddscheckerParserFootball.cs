using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker;
using Betfair.ExchangeComparison.Scraping.Extensions;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Football
{
    public class OddscheckerParserFootball<T> : IOddscheckerParser<T>
    {
        public OddscheckerParserFootball()
        {
        }

        public Dictionary<string, string> ParseLinksFromCompetitionPageSimple(string html)
        {
            var document = html!.LoadDocument();

            var fixturesTable = document.DocumentNode.SelectSingleNode("//table[@class='at-hda standard-list']");
            var tableRows = fixturesTable.SelectNodes("//tr");

            var date = DateTime.Today;
            var matches = new Dictionary<string, string>();
            foreach (var row in tableRows)
            {
                var rowDocument = row.InnerHtml.LoadDocument().DocumentNode;

                if (row.HasAttributes &&
                    row.Attributes["class"] != null &&
                    row.Attributes["class"].Value.Contains("hda-header"))
                {
                    var dateP = rowDocument.SelectSingleNode("//p[@class='beta-caption1 event-date']");
                    if (dateP == null)
                    {
                        dateP = rowDocument.SelectSingleNode("//td[@class='bh-date beta-caption2']");
                    }
                    if (dateP != null)
                    {
                        var dateStr = dateP.InnerText;
                        if (!dateStr.TryParseCustomDate(out date))
                        {
                            continue;
                        }
                        else
                        {
                            if (date > DateTime.Today)
                            {
                                break;
                            }
                        }
                    }
                }

                if (row.HasAttributes &&
                    row.Attributes["class"] != null &&
                    row.Attributes["class"].Value.Contains("match-on"))
                {
                    var timeNode = rowDocument.SelectSingleNode("//td[@class='time all-odds-click']");
                    var time = "";

                    if (timeNode != null)
                    {
                        time = timeNode.InnerText;
                        if (time == "In Play")
                            continue;
                    }

                    var names = rowDocument
                        .SelectManyNodes("p", "class", "fixtures-bet-name beta-footnote")
                        .Select(n => n.InnerText)
                        .ToList();

                    if (names.Count != 2)
                    {
                        continue;
                    }

                    var pair = $"{names[0]}-v-{names[1]}"
                        .Replace(" ", "-")
                        .ToLower()
                        .Trim();

                    if (!matches.ContainsKey(pair))
                        matches.Add(pair, time);

                }
            }

            return matches;
        }

        public Dictionary<string, string> ParseLinksFromCompetitionPageComplex(string html)
        {
            var document = html!.LoadDocument();

            var matchNodes = document.DocumentNode.SelectManyNodesContains("div", "class", "TeamWrapper_tedwdbv");

            var matches = new Dictionary<string, string>();
            //foreach (var row in matchNodes)
            //{
            //    var names = row.SelectManyNodes("p", "class", "fixtures-bet-name beta-footnote").Select(n => n.InnerText).ToList();

            //    foreach (var matchPair in names.Chunk(2))
            //    {
            //        var pair = $"{matchPair[0]}-v-{matchPair[1]}".Replace(" ", "-").ToLower().Trim();
            //        if (!matches.Contains(pair))
            //            matches.Add(pair);
            //    }
            //}

            return matches;
        }

        public IEnumerable<OcMarketDefinition> BuildMarketDefinitions(string html)
        {
            var result = new List<OcMarketDefinition>();

            try
            {
                var document = html!.LoadDocument();

                var err = document.DocumentNode.SelectSingleNode("//script[@data-hypernova-key='subeventmarkets']");

                var json = err.InnerHtml.Replace("<!--", "").Replace("-->", "");

                //File.WriteAllText("TestHtml/errr.json", errr);

                //var json = JsonConvert.DeserializeObject(errr);

                JObject jo = JObject.Parse(json);
                var parseToken = string.Empty;

                foreach (JToken token in jo.FindTokens("markets"))
                {
                    var str = token.ToString();
                    if (str.Contains("entities") && str.Contains("ocMarketId"))
                    {
                        //File.WriteAllText("TestHtml/errrrr.json", token.ToString());
                        parseToken = str.Replace("\"entities\": {", "\"entities\": [");
                        parseToken = parseToken.Replace("},\n  \"ids\":", "],\n  \"ids\":");

                        while (parseToken.Contains("\"3"))
                        {
                            var indexOf = parseToken.IndexOf("\"3");
                            var idStr = parseToken.Substring(indexOf, 13);
                            parseToken = parseToken.Replace(idStr, "");
                        }
                    }
                }

                var ocMarkets = JsonConvert.DeserializeObject<OcEntities>(parseToken);

                return ocMarkets.OcMarketDefinitions;
            }
            catch (Exception exception)
            {
                //Console.WriteLine($"Exception={exception} on BuildScrapedEvent; {compoundObj.EventWithCompetition.Event.Name} " +
                //    $"{compoundObj.SportsbookMarket.marketName} {compoundObj.SportsbookMarket.marketStartTime}");

                return result;
            }
        }

        public ScrapedEvent BuildScrapedEventFromJson(IEnumerable<OcMarket> ocMarkets, EventByCountry ebc)
        {
            var scrapedMarkets = new List<ScrapedMarket>();
            foreach (var ocMarket in ocMarkets)
            {
                var selections = ocMarket.Odds.GroupBy(o => o.BetId);
                var bets = ocMarket.Bets;

                var scrapedRunners = new List<ScrapedRunner>();
                foreach (var selection in selections)
                {
                    var s = selection.First();
                    var ocBet = bets.Where(b => b.BetId != null).FirstOrDefault(b => b.BetId.ToString() == s.BetId.ToString());
                    var line = string.IsNullOrEmpty(ocBet.Line) ? "" : $" {ocBet.Line}";
                    var scrapedPrices = new List<ScrapedPrice>();
                    foreach (var o in selection)
                    {
                        if (!OddscheckerMappingExtensions.TryMapBookmaker(o.BookmakerCode, out var bookmaker))
                        {
                            continue;
                        }
                        var scrapedPrice = new ScrapedPrice(o.OddsDecimal, bookmaker);
                        scrapedPrices.Add(scrapedPrice);
                    }
                    var scrapedRunner = new ScrapedRunner($"{ocBet.BetName}{line}", scrapedPrices);
                    scrapedRunners.Add(scrapedRunner);
                }
                scrapedMarkets.Add(new ScrapedMarket(ocMarket.MarketTypeName, scrapedRunners));
            }

            var scrapedEvent = new ScrapedEvent(scrapedMarkets);
            //scrapedEvent.MappedEvent = compoundObj;
            scrapedEvent.BetfairName = ebc.EventName;
            scrapedEvent.StartTime = ebc.EventStartTime
                .ConvertUtcToBritishIrishLocalTime();

            return scrapedEvent;
        }

        public ScrapedEvent BuildScrapedEvent(string html, EventByCountry ebc)
        {
            //try
            //{


            //    var markets = document.DocumentNode.SelectManyNodes("article", "class", "MarketWrapper_mgnb13w");

            //var scrapedMarkets = new List<ScrapedMarket>();

            //    foreach (var market in markets)
            //    {
            //        var marketDocumentNode = market.InnerHtml.LoadDocument().DocumentNode;

            //        var header = marketDocumentNode.SelectSingleNode("//h2[@class='AccordionHeader_a1opm9ml']");

            //        if (header == null)
            //        {
            //            continue;
            //        }

            //        if (scrapedMarkets.Any(m => m.Name == header.InnerText))
            //        {
            //            continue;
            //        }

            //        if (header.InnerText == "Win Market")
            //        {
            //            if (TryScrapeMarket(marketDocumentNode, "Win Market", out var winMarket))
            //            {
            //                scrapedMarkets.Add(winMarket);
            //            }
            //        }

            //        if (header.InnerText == "Total Goals Over/Under")
            //        {
            //            if (TryScrapeMarket(marketDocumentNode, "Total Goals Over/Under", out var goalsMarkets))
            //            {
            //                scrapedMarkets.Add(goalsMarkets);
            //            }
            //        }

            //        if (header.InnerText == "Both Teams To Score")
            //        {
            //            if (TryScrapeMarket(marketDocumentNode, "Both Teams To Score", out var bttsMarket))
            //            {
            //                scrapedMarkets.Add(bttsMarket);
            //            }
            //        }
            //    }

            //    var scrapedEvent = new ScrapedEvent(scrapedMarkets);
            //    //scrapedEvent.MappedEvent = compoundObj;
            //    scrapedEvent.Name = ebc.EventName;
            //    scrapedEvent.StartTime = ebc.EventStartTime
            //        .ConvertUtcToBritishIrishLocalTime();

            return new ScrapedEvent();
            //}
            //catch (Exception exception)
            //{
            //    //Console.WriteLine($"Exception={exception} on BuildScrapedEvent; {compoundObj.EventWithCompetition.Event.Name} " +
            //    //    $"{compoundObj.SportsbookMarket.marketName} {compoundObj.SportsbookMarket.marketStartTime}");

            //    return new ScrapedEvent();
            //}
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            throw new NotImplementedException();
        }

        private static bool TryScrapeMarket(HtmlNode marketDocumentNode, string name, out ScrapedMarket result)
        {
            result = new ScrapedMarket();
            var scrapedRunners = new List<ScrapedRunner>();

            var selections = marketDocumentNode.SelectManyNodesContains("a", "class", "BetRowLeftBetNameLink_b1h3dl3m");
            var oddsRows = marketDocumentNode.SelectManyNodesContains("div", "class", "oddsAreaWrapper_o17xb9rs");

            if (selections.Count != oddsRows.Count)
            {
                return false;
            }

            var selectionArray = selections.ToArray();
            var index = 0;
            foreach (var row in oddsRows)
            {
                var selectionName = selectionArray[index].InnerText;
                index++;

                var cells = row.InnerHtml.LoadDocument().DocumentNode.SelectManyNodesContains("button", "class", "OddsCellDesktop_obvpjra");

                var prices = new List<ScrapedPrice>();
                foreach (var cell in cells)
                {
                    if (!cell.HasAttributes)
                    {
                        continue;
                    }

                    if (!cell.Attributes.Contains("data-bk"))
                    {
                        continue;
                    }

                    var bk = cell.Attributes["data-bk"].Value;

                    if (!bk.TryMapBookmaker(out var bookmaker))
                    {
                        continue;
                    }

                    var price = cell.InnerText;

                    if (string.IsNullOrEmpty(cell.InnerText))
                    {
                        continue;
                    }

                    prices.Add(new ScrapedPrice(cell.InnerText, bookmaker));
                }

                scrapedRunners.Add(new ScrapedRunner(selectionName, prices));
            }

            result = new ScrapedMarket(name, scrapedRunners);

            return true;
        }
    }
}

