using System;
using Betfair.ExchangeComparison.Domain.Definitions.Base;

namespace Betfair.ExchangeComparison.Domain.Definitions.Sport
{
    public class SportRacing : ISportBase
    {
        public SportRacing()
        {
        }

        protected override Enums.Sport SportType => Enums.Sport.Racing;
    }
}

