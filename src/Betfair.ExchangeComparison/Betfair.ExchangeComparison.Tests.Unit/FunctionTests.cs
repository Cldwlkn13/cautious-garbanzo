using Betfair.ExchangeComparison.Domain.Extensions;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Tests.Unit
{
    public class FuntionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WeightedAverageTest()
        {
            var prices = new List<PriceSize>();
            prices.Add(new PriceSize { Price = 2, Size = 10 });
            prices.Add(new PriceSize { Price = 3, Size = 10 });
            prices.Add(new PriceSize { Price = 4, Size = 10 });

            var result = prices.WeightedAverage(x => x.Price, x => x.Size);

            Assert.Pass();
        }
    }
}