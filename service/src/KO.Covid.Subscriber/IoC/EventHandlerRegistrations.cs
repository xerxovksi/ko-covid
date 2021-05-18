namespace KO.Covid.Subscriber.IoC
{
    using FluentValidation;
    using KO.Covid.Application;
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Geo;
    using KO.Covid.Application.Models;
    using KO.Covid.Application.Subscriber;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Net.Http;

    public static class EventHandlerRegistrations
    {
        public static IServiceCollection AddHandlers(
            this IServiceCollection services,
            string baseAddress)
        {
            services.AddScoped<IEventMediator<bool>, EventMediator>();

            services.AddSingleton(_ => new HttpClient());

            services.AddSubscriberHandlers();
            services.AddGeoHandlers(baseAddress);
            services.AddAppointmentHandlers(baseAddress);

            return services;
        }

        private static void AddSubscriberHandlers(this IServiceCollection services)
        {
            services.AddScoped(
                typeof(IRequestHandler<GetActiveSubscribersQuery, List<Subscriber>>),
                typeof(GetActiveSubscribersQueryHandler));

            services.AddScoped(
                typeof(IRequestHandler<UpdateSubscriberCommand, Subscriber>),
                typeof(UpdateSubscriberCommandHandler));

            services.AddScoped<IValidator<UpdateSubscriberCommand>>(
                _ => new UpdateSubscriberCommandValidator());
        }

        private static void AddGeoHandlers(
            this IServiceCollection services,
            string baseAddress)
        {
            services.AddScoped(
                typeof(IRequestHandler<GetStatesQuery, StatesResponse>),
                provider => new GetStatesQueryHandler(
                    credentialCache: provider.GetRequiredService<ICache<Credential>>(),
                    statesCache: provider.GetRequiredService<ICache<StatesResponse>>(),
                    geoClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));

            services.AddScoped(
                typeof(IRequestHandler<GetDistrictsQuery, DistrictsResponse>),
                provider => new GetDistrictsQueryHandler(
                    mediator: provider.GetRequiredService<IMediator>(),
                    credentialCache: provider.GetRequiredService<ICache<Credential>>(),
                    statesCache: provider.GetRequiredService<ICache<StatesResponse>>(),
                    districtsCache: provider.GetRequiredService<ICache<DistrictsResponse>>(),
                    geoClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));
        }

        private static void AddAppointmentHandlers(
            this IServiceCollection services,
            string baseAddress)
        {
            services.AddScoped(
                typeof(IRequestHandler<NotifyAppointmentsByDistrictCommand, List<string>>),
                typeof(NotifyAppointmentsByDistrictCommandHandler));

            services.AddScoped(
                typeof(IRequestHandler<GetAppointmentsCalendarByDistrictQuery, AppointmentCalendarResponse>),
                provider => new GetAppointmentsCalendarByDistrictQueryHandler(
                    mediator: provider.GetRequiredService<IMediator>(),
                    credentialCache: provider.GetRequiredService<ICache<Credential>>(),
                    tokenCache: provider.GetRequiredService<ICache<string>>(),
                    statesCache: provider.GetRequiredService<ICache<StatesResponse>>(),
                    districtsCache: provider.GetRequiredService<ICache<DistrictsResponse>>(),
                    appointmentsCache: provider.GetRequiredService<ICache<AppointmentCalendarResponse>>(),
                    appointmentClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));

            services.AddScoped(
                typeof(IRequestHandler<NotifyAppointmentsByPincodeCommand, bool>),
                typeof(NotifyAppointmentsByPincodeCommandHandler));

            services.AddScoped(
                typeof(IRequestHandler<GetAppointmentsCalendarByPincodeQuery, AppointmentCalendarResponse>),
                provider => new GetAppointmentsCalendarByPincodeQueryHandler(
                    credentialCache: provider.GetRequiredService<ICache<Credential>>(),
                    tokenCache: provider.GetRequiredService<ICache<string>>(),
                    appointmentsCache: provider.GetRequiredService<ICache<AppointmentCalendarResponse>>(),
                    appointmentClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));
        }
    }
}
