using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Interfaces
{
    public interface IScrapingControl
    {
        void Start(Provider provider);
        void Stop(Provider provider);
        public Dictionary<Provider, bool> SwitchBoard { get; }
    }
}

