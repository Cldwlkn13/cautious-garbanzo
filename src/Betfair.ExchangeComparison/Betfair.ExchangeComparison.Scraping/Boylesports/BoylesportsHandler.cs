using Betfair.ExchangeComparison.Domain.DomainModel;
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
        private readonly IScrapingClient _scraper;
        private readonly IBoylesportsParser _parser;

        public BoylesportsHandler(ILogger<BoylesportsHandler> logger, IScrapingClient scraper, IBoylesportsParser parser)
        {
            _logger = logger;
            _scraper = scraper;
            _parser = parser;
        }

        public async Task<ScrapedEvent> Handle(CompoundEventWithMarketDetail @event)
        {
            var url = UrlBuilder(@event);

            var html = await _scraper.Scrape(url);

            if (string.IsNullOrEmpty(html))
            {
                _logger.LogWarning($"HTML_EMPTY; Can not continue parsing empty string. url={url}");
                return new ScrapedEvent();
            }

            var scrapedEvent = _parser.BuildScrapedEvent(html, @event);

            Console.WriteLine($"Event={@event.Event.Name} {@event.SportsbookMarket.marketStartTime} successfully scraped!");

            await Task.Delay(500);

            return scrapedEvent;
        }

        private static string UrlBuilder(CompoundEventWithMarketDetail compoundObj)
        {
            string baseUrl = "https://www.boylesports.com/sports/horse-racing/uk-ire-featured"; //e.g. 201023/redcar/13:35

            var name = compoundObj.Event.Venue;

            var date = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("ddMMyy");

            var time = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("HH:mm");

            var url = $"{baseUrl}/{date}/{name}/{time}";

            return url;
        }
    }
}

