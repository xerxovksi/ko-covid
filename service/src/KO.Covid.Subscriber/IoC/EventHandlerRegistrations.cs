namespace KO.Covid.Subscriber.IoC
{
    using FluentValidation;
    using KO.Covid.Application;
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Authorization;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Geo;
    using KO.Covid.Application.LoadBalancers;
    using KO.Covid.Application.Models;
    using KO.Covid.Application.Subscriber;
    using KO.Covid.Domain.Entities;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public static class EventHandlerRegistrations
    {
        public static IServiceCollection AddHandlers(
            this IServiceCollection services,
            string baseAddress)
        {
            services.AddScoped<IEventMediator<bool>, EventMediator>();

            services.AddSingleton(_ => new HttpClient());

            services.AddLoadBalancers();
            services.AddAuthorizationHandlers();
            services.AddSubscriberHandlers();
            services.AddGeoHandlers(baseAddress);
            services.AddAppointmentHandlers(baseAddress);

            return services;
        }

        private static void AddLoadBalancers(this IServiceCollection services)
        {
            var threshold = 1000;

            services.AddSingleton<ITokenLoadBalancer>(
                _ => new InternalTokenLoadBalancer(TokenType.Internal, threshold));

            services.AddSingleton<ITokenLoadBalancer>(
                _ => new PublicTokenLoadBalancer(TokenType.Public, threshold));
        }

        private static void AddAuthorizationHandlers(this IServiceCollection services)
        {
            services.AddScoped(
                typeof(IRequestHandler<AddActiveUserCommand, bool>),
                typeof(AddActiveUserCommandHandler));

            services.AddScoped<IValidator<AddActiveUserCommand>>(
                _ => new AddActiveUserCommandValidator());

            services.AddScoped(
                typeof(IRequestHandler<GetActiveUsersQuery, HashSet<string>>),
                typeof(GetActiveUsersQueryHandler));

            services.AddScoped(
                typeof(IRequestHandler<RemoveInactiveUsersCommand, bool>),
                typeof(RemoveInactiveUsersCommandHandler));

            services.AddScoped(
                typeof(IRequestHandler<GetInternalTokenQuery, string>),
                provider => new GetInternalTokenQueryHandler(
                    loadBalancer: provider.GetServices<ITokenLoadBalancer>()
                        .First(instance => instance.TokenType.Equals(TokenType.Internal)),
                    tokenCache: provider.GetRequiredService<ICache<Dictionary<string, DateTime>>>(),
                    logger: provider.GetRequiredService<ITelemetryLogger<GetInternalTokenQueryHandler>>()));

            services.AddScoped(
                typeof(IRequestHandler<GetPublicTokenQuery, string>),
                provider => new GetPublicTokenQueryHandler(
                    loadBalancer: provider.GetServices<ITokenLoadBalancer>()
                        .First(instance => instance.TokenType.Equals(TokenType.Public)),
                    tokenCache: provider.GetRequiredService<ICache<Dictionary<string, DateTime>>>(),
                    logger: provider.GetRequiredService<ITelemetryLogger<GetPublicTokenQueryHandler>>()));

            services.AddScoped(
                typeof(IRequestHandler<RemoveInactiveTokensCommand, bool>),
                typeof(RemoveInactiveTokensCommandHandler));
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
                    mediator: provider.GetRequiredService<IMediator>(),
                    statesCache: provider.GetRequiredService<ICache<StatesResponse>>(),
                    geoClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));

            services.AddScoped(
                typeof(IRequestHandler<GetDistrictsQuery, DistrictsResponse>),
                provider => new GetDistrictsQueryHandler(
                    mediator: provider.GetRequiredService<IMediator>(),
                    districtsCache: provider.GetRequiredService<ICache<DistrictsResponse>>(),
                    geoClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));
        }

        private static void AddAppointmentHandlers(
            this IServiceCollection services,
            string baseAddress)
        {
            services.AddScoped(
                typeof(IRequestHandler<GetAppointmentsCalendarByDistrictQuery, AppointmentCalendarResponse>),
                provider => new GetAppointmentsCalendarByDistrictQueryHandler(
                    mediator: provider.GetRequiredService<IMediator>(),
                    appointmentsCache: provider.GetRequiredService<ICache<AppointmentCalendarResponse>>(),
                    appointmentClient: provider.GetRequiredService<HttpClient>(),
                    baseAddress: baseAddress));

            services.AddScoped(
                typeof(IRequestHandler<NotifyAppointmentsByDistrictCommand, List<string>>),
                typeof(NotifyAppointmentsByDistrictCommandHandler));
        }
    }
}
