using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.ScrapingModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Model;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Betfair.ExchangeComparison.Pages.Football
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogProcessor _catalogProcessor;
        private readonly IEventProcessor _eventProcessor;
        private readonly ScrapingProcessor<SportFootball> _scrapingProcessor;

        [BindProperty]
        public RacingFormModel FormModel { get; set; }
        public List<SelectListItem> SelectListBookmakers { get; set; }
        public CatalogViewModel CatalogViewModel { get; set; }

        public IndexModel(ICatalogProcessor catalogProcessor, IEventProcessor eventProcessor, ScrapingProcessor<SportFootball> scrapingProcessor)
        {
            _catalogProcessor = catalogProcessor;
            _eventProcessor = eventProcessor;
            _scrapingProcessor = scrapingProcessor;

            CatalogViewModel = new CatalogViewModel();
        }

        public async Task<PageResult> OnGet()
        {
            var basePageModel = PageProcessor.Process(HttpContext.Session, Sport.Football);
            SelectListBookmakers = basePageModel.SelectListBookmakers;

            _scrapingProcessor.ProcessStartStops(basePageModel);

            try
            {
                var baseCatalogModel = await _catalogProcessor.Process(Sport.Football);

                if(baseCatalogModel == null)
                {
                    //log problem here

                    return Page();
                }

                if (!_scrapingProcessor.TryProcessScrapedEvents(
                    basePageModel, out List<ScrapedEvent> scrapedEvents))
                {
                    //log problem here

                    return Page();
                }

                CatalogViewModel = await _eventProcessor.Process(baseCatalogModel, basePageModel, scrapedEvents);
              
            }
            catch (Exception exception)
            {
                Console.WriteLine($"PAGE_BUILD_EXCEPTION; " +
                    $"Exception={exception.Message}");
            }

            return Page();
        }
            

        public async Task<IActionResult> OnPost(RacingFormModel formModel)
        {
            HttpContext.Session.SetString(
                "Bookmaker-Football",
                formModel.Bookmaker.ToString());

            return await OnGet();
        }
    }
}
