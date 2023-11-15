using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Oddschecker.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping.Oddschecker.Racing
{
    public class OddscheckerHandlerRacing<T> : OddscheckerHandler<T>, IOddscheckerHandler<T>
    {
        private readonly IOddscheckerParser<T> _parser;

        public OddscheckerHandlerRacing(ILogger<OddscheckerHandlerRacing<T>> logger, IScrapingClient scrapingClient, IOddscheckerParser<T> parser) :
            base(logger, scrapingClient)
        {
            _parser = parser;
        }

        public Task<ScrapedEvent> Handle(Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public async Task<ScrapedEvent> Handle(MarketDetailWithEvent @event, Sport sport = Sport.Racing)
        {
            var url = UrlBuilder(@event);

            var html = await _scrapingClient.ScrapeZenRowsAsync(url,
                new Dictionary<string, string>());

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

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(EventsByCountry ebc)
        {
            throw new NotImplementedException();
        }

        public Task<UsageModel> Usage()
        {
            throw new NotImplementedException();
        }

        private static string UrlBuilder(MarketDetailWithEvent compoundObj)
        {
            string baseUrl = "https://www.oddschecker.com/horse-racing"; //2023-10-26-lingfield/13:20/winner

            var name = compoundObj.EventWithCompetition.Event.Venue.ToLower();

            var date = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("yyyy-MM-dd");

            var time = compoundObj.SportsbookMarket.marketStartTime
                .ConvertUtcToBritishIrishLocalTime()
                .ToString("HH:mm");

            var url = $"{baseUrl}/{date}-{name}/{time}/winner";

            return url;
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(IEnumerable<MarketDetailWithEvent> events, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ScrapedEvent>> HandleEnumerable(Dictionary<EventWithCompetition, List<MarketDetail>> events, Sport sport = Sport.Racing)
        {
            throw new NotImplementedException();
        }
    }
}

