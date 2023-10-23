using System;
using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IPricingComparisonHandler
    {
        List<BestRunner> TryAddToBestRunnersWinOnly(List<BestRunner> bestRunners, RunnerPriceOverview rpo);
        List<BestRunner> TryAddToBestRunnersEachWay(List<BestRunner> bestRunners, RunnerPriceOverview rpo);
    }
}

