﻿using System;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.Boylesports
{
    public class HtmlParser : IHtmlParser
    {
        public HtmlParser()
        {
        }

        public ScrapedEvent BuildScrapedEvent(string html, MarketDetailWithEvent compoundObj)
        {
            throw new NotImplementedException();
        }

        public ScrapedEvent BuildScrapedEvent(string html)
        {
            throw new NotImplementedException();
        }
    }
}

