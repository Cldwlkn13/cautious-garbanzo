using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Betfair.ExchangeComparison.Scraping
{
    public class ScrapingClient : IScrapingClient
    {
        private readonly ILogger<ScrapingClient> _logger;
        private readonly IOptions<ScrapingSettings> _settings;

        private string ZenRowsApiKey { get; set; }

        public ScrapingClient(ILogger<ScrapingClient> logger, IOptions<ScrapingSettings> settings)
        {
            _logger = logger;
            _settings = settings;

            ZenRowsApiKey = Environment.GetEnvironmentVariable("ZENROWS_API_KEY") != null ?
                Environment.GetEnvironmentVariable("ZENROWS_API_KEY")! :
                settings.Value.ZENROWS_API_KEY!;
        }

        public string Scrape(string url)
        {
            //var options = new RestClientOptions()
            //{
            //    Proxy = new WebProxy()
            //    {
            //        Address = new Uri("http://proxy.zenrows.com:8001"),
            //        Credentials = new NetworkCredential("{ZenRowsApiKey}", "")
            //    },
            //    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
            //    MaxTimeout = 10000
            //};

            var client = new RestClient($"https://api.zenrows.com/v1/?apikey={ZenRowsApiKey}&url={url}");
            var request = new RestRequest();

            string html = string.Empty;
            try
            {
                var result = client.Get(request);

                return result.Content == null ? "" : result.Content;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"SCRAPE_RESPONSE_FAILED url={url} Exception={exception.Message}");

                return string.Empty;
            }
        }

        public async Task<string> ScrapeAsync(string url)
        {
            //var options = new RestClientOptions()
            //{
            //    Proxy = new WebProxy()
            //    {
            //        Address = new Uri("http://proxy.zenrows.com:8001"),
            //        Credentials = new NetworkCredential("{ZenRowsApiKey}", "")
            //    },
            //    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
            //    MaxTimeout = 10000
            //};

            var client = new RestClient($"https://api.zenrows.com/v1/?apikey={ZenRowsApiKey}&url={url}");
            var request = new RestRequest();

            string html = string.Empty;
            try
            {
                var result = await client.GetAsync(request);

                return result.Content == null ? "" : result.Content;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"SCRAPE_RESPONSE_FAILED url={url} Exception={exception.Message}");

                return string.Empty;
            }
        }

        public async Task Usage()
        {
            var client = new RestClient($"https://api.zenrows.com/v1/usage?apikey={ZenRowsApiKey}");
            var request = new RestRequest();

            string html = string.Empty;
            try
            {
                var result = await client.GetAsync(request);

                var content = result.Content == null ? "" : result.Content;

                Console.WriteLine($"{content}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"USAGE_RESPONSE_FAILED Exception={exception.Message}");
            }
        }
    }
}

