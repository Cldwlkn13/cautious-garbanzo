using System;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.WilliamHill.Interfaces;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.WilliamHill.Football
{
    public class WilliamHillParserFootball<T> : IWilliamHillParser<T>
    {
        public WilliamHillParserFootball()
        {
        }

        public Dictionary<string, string> BuildHorseRacingLinks(string html)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(HtmlNode htmlNode)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj)
        {
            throw new NotImplementedException();
        }

        public HtmlNodeCollection SplitRaces(string html)
        {
            throw new NotImplementedException();
        }
    }
}

