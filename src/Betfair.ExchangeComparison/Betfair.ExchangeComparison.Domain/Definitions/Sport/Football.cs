using System;
using Betfair.ExchangeComparison.Domain.Definitions.Base;

namespace Betfair.ExchangeComparison.Domain.Definitions.Sport
{
    public class SportFootball : ISportBase
    {
        public SportFootball()
        {
        }

        protected override Enums.Sport SportType => Enums.Sport.Football;
    }
}

