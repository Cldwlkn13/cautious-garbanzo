using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Domain.Extensions;

namespace Betfair.ExchangeComparison.Domain.ScrapingModel
{
    public class ScrapedPrice
    {
        public ScrapedPrice()
        {
            PriceString = "";
        }

        public ScrapedPrice(string priceString, Bookmaker bookmaker)
        {
            Bookmaker = bookmaker;
            PriceString = "";

            if (!string.IsNullOrEmpty(priceString) && priceString.Contains("/"))
            {
                SplitPriceString(priceString);
            }
        }

        public ScrapedPrice(double price, Bookmaker bookmaker, bool addOne = false)
        {
            Bookmaker = bookmaker;
            PriceString = CompilePriceString((decimal)price);
            Decimal = (decimal)price;
        }

        public Bookmaker Bookmaker { get; set; }
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
        }

        private string CompilePriceString(decimal @decimal)
        {
            try
            {
                var fraction = (@decimal - 1).RealToFraction();

                Denominator = fraction.D;
                Numerator = fraction.N;
            }
            catch
            {
                Denominator = 0;
                Numerator = 0;
            }

            return $"{Numerator}/{Denominator}";
        }

        private decimal GetDecimal()
        {
            return (Numerator / (decimal)Denominator) + 1;
        }

        public override string ToString()
        {
            return $"{Bookmaker} - {PriceString}";
        }
    }
}

