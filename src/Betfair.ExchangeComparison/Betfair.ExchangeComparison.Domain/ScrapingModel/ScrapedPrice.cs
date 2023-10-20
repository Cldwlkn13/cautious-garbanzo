using System;
using System.Xml.Linq;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedPrice
    {
        public ScrapedPrice()
        {
            PriceString = "";
        }

        public ScrapedPrice(string priceString)
        {
            PriceString = "";

            if (!string.IsNullOrEmpty(priceString) && priceString.Contains("/"))
            {
                SplitPriceString(priceString);
            }
        }

        public decimal Decimal { get; set; }
        public string PriceString { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public void SplitPriceString(string priceString)
        {
            var components = priceString.Split("/");

            if (components.Length != 2)
            {
                throw new InvalidDataException($"PriceString={priceString} cannot be parsed to a price");
            }

            PriceString = priceString;

            if (int.TryParse(components[0], out int numerator))
            {
                Numerator = numerator > 0 ? numerator : 1;
            }

            if (int.TryParse(components[1], out int denominator))
            {
                Denominator = denominator > 0 ? denominator : 1;
            }

            Decimal = GetDecimal();
        }

        private decimal GetDecimal()
        {
            return (Numerator / (decimal)Denominator) + 1;
        }

        public override string ToString()
        {
            return $"{PriceString}";
        }
    }
}

