﻿using System;
namespace Betfair.ExchangeComparison.Domain.DomainModel
{
    public struct Fraction
    {
        public Fraction(int n, int d)
        {
            N = n;
            D = d;
        }

        public int N { get; private set; }
        public int D { get; private set; }
    }
}

