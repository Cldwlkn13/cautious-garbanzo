using Betfair.ExchangeComparison.Domain.Matchbook;
using Betfair.ExchangeComparison.Domain.Matchbook.Requests;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class TradingExtensions
    {
        public static OffersRequest NewOffersRequest(long id, float stake, float odds, bool splitStakes = false)
        {
            var offerRequests = new List<OfferRequest>();

            if (splitStakes)
            {
                float upperOdds = (float)MatchbookOddsLadder.TickToPrice[MatchbookOddsLadder.PriceToTick[Math.Round(odds ,2)] + 1];

                var offer1 = new OfferRequest
                {
                    RunnerId = id,
                    Side = "back",
                    Odds = odds,
                    Stake = stake * 0.4f,
                    KeepInPlay = false
                };
                var offer2 = new OfferRequest
                {
                    RunnerId = id,
                    Side = "back",
                    Odds = upperOdds,
                    Stake = stake * 0.6f,
                    KeepInPlay = false
                };

                offerRequests = new List<OfferRequest> { offer1, offer2 };
            }
            else
            {
                offerRequests = new List<OfferRequest> {
                                    new OfferRequest
                                    {
                                        RunnerId = id,
                                        Side = "back",
                                        Odds = odds,
                                        Stake = stake,
                                        KeepInPlay = false
                                    }
                                };
            }
            


            return new OffersRequest
            {
                Offers = offerRequests
            };
        }

        public static string FiltersLogRunner(this BestRunner bestRunner)
        {
            return  $"/ {bestRunner.Event.Venue} {bestRunner.MarketDetail.marketStartTime.ConvertUtcToBritishIrishLocalTime().ToString("HH:mm")} " +
                    $"/ {bestRunner} " +
                    $"[{bestRunner.SportsbookRunner.winRunnerOdds.@decimal:F}] -";
        }

        public static HttpClient HandleSessionTokenHeader(this HttpClient client, string sessionToken)
        {
            if (!client.DefaultRequestHeaders.Contains("session-token"))
            {
                client.DefaultRequestHeaders.Add("session-token", sessionToken);
            }
            else if (!string.IsNullOrEmpty(sessionToken))
            {
                client.DefaultRequestHeaders.Remove("session-token");
                client.DefaultRequestHeaders.Add("session-token", sessionToken);
            }
            return client;
        }

        public static IEnumerable<long> AllMarketIds(this CatalogViewModel cvm)
        {
            return cvm.Markets
                .Where(m => m.MappedMatchbookEvent != null && m.MappedMatchbookEvent.Id != default)
                .Select(m => m.MappedMatchbookEvent.WinMarket().Id)
                .ToArray();
        }

        public static RunnerViewModel? MapOfferToRunner(this Offer offer, CatalogViewModel cvm)
        {
            var mappedEvent = cvm.Markets
                 .FirstOrDefault(m => m.MappedMatchbookEvent?.Id == offer.EventId);

            var runners = mappedEvent?.Runners;

            return runners?.FirstOrDefault(r => 
            r.SportsbookRunner?.selectionName == 
            offer.RunnerName
                .CleanRunnerName()
                .Replace("'", ""));
        }

        public static double GetCurrentPrice(this Offer offer, CatalogViewModel cvm)
        {
            var currentRunner = offer.MapOfferToRunner(cvm);

            if (currentRunner == null) return 0;
                
            var price = currentRunner.ExchangeWinRunner?.ExchangePrices?.AvailableToLay[0]?.Price;

            if(price == null) return 0;

            return price.Value;
        }

        public static double LatestWeightedAveragePrice(this Offer offer, CatalogViewModel cvm)
        {
            var currentRunner = offer.MapOfferToRunner(cvm);

            if (currentRunner == null) return 0;

            var wap = currentRunner.ExchangeWinRunner?.WeightedAverage();

            if (wap == null) return 0;

            return wap.Value;
        }
    }
}
