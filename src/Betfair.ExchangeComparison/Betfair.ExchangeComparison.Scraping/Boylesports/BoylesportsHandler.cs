﻿using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.Boylesports
{
    public class BoylesportsHandler : IBoylesportsHandler
    {
        private readonly ILogger<BoylesportsHandler> _logger;
        private readonly IScrapingClient _scrapingClient;
        private readonly IBoylesportsParser _parser;

        public BoylesportsHandler(ILogger<BoylesportsHandler> logger, IScrapingClient scrapingClient, IBoylesportsParser parser)
        {
            _logger = logger;
            _scrapingClient = scrapingClient;
            _parser = parser;
        }

        public async Task<ScrapedEvent> Handle(MarketDetailWithEvent @event)
        {
            var url = UrlBuilder(@event);

            var html = _scrapingClient.Scrape(url);

            if (string.IsNullOrEmpty(html))
            {
                _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={url}");
                return new ScrapedEvent();
            }

            var scrapedEvent = _parser.BuildScrapedEvent(html, @event);

            Console.WriteLine($"Event={@event.EventWithCompetition.Event.Name} " +
                $"{@event.SportsbookMarket.marketStartTime} successfully scraped!");

            return scrapedEvent;
        }

        private static string UrlBuilder(MarketDetailWithEvent compoundObj)
        {
            string baseUrl = "https://www.boylesports.com/sports/horse-racing/uk-ire-featured"; //e.g. 201023/redcar/13:35

            var name = compoundObj.EventWithCompetition.Event.Venue;

            var date = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("ddMMyy");

            var time = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("HH:mm");

            var url = $"{baseUrl}/{date}/{name}/{time}";

            return url;
        }

        public async Task Usage() =>
            await _scrapingClient.Usage();

    }
}

