using System;
using Betfair.ExchangeComparison.Domain.Enums;

namespace Betfair.ExchangeComparison.Domain.CustomExceptions
{
    public class AuthException : Exception
    {
        private string CustomMessage = "LOGIN_FAILED;";

        public AuthException()
        {
        }

        public AuthException(Bookmaker bookmaker)
        {
            CustomMessage = $"LOGIN_FAILED_{bookmaker.ToString().ToUpper()}";
        }

        public AuthException(string msg)
        {
            CustomMessage = $"LOGIN_FAILED; Msg={msg}";
        }

        public override string Message => CustomMessage;
    }
}

