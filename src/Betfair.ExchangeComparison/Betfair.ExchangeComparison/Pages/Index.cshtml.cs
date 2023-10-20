using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Scraping.Boylesports.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IBoylesportsHandler _scraper;

    public IndexModel(ILogger<IndexModel> logger, IBoylesportsHandler scraper)
    {
        _logger = logger;
        _scraper = scraper;
    }

    public async Task OnGet()
    {
        //await _scraper.Handle(new List<CompoundEventWithMarketCatalogue>());
    }
}

