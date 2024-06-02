namespace Betfair.ExchangeComparison.Domain.Interfaces.Matchbook
{
    public interface ISessionClient : IMatchbookClient
    {
        Task<string> PostSessionToken();
        Task<string> GetSessionToken(bool refreshSession = true);
        Task<string> DeleteSessionToken(string sessionToken);
    }
}
