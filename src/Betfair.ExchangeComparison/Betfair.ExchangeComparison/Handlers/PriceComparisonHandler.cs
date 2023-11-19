using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;
using Betfair.ExchangeComparison.Settings;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Handlers
{
    public class PriceComparisonHandler : IPricingComparisonHandler
    {
        private readonly IOptions<PriceComparisonSettings> _settings;
        
        public PriceComparisonHandler(IOptions<PriceComparisonSettings> settings)
        {
            _settings = settings;
        }

        public List<BestRunner> TryAddToBestRunnersWinOnly(List<BestRunner> bestRunners, RunnerPriceOverview rpo)
        {
            if (rpo.ExpectedValueWin > _settings.Value.MinExpectedValueWin && 
                rpo.ExpectedWinPrice > 1)
            {
                bestRunners.Add(new BestRunner(rpo));
            }

            return bestRunners;
        }

        public List<BestRunner> TryAddToBestRunnersEachWay(List<BestRunner> bestRunners, RunnerPriceOverview rpo)
        {
            if (rpo.ExpectedValueEachWay > _settings.Value.MinExpectedValueEachWay && 
                rpo.NumberEachWayPlaces > 1 && 
                rpo.ExpectedWinPrice > 1)
            {
                bestRunners.Add(new BestRunner(rpo));
            }

            return bestRunners;
        }

        public bool AssessIsBestRunnerWinOnly(RunnerPriceOverview rpo, out BestRunner result)
        {
            result = new BestRunner();
            
            if (rpo.ExpectedValueWin > _settings.Value.MinExpectedValueWin && 
                rpo.ExpectedWinPrice > 1)
            {
                result = new BestRunner(rpo);
                return true;
            }

            return false;
        }


        public bool AssessIsBestRunnerEachWay(RunnerPriceOverview rpo, out BestRunner result)
        {
            result = new BestRunner();

            if (rpo.ExpectedValueEachWay > _settings.Value.MinExpectedValueEachWay && 
                rpo.NumberEachWayPlaces > 1 && 
                rpo.ExpectedWinPrice > 1)
            {
                result = new BestRunner(rpo);
                return true;
            }

            return false;
        }
    }
}

