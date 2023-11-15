using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Handlers
{
    public class PriceComparisonHandler : IPricingComparisonHandler
    {
        public PriceComparisonHandler()
        {
        }

        public List<BestRunner> TryAddToBestRunnersWinOnly(List<BestRunner> bestRunners, RunnerPriceOverview rpo)
        {
            if (rpo.ExpectedValueWin > -0.04 && rpo.ExpectedWinPrice > 1)
            {
                bestRunners.Add(new BestRunner(rpo));
            }

            return bestRunners;
        }

        public List<BestRunner> TryAddToBestRunnersEachWay(List<BestRunner> bestRunners, RunnerPriceOverview rpo)
        {
            if (rpo.ExpectedValueEachWay > 0.96 && rpo.NumberEachWayPlaces > 1 && rpo.ExpectedWinPrice > 1)
            {
                bestRunners.Add(new BestRunner(rpo));
            }

            return bestRunners;
        }
    }
}

