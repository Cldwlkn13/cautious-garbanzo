using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Betfair.ExchangeComparison.Exchange.Model
{
    public class APINGException : System.Exception
    {
        public APINGException(SerializationInfo info, StreamingContext context)
        {
            this.ErrorDetails = info.GetString("errorDetails");
            this.ErrorCode = info.GetString("errorCode");
            this.RequestUUID = info.GetString("requestUUID");
        }

        public APINGException()
        {

        }

        [JsonProperty(PropertyName = "errorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty(PropertyName = "errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty(PropertyName = "requestUUID")]
        public string RequestUUID { get; set; }
    }
}
