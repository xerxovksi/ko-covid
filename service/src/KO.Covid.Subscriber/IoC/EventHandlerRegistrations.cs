namespace KO.Covid.Subscriber.IoC
{
    using KO.Covid.Application;
    using KO.Covid.Application.Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using System.Net.Http;

    public static class EventHandlerRegistrations
    {
        public static void AddHandlers(this IServiceCollection services)
        {
            services.AddScoped<IEventMediator<bool>, EventMediator>();
            services.AddSingleton(_ => new HttpClient());
        }
    }
}
