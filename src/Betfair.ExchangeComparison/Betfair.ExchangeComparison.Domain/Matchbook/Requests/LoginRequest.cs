using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook.Requests
{
    public class LoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
