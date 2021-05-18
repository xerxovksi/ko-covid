namespace KO.Covid.Infrastructure.IoC
{
    using KO.Covid.Application.Contracts;
    using KO.Covid.Infrastructure.ApplicationInsights;
    using KO.Covid.Infrastructure.Cosmos;
    using KO.Covid.Infrastructure.Mail;
    using KO.Covid.Infrastructure.Redis;
    using KO.Covid.Infrastructure.Subscriber;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.ApplicationInsights;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;

    public static class InfrastructureRegistrations
    {
        public static IServiceCollection AddApplicationInsights(
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

            return services;
        }

        public static IServiceCollection AddRedis(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<IRedisConnection>(_ => new RedisConnection(connectionString));
            services.AddScoped(typeof(ICache<>), typeof(RedisCache<>));

            return services;
        }

        public static IServiceCollection AddCosmos(
            this IServiceCollection services,
            string cosmosEndpoint,
            string cosmosAuthKey,
            string databaseId,
            string containerId)
        {
            services.AddSingleton((provider) =>
                new CosmosClient(
                    cosmosEndpoint,
                    cosmosAuthKey,
                    new CosmosClientOptions
                    {
                        SerializerOptions = new CosmosSerializationOptions
                        {
                            IgnoreNullValues = true,
                            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                        }
                    })
            );

            services.AddSingleton<ICosmosClientFactory>((provider) =>
                new CosmosClientFactory(
                    databaseId,
                    new List<string> { containerId },
                    provider.GetRequiredService<CosmosClient>()));

            return services;
        }

        public static IServiceCollection AddMailNotifier(
            this IServiceCollection services,
            string username,
            string password)
        {
            services.AddSingleton(
                new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                });

            services.AddScoped<INotifier>(provider =>
                new GmailNotifier(provider.GetRequiredService<SmtpClient>(), username));

            return services;
        }

        public static IServiceCollection AddSubscriberRepository(
            this IServiceCollection services,
            string containerId)
        {
            services.AddSingleton(new SubscriberContainer(containerId));
            services.AddScoped(typeof(IRepository<>), typeof(SubscriberRepository<>));

            return services;
        }
    }
}
