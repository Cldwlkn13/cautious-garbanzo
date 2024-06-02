namespace Betfair.ExchangeComparison.Domain.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WriteLine(string message, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLine(string message, ConsoleColor colour, List<string>? LoggingStore)
        {
            if (LoggingStore == null || LoggingStore.Contains(message)) return;
            LoggingStore.Add(message);

            Console.ForegroundColor = colour;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
