namespace Betfair.ExchangeComparison.Matchbook.Interfaces
{
    public interface ISessionClient : IMatchbookClient
    {
        Task<string> PostSessionToken();
        Task<string> GetSessionToken(bool refreshSession = true);
        Task<string> DeleteSessionToken(string sessionToken);
    }
}
