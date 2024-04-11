using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betfair.ExchangeComparison.Domain.Matchbook
{
    public class EventParticipant
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("event-id")]
        public long EventId { get; set; }

        [JsonProperty("participant-name")]
        public string ParticipantName { get; set; }

        [JsonProperty("jockey-name")]
        public string JockeyName { get; set; }

        [JsonProperty("trainer-name")]
        public string TrainerName { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
