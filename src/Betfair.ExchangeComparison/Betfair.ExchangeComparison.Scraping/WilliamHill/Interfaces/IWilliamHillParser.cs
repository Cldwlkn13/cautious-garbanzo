using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces
{
    public interface IWilliamHillParser<T> : IHtmlParser<T>
    {
        Dictionary<string, string> BuildHorseRacingLinks(string html);
        HtmlNodeCollection SplitRaces(string html);
        ScrapedEvent BuildScrapedEvent(HtmlNode htmlNode);
    }
}

