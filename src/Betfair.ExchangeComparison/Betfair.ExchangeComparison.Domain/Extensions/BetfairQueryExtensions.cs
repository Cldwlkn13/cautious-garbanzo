using System;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;
using Microsoft.Net.Http.Headers;

namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class BetfairQueryExtensions
    {
        public static TimeRange TimeRangeForNextWholeDays(int daysAhead = 1)
        {
            return new TimeRange
            {
                From = DateTime.Today,
                To = DateTime.Today.AddDays(daysAhead)
            };
        }

        public static TimeRange TimeRangeForNextDays(int daysAhead = 1)
        {
            return new TimeRange
            {
                From = DateTime.UtcNow,
                To = DateTime.Today.AddDays(daysAhead)
            };
        }

        public static TimeRange TimeRangeForNextHours(int hoursAhead = 1)
        {
            return new TimeRange
            {
                From = DateTime.UtcNow,
                To = DateTime.UtcNow.AddHours(hoursAhead)
            };
        }

        public static MarketFilter CustomTimeRangeDays(this MarketFilter marketFilter, int daysAhead = 1)
        {
            var time = new TimeRange
            {
                From = DateTime.UtcNow,
                To = DateTime.Today.AddDays(daysAhead)
            };

            marketFilter.MarketStartTime = time;

            return marketFilter;
        }

        public static MarketFilter CustomTimeRangeHours(this MarketFilter marketFilter, int hoursAhead = 1)
        {
            var time = new TimeRange
            {
                From = DateTime.UtcNow,
                To = DateTime.Today.AddHours(hoursAhead)
            };

            marketFilter.MarketStartTime = time;

            return marketFilter;
        }

        public static HashSet<string> CountryCodes(this Sport sport)
        {
            return CountryCodes(sport.SportMap());
        }

        public static HashSet<string> CountryCodes(this string eventTypeId)
        {
            switch (eventTypeId)
            {
                case "7":
                    return new HashSet<string>()
                    {
                        "GB",
                        "IE"
                    };
            }

            return new HashSet<string>();
        }

        public static HashSet<string> MarketTypes(this Sport sport)
        {
            return MarketTypes(sport.SportMap());
        }

        public static HashSet<string> MarketTypes(this string eventTypeId)
        {
            switch (eventTypeId)
            {
                case "7":
                    return new HashSet<string>()
                    {
                        "WIN",
                        "PLACE",
                        "OTHER_PLACE"
                    };
                case "1":
                    return new HashSet<string>()
                    {
                        "MATCH_ODDS",
                        "OVER_UNDER_15",
                        "OVER_UNDER_25",
                        "OVER_UNDER_35",
                        "BOTH_TEAMS_TO_SCORE"
                    };
            }

            return new HashSet<string>();
        }

        public static string SportMap(this Sport sport)
        {
            switch (sport)
            {
                case Sport.Racing:
                    return "7";
                case Sport.Football:
                    return "1";
            }

            return "0";
        }


    }
}

