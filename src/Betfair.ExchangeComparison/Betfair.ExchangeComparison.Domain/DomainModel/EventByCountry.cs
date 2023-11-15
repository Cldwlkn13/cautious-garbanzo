using System.Text;

namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public class EventByCountry
    {
        public EventByCountry()
        {

        }

        public string CountryCode { get; set; }
        public string CompetitionName { get; set; }
        public string EventName { get; set; }
        public DateTime EventStartTime { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.CountryCode)) sb.AppendLine(this.CountryCode + " - ");
            if (!string.IsNullOrEmpty(this.CompetitionName)) sb.AppendLine(this.CompetitionName + " - ");
            if (!string.IsNullOrEmpty(this.EventName)) sb.AppendLine(this.EventName + " - ");
            sb.AppendLine(this.EventStartTime.ToString("dd-MM-yyyy HH:mm"));
            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (string.IsNullOrEmpty(this.CountryCode) ||
               string.IsNullOrEmpty(this.CompetitionName) ||
               string.IsNullOrEmpty(this.CountryCode))
                return false;

            var o = obj as EventByCountry;

            return this.CountryCode == o.CountryCode &&
                this.CompetitionName == o.CompetitionName &&
                this.EventName == o.EventName;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(this.CountryCode) ||
               string.IsNullOrEmpty(this.CompetitionName) ||
               string.IsNullOrEmpty(this.CountryCode))
                return 1;

            return 2 * this.CountryCode.GetHashCode() *
                this.CompetitionName.GetHashCode() *
                this.EventName.GetHashCode();
        }
    }
}

