using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KO.Covid.Subscriber.Startup))]
namespace KO.Covid.Subscriber
{
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using KO.Covid.Infrastructure.IoC;
    using KO.Covid.Subscriber.IoC;
    using MediatR;
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

            var keyVaultClient = new SecretClient(
                new UriBuilder(localConfiguration["KEY_VAULT_URI"]).Uri,
                new EnvironmentCredential());

            this.Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddAzureKeyVault(
                    keyVaultClient,
                    new KeyVaultSecretManager())
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

            builder.Services.AddSingleton(this.Configuration);
            builder.Services.AddApplicationInsights(this.Configuration["KOCInstrumentationKey"]);

            builder.Services.AddHandlers();
        }
    }
}
