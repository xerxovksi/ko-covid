namespace KO.Covid.Api
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using KO.Covid.Api.Filters;
    using KO.Covid.Api.IoC;
    using KO.Covid.Application;
    using KO.Covid.Infrastructure.IoC;
    using MediatR;
    using MediatR.Extensions.FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.Json;
    using JsonStringEnumConverter = System.Text.Json.Serialization.JsonStringEnumConverter;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

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
                        ReloadInterval = TimeSpan.FromDays(1)
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(_ => this.Configuration);

            services.AddScoped<Correlator>();
            services
                .AddControllers(configuration =>
                    configuration.Filters.Add(typeof(CorrelationActionFilter)))
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase;

                    option.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());

                    option.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddOptions();

            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            services.AddMediatR(assembly);
            services.AddFluentValidation(new[] { assembly });

            services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer();

            services.AddApplicationInsights(this.Configuration["KOCInstrumentationKey"]);
            services.AddRedis(this.Configuration["KOCCacheConnectionString"]);
            services.AddCosmos(
                this.Configuration["KOCCosmosEndpoint"],
                this.Configuration["KOCCosmosAuthKey"],
                this.Configuration["COSMOS_DATABASE_ID"],
                this.Configuration["COSMOS_SUBSCRIBER_CONTAINER_ID"]);
            
            services.AddSubscriberRepository(this.Configuration["COSMOS_SUBSCRIBER_CONTAINER_ID"]);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(policy =>
            {
                policy.WithOrigins(this.Configuration["ALLOWED_HOSTS"].Split(";"));
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new RequestHandlerModule(this.Configuration["COWIN_BASE_ADDRESS"]));
        }
    }
}
