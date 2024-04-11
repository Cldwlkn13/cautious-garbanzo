using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class Question
    {
        [JsonProperty("security-question-id")]
        public int SecurityQuestionId { get; set; }

        [JsonProperty("security-question")]
        public string SecurityQuestion { get; set; }
    }
}
