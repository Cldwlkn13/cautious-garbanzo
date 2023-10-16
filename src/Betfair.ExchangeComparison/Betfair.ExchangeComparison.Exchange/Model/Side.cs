using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Betfair.ExchangeComparison.Exchange.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Side
    {
        BACK, LAY
    }
}
