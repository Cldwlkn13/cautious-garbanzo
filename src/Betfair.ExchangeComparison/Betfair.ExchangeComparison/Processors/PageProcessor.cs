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

            var bookmakers = new Bookmaker[]
            {
                Bookmaker.BetfairSportsbook,
                Bookmaker.WilliamHillDirect
            };

            var isScrapableBookmakers = new Bookmaker[]
            {
                Bookmaker.WilliamHillDirect
            };

            var model = new BasePageModel(sport, bookmaker, bookmakers, isScrapableBookmakers);

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
    }
}

