using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KO.Covid.Subscriber.Startup))]
namespace KO.Covid.Subscriber
{
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using KO.Covid.Application;
    using KO.Covid.Infrastructure.IoC;
    using KO.Covid.Subscriber.IoC;
    using MediatR;
    using MediatR.Extensions.FluentValidation.AspNetCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class Startup : FunctionsStartup
    {
        public IConfiguration Configuration { get; }

        public JsonSerializerSettings JsonSerializerSettings { get; }

        public Startup()
        {
            var localConfiguration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var secretClient = new SecretClient(
                new UriBuilder(localConfiguration["KEY_VAULT_URI"]).Uri,
                new ChainedTokenCredential(
                    new DefaultAzureCredential(),
                    new EnvironmentCredential()));

            this.Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddAzureKeyVault(
                    secretClient,
                    new AzureKeyVaultConfigurationOptions
                    {
                        ReloadInterval = TimeSpan.FromHours(24)
                    })
                .Build();

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(new CamelCaseNamingStrategy())
                },
                NullValueHandling = NullValueHandling.Ignore
            };

            JsonConvert.DefaultSettings = () => serializerSettings;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            builder.Services.AddMediatR(assembly);
            builder.Services.AddFluentValidation(new[] { assembly });

            builder.Services.AddSingleton(this.Configuration);
            builder.Services.AddScoped<Correlator>();

            builder.Services.AddApplicationInsights(this.Configuration["KOCInstrumentationKey"]);
            builder.Services.AddRedis(this.Configuration["KOCCacheConnectionString"]);
            builder.Services.AddCosmos(
                this.Configuration["KOCCosmosEndpoint"],
                this.Configuration["KOCCosmosAuthKey"],
                this.Configuration["COSMOS_DATABASE_ID"],
                this.Configuration["COSMOS_SUBSCRIBER_CONTAINER_ID"]);

            builder.Services.AddSubscriberRepository(
                this.Configuration["COSMOS_SUBSCRIBER_CONTAINER_ID"]);

            builder.Services.AddMailNotifier(
                this.Configuration["KOCMailUsername"],
                this.Configuration["KOCMailPassword"]);

            builder.Services.AddHandlers(this.Configuration["COWIN_BASE_ADDRESS"]);
        }
    }
}
