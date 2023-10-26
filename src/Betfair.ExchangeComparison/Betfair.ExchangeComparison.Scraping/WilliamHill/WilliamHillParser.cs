using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Extensions;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using HtmlAgilityPack;


namespace Betfair.ExchangeComparison.Scraping.WilliamHill
{
    public class WilliamHillParser : IWilliamHillParser
    {
        public WilliamHillParser()
        {
        }

        public Dictionary<string, string> BuildHorseRacingLinks(string html)
        {
            var result = new Dictionary<string, string>();

            var document = html!.LoadDocument();

            //var mainContainerNode = GetMainContainerNode(document);
            //var mainContainerNodeDocument = mainContainerNode.InnerHtml.LoadDocument();

            var headerNodes = GetHeaderNodes(document);
            var ukiSection = headerNodes.ToList().FirstOrDefault(n => n.InnerText == "UK &amp; Ireland");

            if (ukiSection == null)
            {
                return result;
            }
            if (!ukiSection.HasAttributes || !ukiSection.Attributes.Contains("id"))
            {
                return result;
            }

            var ukiId = ukiSection.Attributes["id"].Value;

            var contentNodes = GetContentNodes(document);
            var ukiContentNode = contentNodes
                .FirstOrDefault(n => n.HasAttributes &&
                n.GetAttributeValue("aria-labelledby", "") == ukiId);

            if (ukiContentNode == null)
            {
                return result;
            }

            var ukiContentDocument = ukiContentNode?.InnerHtml.LoadDocument();
            var raceRows = ukiContentDocument.SelectManyNodes("div", "class", "component-race-row");

            foreach (var raceRow in raceRows)
            {
                var raceRowDocument = raceRow.InnerHtml.LoadDocument();
                var linkNode = raceRowDocument.DocumentNode.SelectSingleNode("//a[@class='non-route']");
                var nameNode = raceRowDocument.DocumentNode.SelectSingleNode("//span[@data-test-id='competition-name']");

                var link = linkNode.HasAttributes && linkNode.Attributes.Contains("href") ? linkNode.Attributes["href"].Value : "";
                var name = nameNode.InnerText.Trim();

                result.Add(name!, link!);
            }

            return result;
        }

        public HtmlNodeCollection SplitRaces(string html)
        {
            var document = html!.LoadDocument();

            var races = document.SelectManyNodes("div", "class", "component-race-card");

            return races;
        }

        public ScrapedEvent BuildScrapedEvent(HtmlNode htmlNode)
        {
            var result = new ScrapedEvent();

            var document = htmlNode.InnerHtml.LoadDocument();

            return result;
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            var result = new ScrapedEvent();

            return result;
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj)
        {
            throw new NotImplementedException();
        }

        private static HtmlNode GetMainContainerNode(HtmlDocument document)
        {
            return document.DocumentNode.SelectSingleNode("//div[@data-test-id='competitions']");
        }

        private static HtmlNodeCollection GetHeaderNodes(HtmlDocument document)
        {
            return document.SelectManyNodesContains("header", "data-test-id", "Accordion.header");
        }

        private static HtmlNodeCollection GetContentNodes(HtmlDocument document)
        {
            return document.SelectManyNodesContains("div", "data-test-id", "Accordion.content");
        }
    }
}

