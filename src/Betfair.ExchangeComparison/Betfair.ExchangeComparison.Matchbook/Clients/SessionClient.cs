using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Matchbook.Interfaces;
using Betfair.ExchangeComparison.Matchbook.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Matchbook.Clients
{
    public class SessionClient : MatchbookClient, ISessionClient
    {
        protected readonly HttpClient _httpClient;
        protected override string EndpointAddress { get => $"{DomainAddress}/bpapi/rest/security/session"; }

        public SessionClient(HttpClient httpClient, IOptions<MatchbookSettings> settings, ILogger<MatchbookClient> logger) : 
            base(settings, logger)
        {
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "api-doc-test-client");
        }

        public async Task<string> PostSessionToken()
        {
            var loginContract = new LoginRequest
            {
                Username = _settings.Value.Username,
                Password = _settings.Value.Password
            };

            var json = JsonConvert.SerializeObject(loginContract);
            var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));

            try
            {
                var message = await _httpClient.PostAsync(new Uri($"{EndpointAddress}"), content);
                var loginResponse = await HandleResponse<LoginResponse>(message);
                return loginResponse.SessionToken;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Login Failed!");
                return string.Empty;
            }
        }

        public async Task<string> GetSessionToken(bool refreshSession = true)
        {
            try
            {
                var message = await _httpClient.GetAsync(new Uri($"{EndpointAddress}"));
                var loginResponse = await HandleResponse<LoginResponse>(message);
                return loginResponse.SessionToken;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Session Request Failed!");
                if (refreshSession)
                {
                    return await PostSessionToken();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public async Task<string> DeleteSessionToken(string sessionToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);
                var message = await _httpClient.DeleteAsync(new Uri($"{EndpointAddress}"));
                var loginResponse = await HandleResponse<LoginResponse>(message);
                return string.Empty;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Session Request Failed!");
                return string.Empty;
            }
        }
    }
}
