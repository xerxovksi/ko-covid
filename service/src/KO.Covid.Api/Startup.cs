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
    using System.Text.Json.Serialization;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

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
                    option.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());
                });

            services.AddOptions();

            var assembly = typeof(Startup).GetTypeInfo().Assembly;

            services.AddMediatR(assembly);
            services.AddFluentValidation(new[] { assembly });

            services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer();

            services.AddApplicationInsights(this.Configuration["KOCInstrumentationKey"]);
            services.AddRedisCache(this.Configuration["KOCCacheConnectionString"]);
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

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            serializerSettings.Converters.Add(
                new StringEnumConverter(
                    new CamelCaseNamingStrategy()));

            JsonConvert.DefaultSettings = () => serializerSettings;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new RequestHandlerModule(this.Configuration["COWIN_BASE_ADDRESS"]));
        }
    }
}
