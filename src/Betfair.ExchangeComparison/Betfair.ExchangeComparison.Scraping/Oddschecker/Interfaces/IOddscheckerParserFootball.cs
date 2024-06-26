﻿using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Domain.ScrapingModel.Oddschecker;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces
{
    public interface IOddscheckerParserFootball
    {
        Dictionary<string, string> ParseLinksFromCompetitionPageSimple(string html);
        Dictionary<string, string> ParseLinksFromCompetitionPageComplex(string html);

        IEnumerable<OcMarketDefinition> BuildMarketDefinitions(string html);

        ScrapedEvent BuildScrapedEvent(string html, EventByCountry ebc);
        ScrapedEvent BuildScrapedEventFromJson(IEnumerable<OcMarket> markets, EventByCountry ebc);
    }
}

