using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.Interfaces.Matchbook
{
    public interface IReportsClient : IMatchbookClient
    {
        public string SessionToken { get; set; }

        Task<SettledBetsResponse> GetSettledBets(string sportId, string beforeEpoch, string afterEpoch, int offset);
    }
}
