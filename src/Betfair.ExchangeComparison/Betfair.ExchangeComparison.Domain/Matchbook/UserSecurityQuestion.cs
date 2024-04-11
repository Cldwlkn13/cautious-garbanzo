using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class UserSecurityQuestion
    {
        [JsonProperty("user-security-question-id")]
        public int UserSecurityQuestionId { get; set; }

        [JsonProperty("question")]
        public Question Question { get; set; }
    }
}
