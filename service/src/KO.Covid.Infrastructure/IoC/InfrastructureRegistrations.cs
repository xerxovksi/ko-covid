namespace KO.Covid.Infrastructure.IoC
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Infrastructure.ApplicationInsights;
    using KO.Covid.Infrastructure.Cache;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.ApplicationInsights;

    public static class InfrastructureRegistrations
    {
        public static void AddApplicationInsights(
            this IServiceCollection services,
            string instrumentationKey)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddScoped(typeof(ITelemetryLogger<>), typeof(ApplicationInsightsLogger<>));
            services.AddLogging(
                builder =>
                {
                    builder.AddApplicationInsights(instrumentationKey);
                    builder.AddFilter<ApplicationInsightsLoggerProvider>(
                        "",
                        LogLevel.Information);
                });
        }

        public static void AddRedisCache(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<IRedisConnection>(_ => new RedisConnection(connectionString));
            services.AddScoped(typeof(ICache<>), typeof(RedisCache<>));
        }
    }
}
