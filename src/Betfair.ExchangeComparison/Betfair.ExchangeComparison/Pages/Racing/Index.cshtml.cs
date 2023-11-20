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

namespace Betfair.ExchangeComparison.Pages.Racing
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogProcessor _catalogProcessor;
        private readonly IEventProcessor _eventProcessor;
        private readonly ScrapingProcessorRacing _scrapingProcessor;
        private readonly IScrapingOrchestratorRacing _scrapingOrchestrator;

        public CatalogViewModel CatalogViewModel { get; set; }

        [BindProperty]
        public RacingFormModel FormModel { get; set; }
        public List<SelectListItem> SelectListBookmakers { get; set; }

        public IndexModel(IScrapingOrchestratorRacing scrapingOrchestrator, ICatalogProcessor catalogProcessor, IEventProcessor eventProcessor,
            ScrapingProcessorRacing scrapingProcessor)
        {
            _scrapingOrchestrator = scrapingOrchestrator;
            _catalogProcessor = catalogProcessor;
            _eventProcessor = eventProcessor;
            _scrapingProcessor = scrapingProcessor;

            CatalogViewModel = new CatalogViewModel();
        }

        public async Task<IActionResult> OnGet()
        {
            var basePageModel = PageProcessor.Process(HttpContext.Session, Sport.Racing);
            SelectListBookmakers = basePageModel.SelectListBookmakers;

            _scrapingProcessor.ProcessStartStops(basePageModel);

            try
            {
                var baseCatalogModel = await _catalogProcessor.Process(Sport.Racing);

                if (baseCatalogModel == null)
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

                var usageModel = await _scrapingOrchestrator.Usage();
                CatalogViewModel.UsageModel = usageModel;

                return Page();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"PAGE_EXCEPTION; " +
                    $"Exception={exception.Message}");

                return Page();
            }
        }

        public async Task<IActionResult> OnPost(RacingFormModel formModel)
        {
            HttpContext.Session.SetString("Bookmaker-Racing", formModel.Bookmaker.ToString());

            return await OnGet();
        }
    }
}
