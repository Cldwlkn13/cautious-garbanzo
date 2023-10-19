﻿using Betfair.ExchangeComparison.Exchange.Clients;
using Betfair.ExchangeComparison.Exchange.Interfaces;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Exchange.Settings;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Exchange
{
    public class ExchangeHandler : IExchangeHandler
    {
        private readonly IAuthClient _authClient;
        private readonly IOptions<ExchangeSettings> _options;
        private readonly IOptions<LoginSettings> _logins;

        private IExchangeClient? _exchangeClient;

        public ExchangeHandler(IAuthClient authClient, IOptions<ExchangeSettings> options, IOptions<LoginSettings> logins)
        {
            _authClient = authClient;
            _options = options;
            _logins = logins;

            SessionToken = string.Empty;

            Username = Environment.GetEnvironmentVariable("USERNAME") != null ?
                Environment.GetEnvironmentVariable("USERNAME")! :
                logins.Value.USERNAME!;

            Password = Environment.GetEnvironmentVariable("PASSWORD") != null ?
                Environment.GetEnvironmentVariable("PASSWORD")! :
                logins.Value.PASSWORD!;

            AppKey = Environment.GetEnvironmentVariable("APP_KEY") != null ?
                Environment.GetEnvironmentVariable("APP_KEY")! :
                logins.Value.APP_KEY!;
        }

        public string SessionToken { get; private set; }
        public string AppKey { get; private set; }

        private string Username { get; set; }
        private string Password { get; set; }

        public bool Login(string username, string password)
        {
            var loginResult = _authClient.Login(Username, Password) ??
                throw new NullReferenceException($"Login Failed");

            SessionToken = loginResult.Token;

            _exchangeClient = new ExchangeClient(
                _options.Value.Url, AppKey, SessionToken);

            return !string.IsNullOrEmpty(SessionToken);
        }

        public bool SessionValid()
        {
            return !string.IsNullOrEmpty(SessionToken);
        }

        public IList<EventTypeResult> ListEventTypes()
        {
            var marketFilter = new MarketFilter();

            var eventTypes = _exchangeClient?.ListEventTypes(marketFilter) ??
                throw new NullReferenceException($"Event Types null.");

            return eventTypes;
        }

        public IList<MarketCatalogue> ListMarketCatalogues(string eventTypeId = "7")
        {
            var marketFilter = new MarketFilter();
            marketFilter.EventTypeIds = new List<string> { eventTypeId }.ToHashSet();

            var time = new TimeRange();
            time.From = DateTime.Now;

            switch (eventTypeId)
            {
                case "7":
                    marketFilter.MarketCountries = new HashSet<string>() { "GB", "IE" };
                    marketFilter.MarketTypeCodes = new HashSet<String>() { "WIN", "PLACE", "OTHER_PLACE" };
                    time.To = DateTime.Today.AddDays(1);
                    break;
                case "1":
                    marketFilter.MarketTypeCodes = new HashSet<String>() { "MATCH_ODDS", "OVER_UNDER_15", "OVER_UNDER_25", "OVER_UNDER_35", "BOTH_TEAMS_TO_SCORE" };
                    time.To = DateTime.Now.AddHours(6);
                    break;
            }

            marketFilter.MarketStartTime = time;
            var marketSort = MarketSort.FIRST_TO_START;
            var maxResults = "1000";

            //as an example we requested runner metadata 
            ISet<MarketProjection> marketProjections = new HashSet<MarketProjection>();
            marketProjections.Add(MarketProjection.EVENT);
            marketProjections.Add(MarketProjection.MARKET_DESCRIPTION);
            //marketProjections.Add(MarketProjection.RUNNER_METADATA);
            //marketProjections.Add(MarketProjection.RUNNER_DESCRIPTION);

            var marketCatalogues = _exchangeClient!.ListMarketCatalogue(
                marketFilter, marketProjections, marketSort, maxResults);

            return marketCatalogues;
        }

        public IList<MarketBook> ListMarketBooks(IList<string> marketIds)
        {
            //as an example we requested runner metadata 
            PriceProjection priceProjection = new PriceProjection()
            {
                PriceData = new HashSet<PriceData> { PriceData.EX_BEST_OFFERS, PriceData.EX_TRADED },
            };

            var marketBooks = _exchangeClient!.ListMarketBook(
                marketIds, priceProjection);

            return marketBooks;
        }
    }
}

