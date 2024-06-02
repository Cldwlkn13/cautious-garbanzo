using Betfair.ExchangeComparison.Domain.Interfaces.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Tests.Integration.Extensions;

namespace Betfair.ExchangeComparison.Tests.Integration.MatchbookTests
{
    public class MatchbookHandlerTests : AbstractTests
    {
        private readonly IMatchbookHandler _matchbookHandler;
        private readonly ITradingHandler _tradingHandler;
        private string _sessionToken;
        
        public MatchbookHandlerTests()
        {
            if (!Services.TryGetService(out _matchbookHandler)) throw new Exception($"No service.");
            if (!Services.TryGetService(out _tradingHandler)) throw new Exception($"No service.");

            Setup();
        }

        [SetUp]
        public void Setup()
        {
            Task.Run(() => GetSession()).Wait();
            _sessionToken = _matchbookHandler.SessionToken;
        }

        public async Task GetSession()
        {
            await _matchbookHandler.GetSessionToken();

            Assert.NotNull(_matchbookHandler.SessionToken);
            Assert.IsNotEmpty(_matchbookHandler.SessionToken);   
        }

        [Test]
        public async Task GetEventsTest()
        {
            for (int i = 0; i < 10; i++)
            {
                await GetEvents();
                Thread.Sleep(5000);
            }

            var events = await GetEvents();
        }

        [Test]
        public async Task GetOffersTest()
        {
            var events = await GetEvents();
            var marketIds = events
                .SelectMany(e => e.Markets)
                .Select(m => m.Id)
                .ToArray();

            for (int i = 0; i < 10; i++)
            {
                await GetOffers(marketIds);
                Thread.Sleep(3000);
            }

            var offers = await GetOffers(marketIds);
        }

        #region Helpers
        private async Task<List<MatchbookEvent>> GetEvents()
        {
            var events = await _matchbookHandler.GetEvents();
            Assert.NotNull(events);
            Assert.IsNotEmpty(events);
            return events;
        }
        private async Task<List<Offer>> GetOffers(long[] marketIds)
        {
            var offers = await _matchbookHandler.GetOffers(marketIds);
            Assert.NotNull(offers);
            return offers;
        }
        #endregion
    }
}