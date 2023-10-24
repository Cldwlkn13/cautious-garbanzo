using System;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class UsageModel
    {
        [JsonProperty("api_credit_usage")]
        public long ApiCreditUsage { get; set; }

        [JsonProperty("api_credit_limit")]
        public long ApiCreditLimit { get; set; }
    }
}

