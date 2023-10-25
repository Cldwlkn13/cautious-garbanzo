using System;
using Betfair.ExchangeComparison.Domain.Enums;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Auth.Interfaces
{
    public interface IAuthClient
    {
        KeepAliveLogoutResponse Login(string username, string password, string appKey, Bookmaker bookmaker = Bookmaker.BetfairSportsbook);
    }
}


