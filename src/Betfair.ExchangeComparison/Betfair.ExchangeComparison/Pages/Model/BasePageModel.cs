using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Pages.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Betfair.ExchangeComparison.Pages.Model
{
    public class BasePageModel
    {
        public Sport Sport { get; set; }
        public Bookmaker Bookmaker { get; set; }
        public Provider Provider { get; set; }
        public List<SelectListItem> SelectListBookmakers { get; set; }
        public Bookmaker[] IsScrapableBookmaker { get; set; }

        public BasePageModel(Sport sport, Bookmaker bookmaker, Bookmaker[] bookmakers, Bookmaker[] scrapableBookmakers)
        {
            Sport = sport;
            Bookmaker = bookmaker;
            Provider = MapProviderToBookmaker(bookmaker);

            var bookmakerStrings = bookmakers.Select(b => b.ToString()).ToArray();
            SelectListBookmakers = typeof(Bookmaker).SelectList(bookmakerStrings);
            SetSelectedBookmaker();

            IsScrapableBookmaker = scrapableBookmakers;
        }

        private static Provider MapProviderToBookmaker(Bookmaker bookmaker)
        {
            switch (bookmaker)
            {
                case Bookmaker.Boylesports:
                case Bookmaker.Ladbrokes:
                case Bookmaker.WilliamHill:
                    return Provider.Oddschecker;
                case Bookmaker.BoylesportsDirect:
                    return Provider.BoylesportsDirect;
                case Bookmaker.WilliamHillDirect:
                    return Provider.WilliamHillDirect;
                default:
                    return Provider.BetfairSportsbook;
            }
        }

        private void SetSelectedBookmaker()
        {
            SelectListBookmakers.FirstOrDefault(
                bm => bm.Text == Bookmaker.ToString())!.Selected = true;
        }
    }
}

