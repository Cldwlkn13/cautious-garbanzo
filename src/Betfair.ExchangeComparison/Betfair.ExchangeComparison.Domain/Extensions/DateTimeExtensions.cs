using System;
namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertUtcToBritishIrishLocalTime(this DateTime dateTime)
        {
            var BritishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, BritishZone);
        }

        public static long ToEpochTime(this DateTime dateTime)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var offSeTimeSpan = dateTime.ToUniversalTime() - origin;
            return (long)Math.Floor(offSeTimeSpan.TotalMilliseconds);
        }

        public static string ToDateTimeString(this DateTime datetime)
        {
            return datetime.ToString("dd/MM/yyyy HH:mm");
        }
    }
}

