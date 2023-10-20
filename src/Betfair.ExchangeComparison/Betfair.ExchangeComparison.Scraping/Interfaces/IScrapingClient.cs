using System;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingClient
    {
        Task<string> Scrape(string url);
    }
}

