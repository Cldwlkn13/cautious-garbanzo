using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Betfair.ExchangeComparison.Exchange.Model
{
    public class PriceProjection
    {
        [JsonProperty(PropertyName = "priceData")]
        public ISet<PriceData> PriceData { get; set; }

        [JsonProperty(PropertyName = "exBestOffersOverrides")]
        public ExBestOffersOverrides ExBestOffersOverrides { get; set; }

        [JsonProperty(PropertyName = "virtualise")]
        public bool? Virtualise { get; set; }

        [JsonProperty(PropertyName = "rolloverStakes")]
        public bool? RolloverStakes { get; set; }


    }
}
