using Microsoft.AspNetCore.Mvc.Rendering;

namespace Betfair.ExchangeComparison.Pages.Extensions
{
    public static class PageExtensions
    {
        public static List<SelectListItem> SelectList(this Type type, string[] cases)
        {
            var result = new List<SelectListItem>();
            foreach (var obj in Enum.GetValues(type))
            {
                if (cases.Contains(obj.ToString()))
                {
                    result.Add(new SelectListItem()
                    {
                        Value = ((int)obj).ToString(),
                        Text = obj.ToString()
                    });
                }
            }

            return result;
        }
    }
}

