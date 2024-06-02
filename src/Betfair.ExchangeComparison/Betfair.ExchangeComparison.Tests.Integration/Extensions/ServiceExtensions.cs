namespace Betfair.ExchangeComparison.Tests.Integration.Extensions
{
    public static class ServiceExtensions
    {
        public static bool TryGetService<T, V>(this IServiceProvider services, out V service)
        {
            service = (V)services.GetService(typeof(T));
            return service != null;
        }

        public static bool TryGetService<T>(this IServiceProvider services, out T service)
        {
            service = (T)services.GetService(typeof(T));
            return service != null;
        }
    }
}
