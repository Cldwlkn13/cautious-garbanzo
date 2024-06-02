using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public abstract class MatchbookClient : IMatchbookClient
    {
        protected readonly IOptions<MatchbookSettings> _settings;
        protected readonly ILogger<MatchbookClient> _logger;

        public MatchbookClient(IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        protected const string DomainAddress = "https://api.matchbook.com";
        protected abstract string EndpointAddress { get; }

        protected async Task<T> HandleResponse<T>(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                try
                {
                    var responseContentJson = await message.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(responseContentJson))
                    {
                        return (T)Convert.ChangeType(responseContentJson, typeof(T));
                    }

                    if(!string.IsNullOrEmpty(responseContentJson))          
                        return JsonConvert.DeserializeObject<T>(responseContentJson)!;

                    throw new Exception($"Invalid Response");
                }
                catch(Exception exception)
                {
                    throw;
                }
            }
            else
            {
                throw new InvalidOperationException($"Response indicates failure - {message.ReasonPhrase}");
            }
        }
    }
}
