namespace KO.Covid.Api
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using KO.Covid.Api.Filters;
    using KO.Covid.Application;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Infrastructure.ApplicationInsights;
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
    using Microsoft.Extensions.Logging.ApplicationInsights;
    using Microsoft.IdentityModel.Tokens;
    using System.Reflection;
    using System.Text.Json.Serialization;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public Startup()
        {
            this.Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
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

            services.AddApplicationInsightsTelemetry();
            services.AddScoped(typeof(ITelemetryLogger<>), typeof(ApplicationInsightsLogger<>));
            services.AddLogging(
                builder =>
                {
                    builder.AddApplicationInsights(
                        this.Configuration["KOCInstrumentationKey"]);

                    builder.AddFilter<ApplicationInsightsLoggerProvider>(
                        "",
                        LogLevel.Information);
                });

            services.AddOptions();

            var assembly = typeof(Startup).GetTypeInfo().Assembly;

            services.AddMediatR(assembly);
            services.AddFluentValidation(new[] { assembly });

            services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.Authority = this.Configuration["KOCIdentityAuthority"];
                   options.Audience = this.Configuration["KOCIdentityAudience"];
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidIssuer = this.Configuration["KOCIdentityIssuer"],
                       ValidAudience = this.Configuration["KOCIdentityAudience"],
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true
                   };
               });
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
                policy.WithOrigins(this.Configuration["AllowedHosts"].Split(";"));
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
        }
    }
}
