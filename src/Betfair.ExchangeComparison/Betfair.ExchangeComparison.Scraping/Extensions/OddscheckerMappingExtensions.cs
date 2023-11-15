using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Microsoft.AspNetCore.Http;
using DuoVia.FuzzyStrings;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Betfair.ExchangeComparison.Scraping.Extensions
{
    public static class OddscheckerMappingExtensions
    {
        public static bool TryMapBookmaker(this string bk, out Bookmaker result)
        {
            return OddscheckerBookmakerMappings().TryGetValue(bk, out result);
        }

        public static Dictionary<string, Bookmaker> OddscheckerBookmakerMappings()
        {
            return new Dictionary<string, Bookmaker>
            {
                { "WH", Bookmaker.WilliamHill },
                { "BY", Bookmaker.Boylesports },
                { "LD", Bookmaker.Ladbrokes }
            };
        }

        public static Dictionary<string, CountryMapping> OddscheckerCompetitionMapping()
        {
            return new Dictionary<string, CountryMapping>
            {
                { "BR", BR() },
                { "CZ", CZ() },
                { "DK", DK() },
                { "GB", GB() },
                { "ID", ID() },
                { "NL", NL() },
                { "PT", PT() },
                { "TR", TR() }
            };
        }

        public static bool TryMapCountry(this EventByCountry ebc, out CountryMapping result)
        {
            result = new CountryMapping();
            var map = OddscheckerCompetitionMapping()[ebc.CountryCode];
            if (map == null)
            {
                return false;
            }

            result = map;
            return true;
        }

        public static bool TryMapCountry(this string countryCode, out CountryMapping result)
        {
            result = new CountryMapping();
            var map = OddscheckerCompetitionMapping()[countryCode];
            if (map == null)
            {
                return false;
            }

            result = map;
            return true;
        }

        public static bool TryMapCompetition(this EventByCountry ebc, out string result)
        {
            result = "";
            var sportMap = OddscheckerCompetitionMapping()[ebc.CountryCode].CompetitionMaps[Sport.Football];
            if (!sportMap.ContainsKey(ebc.CompetitionName))
            {
                return false;
            }
            var map = sportMap[ebc.CompetitionName];
            if (map == null)
            {
                return false;
            }

            result = map;
            return true;
        }

        public static bool TryMapCompetition(this string competitionName, string countryCode, out string result)
        {
            result = "";
            var sportMap = OddscheckerCompetitionMapping()[countryCode].CompetitionMaps[Sport.Football];
            if (!sportMap.ContainsKey(competitionName))
            {
                return false;
            }
            var map = sportMap[competitionName];
            if (map == null)
            {
                return false;
            }

            result = map;
            return true;
        }



        public static bool TryParseCustomDate(this string dateString, out DateTime result)
        {
            result = default(DateTime);

            // Define the custom format string and culture info
            string customFormat = "dddd d MMMM yyyy";
            CultureInfo cultureInfo = new CultureInfo("en-GB");

            // Remove the "th" or "st" or "nd" or "rd" from the day part of the string
            dateString = dateString.Replace("th ", " ").Replace("st ", " ").Replace("nd ", " ").Replace("rd ", " ");

            if (DateTime.TryParseExact(dateString, customFormat, cultureInfo, DateTimeStyles.None, out result))
            {
                return true;
            }
            return false;
        }

        public static CountryMapping BR()
        {
            return new CountryMapping
            {
                CountryCode = "BR",
                CountryName = "world/brazil",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Brazilian Serie A", "serie-a" },
                            { "Brazilian Serie B", "serie-b" },
                        }
                    }
                }
            };
        }

        public static CountryMapping CZ()
        {
            return new CountryMapping
            {
                CountryCode = "CZ",
                CountryName = "czech-republic",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Czech 1 Liga", "synot-liga" },
                            { "Czech 2 Liga", "druha-liga" },
                            //{ "Czech 3 Liga", "tipsport-liga" }
                        }
                    }
                }
            };
        }

        public static CountryMapping DK()
        {
            return new CountryMapping
            {
                CountryCode = "DK",
                CountryName = "denmark",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Danish Superliga", "superligaen" },
                        }
                    }
                }
            };
        }

        public static CountryMapping GB()
        {
            return new CountryMapping
            {
                CountryCode = "GB",
                CountryName = "english",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "English Premier League", "premier-league" },
                        }
                    }
                }
            };
        }

        public static CountryMapping ID()
        {
            return new CountryMapping
            {
                CountryCode = "ID",
                CountryName = "world/indonesia",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Indonesian Liga 1", "super-liga" },
                        }
                    }
                }
            };
        }

        public static CountryMapping NL()
        {
            return new CountryMapping
            {
                CountryCode = "NL",
                CountryName = "netherlands",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Dutch Eredivisie", "eredivisie" },
                        }
                    }
                }
            };
        }

        public static CountryMapping PT()
        {
            return new CountryMapping
            {
                CountryCode = "PT",
                CountryName = "portugal",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Portuguese Primeira Liga", "primeira-liga" },
                        }
                    }
                }
            };
        }

        public static CountryMapping TR()
        {
            return new CountryMapping
            {
                CountryCode = "TR",
                CountryName = "turkey",
                CompetitionMaps = new Dictionary<Sport, Dictionary<string, string>>()
                {
                    {
                        Sport.Football, new Dictionary<string, string>
                        {
                            { "Turkish Cup", "cup" },
                        }
                    }
                }
            };
        }
    }
}

