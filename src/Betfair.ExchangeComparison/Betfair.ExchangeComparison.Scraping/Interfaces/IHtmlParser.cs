using System;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IHtmlParser
    {
        ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj);
    }
}

