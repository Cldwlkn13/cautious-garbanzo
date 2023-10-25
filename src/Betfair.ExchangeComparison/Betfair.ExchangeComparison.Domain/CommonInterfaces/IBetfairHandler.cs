using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Domain.CommonInterfaces
{
    public interface IBetfairHandler
    {
        bool TryLogin();
        bool Login(string username = "", string password = "");
        bool SessionValid();
    }
}

