using Betfair.ExchangeComparison.Domain.DomainModel;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IExpectedValueModel
    {
        double ModelOutcome<T>(T parameters, RunnerPriceOverview rpo);
    }
}
