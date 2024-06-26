﻿using Betfair.ExchangeComparison.Domain.DomainModel;
using Betfair.ExchangeComparison.Pages.Models;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IPricingComparisonHandler
    {
        List<BestRunner> TryAddToBestRunnersWinOnly(List<BestRunner> bestRunners, RunnerPriceOverview rpo);
        List<BestRunner> TryAddToBestRunnersEachWay(List<BestRunner> bestRunners, RunnerPriceOverview rpo);

        bool AssessIsBestRunnerWinOnly(RunnerPriceOverview rpo, out BestRunner result);
        bool AssessIsBestRunnerEachWay(RunnerPriceOverview rpo, out BestRunner result);
    }
}

