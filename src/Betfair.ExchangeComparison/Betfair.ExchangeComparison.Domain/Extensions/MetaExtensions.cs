using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class MetaExtensions
    {
        public static MarketMeta MapMarketMeta(this BestRunner bestRunner)
        {
            var result = new MarketMeta();
            result.Furlongs = bestRunner.MarketCatalogue.MarketName.Substring(0, 5).TotalFurlongs();
            result.IsHandicap = bestRunner.MarketCatalogue.MarketName.Contains("Hcap");
            result.IsHurdle = bestRunner.MarketCatalogue.MarketName.Contains("Hrd");
            result.IsChase = bestRunner.MarketCatalogue.MarketName.Contains("Chs");
            result.IsNationalHuntFlat = bestRunner.MarketCatalogue.MarketName.Contains("NHF");
            result.IsMaidenOrNovice = bestRunner.MarketCatalogue.MarketName.Contains("Mdn") || bestRunner.MarketCatalogue.MarketName.Contains("Nov");
            result.IsJumps = result.IsHurdle || result.IsChase || result.IsNationalHuntFlat;
            result.IsTurf = result.IsJumps ? true :
                AllWeatherTracks().Contains(bestRunner.Event.Venue) ? false : true;

            return result;
        }

        public static bool IsFlatTurf(this MarketMeta marketMeta)
        {
            return marketMeta.IsTurf && !marketMeta.IsJumps;
        }

        public static int TotalFurlongs(this string data)
        {
            var en = data.Substring(0, 5);
            var miles = 0;
            var furlongs = 0;
            if (en.Contains("m") && !en.Contains("f"))
            {
                int.TryParse(en.Split("m")[0], out miles);
            }
            else if (!en.Contains("m") && en.Contains("f"))
            {
                int.TryParse(en.Split("f")[0], out furlongs);
            }
            else
            {
                int.TryParse(en.Split("m")[0], out miles);
                int.TryParse(en.Substring(2, 2).Split("f")[0], out furlongs);
            }

            return (miles * 8) + furlongs;
        }

        public static string[] AllWeatherTracks()
        {
            return new string[]
            {
                "Kempton",
                "Lingfield",
                "Chelmsford City",
                "Chelmsford",
                "Wolverhampton",
                "Dundalk",
                "Newcastle",
                "Southwell"
            };
        }
    }
}
