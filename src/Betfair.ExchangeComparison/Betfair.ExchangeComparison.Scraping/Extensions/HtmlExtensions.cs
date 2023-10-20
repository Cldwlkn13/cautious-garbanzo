using System;
using HtmlAgilityPack;

namespace Betfair.ExchangeComparison.Scraping.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlDocument LoadDocument(this string html)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public static string GetInnerText(this HtmlDocument document, string elementType, string attr, string id)
        {
            var result = document.SelectSingleNode(elementType, attr, id);
            var innerText = result.InnerText;

            return innerText;
        }

        public static HtmlNode SelectSingleNode(this HtmlDocument document, string elementType, string attr, string selector)
        {
            var result = document.DocumentNode.SelectSingleNode($"//{elementType}[@{attr}='{selector}']");

            return result;
        }

        public static HtmlNodeCollection SelectManyNodes(this HtmlDocument document, string elementType, string attr, string selector)
        {
            var result = document.DocumentNode.SelectNodes($"//{elementType}[contains(@{attr},'{selector}')]");

            return result;
        }
    }
}

