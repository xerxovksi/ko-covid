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

            this.RegisterAuthorizationHandlers(builder);
            this.RegisterGeoHandlers(builder);
            this.RegisterAppointmentHandlers(builder);
            this.RegisterSubscriberHandlers(builder);

            base.Load(builder);
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

            builder.RegisterType<RegisterDistrictTokenCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RegisterDistrictTokenCommandValidator>()
                .As<IValidator<RegisterDistrictTokenCommand>>()
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
            builder.RegisterType<CreateSubscriberCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<CreateSubscriberCommandValidator>()
                .As<IValidator<CreateSubscriberCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetActiveSubscribersQueryHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UpdateSubscriberCommandHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UpdateSubscriberCommandValidator>()
                .As<IValidator<UpdateSubscriberCommand>>()
                .InstancePerLifetimeScope();
        }
    }
}

