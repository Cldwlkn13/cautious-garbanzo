using Betfair.ExchangeComparison.Scraping.Interfaces;
using Microsoft.Extensions.Logging;

namespace Betfair.ExchangeComparison.Scraping
{
    public class ScrapingClient : IScrapingClient
    {
        public ILogger<ScrapingClient> _logger;

        public ScrapingClient(ILogger<ScrapingClient> logger)
        {
            _logger = logger;
        }

        public async Task<string> Scrape(string url)
        {
            HttpClient webClient = new HttpClient();
            webClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (iPad; CPU OS 12_2 like Mac OS X) " +
                "AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148");

            string html = string.Empty;
            try
            {
                var result = await webClient.GetStringAsync(url);

                return result;
            }
            catch (Exception exception)
            {
                //_logger.LogError($"SCRAPE_RESPONSE_FAILED url={url}", exception);

                Console.WriteLine($"SCRAPE_RESPONSE_FAILED url={url} Exception={exception}");

                return string.Empty;
            }
        }
    }
}

