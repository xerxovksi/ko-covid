namespace KO.Covid.Api.IoC
{
    using Autofac;
    using Autofac.Core;
    using KO.Covid.Application;
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Geo;
    using KO.Covid.Application.Authorization;
    using System.Net.Http;
    using KO.Covid.Application.Subscriber;
    using FluentValidation;
    using KO.Covid.Application.LoadBalancers;
    using KO.Covid.Application.Models;

    public class RequestHandlerModule : Module
    {
        private readonly string cowinBaseAddress = null;

        public RequestHandlerModule(string cowinBaseAddress) =>
            this.cowinBaseAddress = cowinBaseAddress;

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestMediator>()
                .As<IRequestMediator>()
                .InstancePerLifetimeScope();

            builder.Register(_ => new HttpClient())
                .Named<HttpClient>("otpClient")
                .SingleInstance();

            builder.Register(_ => new HttpClient())
                .Named<HttpClient>("geoClient")
                .SingleInstance();

            builder.Register(_ => new HttpClient())
                .Named<HttpClient>("appointmentClient")
                .SingleInstance();

            this.RegisterLoadBalancers(builder);
            this.RegisterAuthorizationHandlers(builder);
            this.RegisterGeoHandlers(builder);
            this.RegisterAppointmentHandlers(builder);
            this.RegisterSubscriberHandlers(builder);

            base.Load(builder);
        }

        private void RegisterLoadBalancers(ContainerBuilder builder)
        {
            var threshold = 1000;

            builder.RegisterType<InternalTokenLoadBalancer>()
                .WithParameter("tokenType", TokenType.Internal)
                .WithParameter("threshold", threshold)
                .Named<ITokenLoadBalancer>("internalTokenLoadBalancer")
                .SingleInstance();

            builder.RegisterType<PublicTokenLoadBalancer>()
                .WithParameter("tokenType", TokenType.Public)
                .WithParameter("threshold", threshold)
                .Named<ITokenLoadBalancer>("publicTokenLoadBalancer")
                .SingleInstance();
        }

        private void RegisterAuthorizationHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<AddActiveUserCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetActiveUsersQueryHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RemoveInactiveUsersCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<GenerateOtpCommandHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "otpClient",
                        (_, context) => context.ResolveNamed<HttpClient>("otpClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();

            builder.RegisterType<ConfirmOtpCommandHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "otpClient",
                        (_, context) => context.ResolveNamed<HttpClient>("otpClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();

            builder.RegisterType<AddInternalTokenCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetInternalTokenQueryHandler>()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "loadBalancer",
                        (_, context) => context.ResolveNamed<ITokenLoadBalancer>("internalTokenLoadBalancer")))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<AddPublicTokenCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetPublicTokenQueryHandler>()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "loadBalancer",
                        (_, context) => context.ResolveNamed<ITokenLoadBalancer>("publicTokenLoadBalancer")))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetCredentialQueryHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<AddInternalTokenCommandValidator>()
                .As<IValidator<AddInternalTokenCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AddPublicTokenCommandValidator>()
                .As<IValidator<AddPublicTokenCommand>>()
                .InstancePerLifetimeScope();
        }

        private void RegisterGeoHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<GetStatesQueryHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "geoClient",
                        (_, context) => context.ResolveNamed<HttpClient>("geoClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();

            builder.RegisterType<GetDistrictsQueryHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "geoClient",
                        (_, context) => context.ResolveNamed<HttpClient>("geoClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();
        }

        private void RegisterAppointmentHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<GetAppointmentsByDistrictQueryHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "appointmentClient",
                        (_, context) => context.ResolveNamed<HttpClient>("appointmentClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();

            builder.RegisterType<GetAppointmentsCalendarByDistrictQueryHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "appointmentClient",
                        (_, context) => context.ResolveNamed<HttpClient>("appointmentClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();
        }

        private void RegisterSubscriberHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<GetSubscriberQueryHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetActiveSubscribersQueryHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<CreateSubscriberCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UpdateSubscriberCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<CreateSubscriberCommandValidator>()
                .As<IValidator<CreateSubscriberCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UpdateSubscriberCommandValidator>()
                .As<IValidator<UpdateSubscriberCommand>>()
                .InstancePerLifetimeScope();
        }
    }
}

