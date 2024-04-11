using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class LoginResponse
    {
        [JsonProperty("session-token")]
        public string SessionToken { get; set; }

        [JsonProperty("user-id")]
        public int UserId { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("account")]
        public Account Account { get; set; }

        [JsonProperty("last-login")]
        public DateTime LastLogin { get; set; }
    }
}
