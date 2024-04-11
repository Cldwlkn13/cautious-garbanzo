using Betfair.ExchangeComparison.Domain.Matchbook;

namespace Betfair.ExchangeComparison.Matchbook.Interfaces
{
    public interface IAccountClient : IMatchbookClient
    {
        Task<Account> GetAccount(string sessionToken);
        Task<BalanceResponse> GetBalance(string sessionToken);
    }
}
