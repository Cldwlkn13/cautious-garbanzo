using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Betfair.ExchangeComparison.Exchange.Json;
using Betfair.ExchangeComparison.Exchange.Model;
using Betfair.ExchangeComparison.Sportsbook.Interfaces;
using Betfair.ExchangeComparison.Sportsbook.Model;

namespace Betfair.ExchangeComparison.Sportsbook.Clients
{
    public class SportsbookClient : HttpClient, ISportsbookClient
    {
        public string EndPoint { get; private set; }
        private static readonly IDictionary<string, Type> operationReturnTypeMap = new Dictionary<string, Type>();
        public const string APPKEY_HEADER = "X-Application";
        public const string SESSION_TOKEN_HEADER = "X-Authentication";
        public WebHeaderCollection CustomHeaders { get; set; }
        private static readonly string LIST_EVENT_TYPES_METHOD = "listEventTypes";
        private static readonly string LIST_COMPETITIONS_METHOD = "listCompetitions";
        private static readonly string LIST_EVENTS_METHOD = "listEvents";
        private static readonly string LIST_MARKET_TYPES_METHOD = "listMarketTypes";
        private static readonly string LIST_MARKET_CATALOGUE_METHOD = "listMarketCatalogue";
        private static readonly string LIST_MARKET_BOOK_METHOD = "listMarketBook";
        private static readonly string LIST_MARKET_PRICES_METHOD = "listMarketPrices";
        private static readonly string PLACE_ORDERS_METHOD = "SportsAPING/v1.0/placeOrders";
        private static readonly string LIST_MARKET_PROFIT_AND_LOST_METHOD = "SportsAPING/v1.0/listMarketProfitAndLoss";
        private static readonly string LIST_CURRENT_ORDERS_METHOD = "SportsAPING/v1.0/listCurrentOrders";
        private static readonly string LIST_CLEARED_ORDERS_METHOD = "SportsAPING/v1.0/listClearedOrders";
        private static readonly string CANCEL_ORDERS_METHOD = "SportsAPING/v1.0/cancelOrders";
        private static readonly string REPLACE_ORDERS_METHOD = "SportsAPING/v1.0/replaceOrders";
        private static readonly string UPDATE_ORDERS_METHOD = "SportsAPING/v1.0/updateOrders";
        private static readonly string GET_ACCOUNT_FUNDS_METHOD = "AccountAPING/v1.0/getAccountFunds";
        private static readonly String FILTER = "filter";
        private static readonly String LOCALE = "locale";
        private static readonly String WALLET = "wallet";
        private static readonly String CURRENCY_CODE = "currencyCode";
        private static readonly String MARKET_PROJECTION = "marketProjection";
        private static readonly String MATCH_PROJECTION = "matchProjection";
        private static readonly String ORDER_PROJECTION = "orderProjection";
        private static readonly String PRICE_PROJECTION = "priceProjection";
        private static readonly String SORT = "sort";
        private static readonly String MAX_RESULTS = "maxResults";
        private static readonly String MARKET_IDS = "marketIds";
        private static readonly String MARKET_ID = "marketId";
        private static readonly String INSTRUCTIONS = "instructions";
        private static readonly String CUSTOMER_REFERENCE = "customerRef";
        private static readonly String INCLUDE_SETTLED_BETS = "includeSettledBets";
        private static readonly String INCLUDE_BSP_BETS = "includeBspBets";
        private static readonly String NET_OF_COMMISSION = "netOfCommission";
        private static readonly String BET_IDS = "betIds";
        private static readonly String PLACED_DATE_RANGE = "placedDateRange";
        private static readonly String ORDER_BY = "orderBy";
        private static readonly String SORT_DIR = "sortDir";
        private static readonly String FROM_RECORD = "fromRecord";
        private static readonly String RECORD_COUNT = "recordCount";
        private static readonly string BET_STATUS = "betStatus";
        private static readonly string EVENT_TYPE_IDS = "eventTypeIds";
        private static readonly string EVENT_IDS = "eventIds";
        private static readonly string RUNNER_IDS = "runnerIds";
        private static readonly string SIDE = "side";
        private static readonly string SETTLED_DATE_RANGE = "settledDateRange";
        private static readonly string GROUP_BY = "groupBy";
        private static readonly string INCLUDE_ITEM_DESCRIPTION = "includeItemDescription";

        public SportsbookClient()
        {
        }

        public SportsbookClient(string endPoint, string appKey, string sessionToken)
        {
            EndPoint = endPoint + "/rest/v1.0/";

            base.BaseAddress = new Uri(EndPoint);

            base.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (appKey != null)
            {
                base.DefaultRequestHeaders.TryAddWithoutValidation(APPKEY_HEADER, appKey);
            }
            if (sessionToken != null)
            {
                base.DefaultRequestHeaders.TryAddWithoutValidation(SESSION_TOKEN_HEADER, sessionToken);
            }
        }

        public T Invoke<T>(string method, object obj)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.Length == 0)
                throw new ArgumentException(null, "method");

            var body = JsonConvert.Serialize(obj);

            Console.WriteLine("Calling Sportsbook: " + method + " With body: " + body);

            HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            ServicePointManager.Expect100Continue = false;

            var response = base.PostAsync(method + "/", content).Result;

            var responseContent = response.Content.ReadAsStringAsync().Result;

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);

            return result;
        }

        public IEnumerable<EventTypeResult> ListEventTypes(MarketFilter marketFilter, string? locale = null)
        {
            var obj = new
            {
                listEventTypesRequestParams = new ListEventTypesRequestParams()
                {
                    MarketFilter = new SportsbookMarketFilter()
                }
            };

            return Invoke<List<EventTypeResult>>(LIST_EVENT_TYPES_METHOD, obj);
        }

        public IEnumerable<CompetitionResult> ListCompetitions(string eventTypeId, TimeRange timeRange, string locale = null)
        {
            var obj = new
            {
                listCompetitionsRequestParams = new ListCompetitionsRequestParams()
                {
                    MarketFilter = new SportsbookMarketFilter()
                    {
                        EventTypeIds = new HashSet<string>() { eventTypeId },
                        TimeRange = timeRange
                    }
                }
            };

            return Invoke<List<CompetitionResult>>(LIST_COMPETITIONS_METHOD, obj);
        }

        public IEnumerable<EventResult> ListEventsByEventType(string eventTypeId, TimeRange timeRange)
        {
            var obj = new
            {
                listEventsRequestParams = new ListEventsRequestParams()
                {
                    MarketFilter = new SportsbookMarketFilter()
                    {
                        EventTypeIds = new HashSet<string>() { eventTypeId },
                        TimeRange = timeRange
                    }
                }
            };

            return Invoke<List<EventResult>>(LIST_EVENTS_METHOD, obj);
        }

        public IEnumerable<EventResult> ListEventsByCompetition(Competition competition, TimeRange timeRange)
        {
            var obj = new
            {
                listEventsRequestParams = new ListEventsRequestParams()
                {
                    MarketFilter = new SportsbookMarketFilter()
                    {
                        CompetitionIds = new HashSet<string>() { competition.Id },
                        TimeRange = timeRange
                    }
                }
            };

            return Invoke<List<EventResult>>(LIST_EVENTS_METHOD, obj);
        }

        public MarketDetails ListMarketPrices(IEnumerable<string> marketIds)
        {
            MarketDetails marketDetails = new MarketDetails() { marketDetails = new List<MarketDetail>() };

            foreach (var batch in marketIds.Chunk(100))
            {
                var obj = new
                {
                    listMarketPricesRequestParams = new ListMarketPricesRequestParams()
                    {
                        MarketIds = batch
                    },
                };

                var batchResult = Invoke<MarketDetails>(LIST_MARKET_PRICES_METHOD, obj);

                foreach (var md in batchResult.marketDetails)
                {
                    marketDetails.marketDetails.Add(md);
                }
            }

            return marketDetails;
        }

        public IEnumerable<MarketCatalogue> ListMarketCatalogue(SportsbookMarketFilter marketFilter, string maxResults = "1", string? locale = null)
        {
            var obj = new
            {
                listMarketCatalogueRequestParams = new ListMarketCatalogueRequestParams()
                {
                    MarketFilter = marketFilter,
                    MaxResults = maxResults
                },
            };

            return Invoke<List<MarketCatalogue>>(LIST_MARKET_CATALOGUE_METHOD, obj);
        }

        public IEnumerable<MarketTypeResult> ListMarketTypes(string eventTypeId)
        {
            var obj = new
            {
                listMarketTypesRequestParams = new ListMarketTypesRequestParams()
                {
                    MarketFilter = new SportsbookMarketFilter()
                    {
                        EventTypeIds = new HashSet<string>() { eventTypeId },
                    }
                }
            };

            return Invoke<List<MarketTypeResult>>(LIST_MARKET_TYPES_METHOD, obj);
        }

        private static System.Exception ReconstituteException(Betfair.ExchangeComparison.Exchange.Model.Exception ex)
        {
            var data = ex.Data;

            // API-NG exception -- it must have "data" element to tell us which exception
            var exceptionName = data.Property("exceptionname").Value.ToString();
            var exceptionData = data.Property(exceptionName).Value.ToString();
            return JsonConvert.Deserialize<APINGException>(exceptionData);
        }


    }
}

