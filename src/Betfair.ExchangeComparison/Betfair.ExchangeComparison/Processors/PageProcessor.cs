using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Pages.Model;

namespace Betfair.ExchangeComparison.Processors
{
    public class PageProcessor
    {
        public PageProcessor()
        {

        }

        public static BasePageModel Process(ISession session, Sport sport)
        {
            var bookmaker = ParseBookmakerFromSession(session, sport);
            Bookmaker[] bookmakers = new Bookmaker[] { };
            Bookmaker[] isScrapableBookmakers = new Bookmaker[] { };

            switch (sport)
            {
                case Sport.Football:
                    bookmakers = new Bookmaker[]
                    {
                        Bookmaker.BetfairSportsbook,
                        Bookmaker.WilliamHillDirect
                    };

                    isScrapableBookmakers = new Bookmaker[]
                    {
                        Bookmaker.WilliamHillDirect
                    };
                    break;

                case Sport.Racing:
                    bookmakers = new Bookmaker[]
                    {
                        Bookmaker.BetfairSportsbook,
                        Bookmaker.WilliamHill,
                        Bookmaker.Ladbrokes,
                        Bookmaker.Boylesports
                    };

                    isScrapableBookmakers = new Bookmaker[]
                    {
                        Bookmaker.WilliamHill,
                        Bookmaker.Ladbrokes,
                        Bookmaker.Boylesports
                    };
                    break;
            }

            var refreshRate = ParseRefreshRateFromSession(session, sport);
            var refreshIsOn = ParseRefreshIsOnFromSession(session, sport);

            var model = new BasePageModel(sport, bookmaker, bookmakers, isScrapableBookmakers, refreshRate, refreshIsOn);

            return model;
        }

        private static Bookmaker ParseBookmakerFromSession(ISession session, Sport sport)
        {
            var bookmakerString = session.GetString($"Bookmaker-{sport}") ?? "Other";

            Bookmaker bookmaker;
            if (!Enum.TryParse(typeof(Bookmaker), bookmakerString, out var savedBookmaker))
            {
                bookmaker = Bookmaker.BetfairSportsbook;
            }
            else
            {
                bookmaker = (Bookmaker)Enum.Parse(typeof(Bookmaker), savedBookmaker.ToString());
            }

            return bookmaker;
        }

        private static int ParseRefreshRateFromSession(ISession session, Sport sport)
        {
            var sessionRate = session.GetString($"RefreshRate-{sport}") ?? "5";

            if (!int.TryParse(sessionRate, out var result))
            {
                return 5;
            }

            return result;
        }

        private static bool ParseRefreshIsOnFromSession(ISession session, Sport sport)
        {
            var sessionRate = session.GetString($"RefreshIsOn-{sport}") ?? "False";

            if (!bool.TryParse(sessionRate, out var result))
            {
                return false;
            }

            return result;
        }
    }
}

