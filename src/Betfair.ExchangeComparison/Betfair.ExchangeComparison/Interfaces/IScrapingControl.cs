using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingControl
    {
        void Start(Bookmaker bookmaker);
        void Stop(Bookmaker bookmaker);
        public Dictionary<Bookmaker, bool> SwitchBoard { get; }
    }
}

