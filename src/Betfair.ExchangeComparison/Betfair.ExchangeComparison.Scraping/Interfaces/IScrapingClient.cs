using System;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.Interfaces
{
    public interface IScrapingClient
    {
        string Scrape(string url);
        Task<string> ScrapeAsync(string url);
        Task Usage();
    }
}

