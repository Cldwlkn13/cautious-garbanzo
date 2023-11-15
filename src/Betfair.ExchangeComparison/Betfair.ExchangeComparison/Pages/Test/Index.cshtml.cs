using Betfair.ExchangeComparison.Pages.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages.Test
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public HtmlModel HtmlModel { get; set; }

        private Random rand = new Random();

        public PageResult OnGet()
        {
            HtmlModel = new HtmlModel() { ContentWin = "SomeText" };

            return Page();
        }

        public PartialViewResult OnGetPartials()
        {
            var val = rand.Next(0, 1000);

            HtmlModel = new HtmlModel() { ContentWin = val.ToString() };

            return PartialView("TestPartial", this);
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
    }
}
