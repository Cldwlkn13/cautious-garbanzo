using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using DuoVia.FuzzyStrings;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping
{
    public class ScrapingHandler<T>
    {
        protected readonly ILogger<ScrapingHandler<T>> _logger;
        protected readonly IScrapingClient _scrapingClient;

        public ScrapingHandler(ILogger<ScrapingHandler<T>> logger, IScrapingClient scrapingClient)
        {
            _logger = logger;
            _scrapingClient = scrapingClient;
        }

        public static bool TryMapEventByParticipantNames(KeyValuePair<string, string> scrapedDetails,
            IEnumerable<EventByCountry> ebcs, out EventByCountry result, int minDistance = 10, string splitter = "-v-")
        {
            result = new EventByCountry();

            var bkNamesArray = scrapedDetails.Key.Split(splitter);

            if (bkNamesArray == null || bkNamesArray!.Length != 2)
            {
                return false;
            }

            var similarities = new Dictionary<EventByCountry, double>();
            foreach (var ebc in ebcs.Where(e => e.EventStartTime.ToString("HH:mm") == scrapedDetails.Value))
            {
                var bfNamesArray = ebc.EventName.Split(" v ");

                if (bfNamesArray == null || bfNamesArray!.Length != 2)
                {
                    continue;
                }

                double similarityA1 = bkNamesArray[0].LevenshteinDistance(bfNamesArray[0]);
                double similarityA2 = bkNamesArray[1].LevenshteinDistance(bfNamesArray[1]);

                var similarity1 = similarityA1 + similarityA2;

                //reverse
                double similarityB1 = bkNamesArray[0].LevenshteinDistance(bfNamesArray[1]);
                double similarityB2 = bkNamesArray[1].LevenshteinDistance(bfNamesArray[0]);

                var similarity2 = similarityB1 + similarityB2;

                if ((similarityA1 < 10 || similarityA2 < minDistance))
                {
                    similarities.Add(ebc, 0);
                }
                else if ((similarityB1 < 10 || similarityB2 < minDistance))
                {
                    similarities.Add(ebc, 0);
                }
                else
                {
                    similarities.Add(ebc, Math.Min(similarity1, similarity2));
                }
            }

            var min = similarities.Any() ? similarities.Min(s => s.Value) : 100;
            if (min > 10)
            {
                Console.WriteLine($"Could not map Event={scrapedDetails.Key}");
                return false;
            }

            result = similarities.FirstOrDefault(s => s.Value == min).Key;

            Console.WriteLine($"Mapped BK Event={scrapedDetails.Key} to " +
                $"Event={result.EventName} at " +
                $"{result.EventStartTime}");

            return true;
        }
    }
}

