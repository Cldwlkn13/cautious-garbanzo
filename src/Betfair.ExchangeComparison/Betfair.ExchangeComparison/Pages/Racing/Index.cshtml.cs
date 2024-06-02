using Betfair.ExchangeComparison.Domain.Definitions.Sport;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;
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
        private readonly IScrapingProcessor<SportRacing> _scrapingProcessor;
        private readonly IScrapingOrchestratorRacing _scrapingOrchestrator;
        private readonly ITradingHandler _tradingHandler;

        public CatalogViewModel CatalogViewModel { get; set; }

        [BindProperty]
        public RacingFormModel FormModel { get; set; }
        public List<SelectListItem> SelectListBookmakers { get; set; }
        public int RefreshRate { get; set; }
        public bool RefreshIsOn { get; set; }

        public IndexModel(IScrapingOrchestratorRacing scrapingOrchestrator, ICatalogProcessor catalogProcessor, 
            IEventProcessor eventProcessor, IScrapingProcessor<SportRacing> scrapingProcessor, ITradingHandler tradingHandler)
        {
            _scrapingOrchestrator = scrapingOrchestrator;
            _catalogProcessor = catalogProcessor;
            _eventProcessor = eventProcessor;
            _scrapingProcessor = scrapingProcessor;
            _tradingHandler = tradingHandler;

            CatalogViewModel = new CatalogViewModel();
        }

        public async Task<IActionResult> OnGet()
        {
            var basePageModel = PageProcessor.Process(HttpContext.Session, Sport.Racing);

            SelectListBookmakers = basePageModel.SelectListBookmakers;
            RefreshRate = basePageModel.RefreshRateSeconds;
            RefreshIsOn = basePageModel.RefreshIsOn;

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

                CatalogViewModel = await _tradingHandler.TradeCatalogue(CatalogViewModel);
                CatalogViewModel.CurrentOffers = CatalogViewModel.Markets.ToDictionary(o => o, o => o.CurrentMarketOffers);

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
            HttpContext.Session.SetString("RefreshRate-Racing", formModel.RefreshRateSeconds.ToString());
            HttpContext.Session.SetString("RefreshIsOn-Racing", formModel.RefreshIsOn.ToString());

            return await OnGet();
        }

        public async Task<PartialViewResult> OnGetRacingCatalog()
        {
            CatalogViewModel = await GetCatalogViewModel();

            return PartialView("RacingCatalogPartial", this);
        }

        [NonAction]
        public virtual PartialViewResult PartialView(string viewName, object model)
        {
            ViewData.Model = model;

            return new PartialViewResult()
            {
                ViewName = viewName,
                ViewData = ViewData,
                TempData = TempData
            };
        }

        private async Task<CatalogViewModel> GetCatalogViewModel()
        {
            var result = new CatalogViewModel();
            var basePageModel = PageProcessor.Process(HttpContext.Session, Sport.Racing);

            SelectListBookmakers = basePageModel.SelectListBookmakers;
            RefreshRate = basePageModel.RefreshRateSeconds;
            RefreshIsOn = basePageModel.RefreshIsOn;

            _scrapingProcessor.ProcessStartStops(basePageModel);

            try
            {
                var baseCatalogModel = await _catalogProcessor.Process(Sport.Racing);

                if (baseCatalogModel == null)
                {
                    //log problem here

                    return result;
                }

                if (!_scrapingProcessor.TryProcessScrapedEvents(
                    basePageModel, out List<ScrapedEvent> scrapedEvents))
                {
                    //log problem here

                    return result;
                }

                result = await _eventProcessor.Process(baseCatalogModel, basePageModel, scrapedEvents);

                var usageModel = await _scrapingOrchestrator.Usage();
                result.UsageModel = usageModel;

                result = await _tradingHandler.TradeCatalogue(result);
                result.CurrentOffers = result.Markets.ToDictionary(o => o, o => o.CurrentMarketOffers);

                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"PAGE_EXCEPTION; " +
                    $"Exception={exception.Message}");

                return result;
            }
        }
    }
}
