using Betfair.ExchangeComparison.Auth.Interfaces;
using Betfair.ExchangeComparison.Auth.Settings;
using Betfair.ExchangeComparison.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Betfair.ExchangeComparison.Auth
{
    public class AuthHandler : IAuthHandler
    {
        private readonly IAuthClient _authClient;
        private readonly IOptions<LoginSettings> _logins;

        public Dictionary<Bookmaker, string> SessionTokens { get; private set; }
        public Dictionary<Bookmaker, DateTime> TokenExpiries { get; set; }
        public string AppKey { get; private set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public AuthHandler(IAuthClient authClient, IOptions<LoginSettings> logins)
        {
            _authClient = authClient;
            _logins = logins;

            SessionTokens = new Dictionary<Bookmaker, string>();
            TokenExpiries = new Dictionary<Bookmaker, DateTime>();

            Username = Environment.GetEnvironmentVariable("USERNAME") != null ?
                Environment.GetEnvironmentVariable("USERNAME")! :
                logins.Value.USERNAME!;

            Password = Environment.GetEnvironmentVariable("PASSWORD") != null ?
                Environment.GetEnvironmentVariable("PASSWORD")! :
                logins.Value.PASSWORD!;

            AppKey = Environment.GetEnvironmentVariable("APP_KEY") != null ?
                Environment.GetEnvironmentVariable("APP_KEY")! :
                logins.Value.APP_KEY!;
        }

        public bool TryLogin(Bookmaker bookmaker)
        {
            if (!SessionValid(bookmaker))
            {
                if (!Login("", "", bookmaker))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Login(string username, string password, Bookmaker bookmaker)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = Username;
                }

                if (string.IsNullOrEmpty(password))
                {
                    password = Password;
                }

                var loginResult = _authClient.Login(
                    username,
                    password,
                    AppKey,
                    bookmaker);

                if (loginResult == null)
                {
                    Console.WriteLine($"LOGIN_FAILED; " +
                            $"Error=loginResult is null");

                    return false;
                }

                if (string.IsNullOrEmpty(loginResult.Token))
                {
                    if (!string.IsNullOrEmpty(loginResult.Error))
                    {
                        Console.WriteLine($"LOGIN_FAILED; " +
                            $"Error={loginResult.Error}");
                    }
                    else
                    {
                        Console.WriteLine($"LOGIN_FAILED; " +
                            $"Error=UNKNOWN");
                    }

                    return false;
                }

                if (SessionTokens.ContainsKey(bookmaker))
                {
                    SessionTokens[bookmaker] = loginResult.Token;
                }
                else
                {
                    SessionTokens.Add(bookmaker, loginResult.Token);
                }

                if (TokenExpiries.ContainsKey(bookmaker))
                {
                    TokenExpiries[bookmaker] = DateTime.UtcNow.AddHours(6);
                }
                else
                {
                    TokenExpiries.Add(bookmaker, DateTime.UtcNow.AddHours(6));
                }

                Console.WriteLine($"SESSION_TOKEN_RENEWED_{bookmaker.ToString().ToUpper()}; " +
                    $"ValidTo={TokenExpiries[bookmaker]
                        .ToString("dd-MM-yyyy HH:mm")}");

                return SessionValid(bookmaker);
            }
            catch (InvalidDataException ivlException)
            {
                Console.WriteLine($"LOGIN_FAIL_{bookmaker.ToString().ToUpper()}; " +
                    $"Exception={ivlException.Message}");

                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"LOGIN_FAIL_{bookmaker.ToString().ToUpper()}; " +
                    $"Exception={exception.Message}");

                return false;
            }
        }

        public bool SessionValid(Bookmaker bookmaker)
        {
            if (!SessionTokens.ContainsKey(bookmaker) ||
                !TokenExpiries.ContainsKey(bookmaker))
            {
                return false;
            }

            return !string.IsNullOrEmpty(SessionTokens[bookmaker]) &&
                 DateTime.UtcNow < TokenExpiries[bookmaker];
        }
    }
}



