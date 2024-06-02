using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Domain.Settings;
using Betfair.ExchangeComparison.Interfaces;
using Betfair.ExchangeComparison.Pages.Models;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Handlers
{
    public class PriceComparisonHandler : IPricingComparisonHandler
    {
        private readonly IOptions<PriceComparisonSettings> _settings;
        private readonly IExpectedValueModelFlat _expectedValueModelFlat;
        private readonly IExpectedValueModelJumps _expectedValueModelJumps;

        public PriceComparisonHandler(IOptions<PriceComparisonSettings> settings, IExpectedValueModelFlat expectedValueModelFlat,
            IExpectedValueModelJumps expectedValueModelJumps)
        {
            _settings = settings;
            _expectedValueModelFlat = expectedValueModelFlat;
            _expectedValueModelJumps = expectedValueModelJumps;
        }

        public List<BestRunner> TryAddToBestRunnersWinOnly(List<BestRunner> bestRunners, RunnerPriceOverview rpo)
        {
            if (rpo.ExpectedValueWin > _settings.Value.MinExpectedValueWin && 
                rpo.ExpectedWinPrice > 1)
            {
                var modelParameters = new FlatParams(rpo);

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
                double expectedPrice = 0;

                if (rpo.MarketCatalogue.MarketName.Contains("Hrd") || 
                    rpo.MarketCatalogue.MarketName.Contains("Chs") || 
                    rpo.MarketCatalogue.MarketName.Contains("NHF"))
                {
                    var modelParameters = new JumpsParams(rpo);

                    expectedPrice = _expectedValueModelJumps.ModelOutcome(
                        modelParameters, rpo);
                }
                else
                {
                    var modelParameters = new FlatParams(rpo);

                    expectedPrice = _expectedValueModelFlat.ModelOutcome(
                        modelParameters, rpo);
                }
                
                result = new BestRunner(rpo);
                result.ExpectedPrice = expectedPrice;
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

