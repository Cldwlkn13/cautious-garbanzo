using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Domain.Interfaces.Matchbook
{
    public interface IAccountClient : IMatchbookClient
    {
        Task<Account> GetAccount();
        Task<BalanceResponse> GetBalance();
        public string SessionToken { get; set; }
    }
}
