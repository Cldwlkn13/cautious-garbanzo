using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Scraping.Interfaces;
using Betfair.ExchangeComparison.Scraping.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;

namespace Betfair.ExchangeComparison.Scraping
{
    public class ScrapingClient : IScrapingClient
    {
        private readonly ILogger<ScrapingClient> _logger;
        private readonly IOptions<ScrapingSettings> _settings;

        private string ZenRowsApiKey { get; set; }

        private static int _maxRetryAttempts = 3;
        private static TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(1);

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

        public async Task<string> ScrapeAsync(string url, bool jsRender = false, bool antiBot = false)
        {
            var client = new RestClient($"https://api.zenrows.com/v1/?apikey={ZenRowsApiKey}&url={url}");
            var request = new RestRequest();

            if (jsRender) request.AddParameter("js_render", "true");
            if (antiBot) request.AddParameter("antibot", "true");

            string html = string.Empty;
            try
            {
                var result = await RetryPolicy().ExecuteAsync(
                     () => client.ExecuteAsync(request));

                return result.Content == null ? "" : result.Content;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"SCRAPE_RESPONSE_COMPLETE_FAILURE url={url} Exception={exception.Message}");

                return string.Empty;
            }
        }

        public async Task<UsageModel> Usage()
        {
            var client = new RestClient($"https://api.zenrows.com/v1/usage?apikey={ZenRowsApiKey}");
            var request = new RestRequest();

            string html = string.Empty;
            try
            {
                var result = await client.GetAsync(request);

                var content = result.Content == null ? "" : result.Content;

                Console.WriteLine($"Usage={content}");

                var usageModel = JsonConvert.DeserializeObject<UsageModel>(content) ??
                    throw new NullReferenceException($"USAGE_MODEL_PARSE_FAILURE");

                return usageModel;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"USAGE_RESPONSE_FAILED Exception={exception.Message}");

                return new UsageModel();
            }
        }

        private static AsyncRetryPolicy<RestResponse> RetryPolicy()
        {
            return Policy
                     .HandleResult<RestResponse>(x => !x.IsSuccessful)
                     .WaitAndRetryAsync(_maxRetryAttempts, x => _pauseBetweenFailures,
                         (iRestResponse, timeSpan, retryCount, context) =>
                         {
                             Console.WriteLine($"SCRAPE_RESPONSE_FAILED; " +
                                 $"HttpStatusCode={iRestResponse.Result.StatusCode}. " +
                                 $"Waiting {timeSpan.TotalSeconds} seconds before retry. " +
                                 $"Number attempt {retryCount}. " +
                                 $"Uri={iRestResponse.Result.ResponseUri}; " +
                                 $"RequestResponse={iRestResponse.Result.Content}");
                         });
        }
    }
}

