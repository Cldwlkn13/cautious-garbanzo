using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Auth.Interfaces
{
    public interface IAuthHandler
    {
        bool TryLogin(Bookmaker bookmaker);
        bool Login(string username, string password, Bookmaker bookmaker);
        bool SessionValid(Bookmaker bookmaker);

        public Dictionary<Bookmaker, string> SessionTokens { get; }
        public string AppKey { get; }
    }
}

