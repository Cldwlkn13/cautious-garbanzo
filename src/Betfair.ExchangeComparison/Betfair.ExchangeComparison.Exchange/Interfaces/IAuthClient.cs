using System;
using Betfair.ExchangeComparison.Exchange.Model;

namespace Betfair.ExchangeComparison.Exchange.Interfaces
{
    public interface IAuthClient
    {
        KeepAliveLogoutResponse Login(string username, string password);
    }
}

