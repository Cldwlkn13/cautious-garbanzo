using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Betfair.ExchangeComparison.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGet()
    {
        HttpContext.Session.Set("Set Session", new byte[] { 1 });
    }
}

