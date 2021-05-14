﻿namespace KO.Covid.Api.IoC
{
    using Autofac;
    using Autofac.Core;
    using KO.Covid.Application;
    using KO.Covid.Application.Appointment;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Geo;
    using KO.Covid.Application.Otp;
    using System.Net.Http;

    public class RequestHandlerModule : Module
    {
        private readonly string cowinBaseAddress = null;

        public RequestHandlerModule(string cowinBaseAddress) =>
            this.cowinBaseAddress = cowinBaseAddress;

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestMediator>()
                .As<IRequestMediator>()
                .SingleInstance();

            builder.Register(_ => new HttpClient())
                .Named<HttpClient>("otpClient")
                .SingleInstance();

            builder.Register(_ => new HttpClient())
                .Named<HttpClient>("geoClient")
                .SingleInstance();

            builder.Register(_ => new HttpClient())
                .Named<HttpClient>("appointmentClient")
                .SingleInstance();

            this.RegisterOtpHandlers(builder);
            this.RegisterGeoHandlers(builder);
            this.RegisterAppointmentHandlers(builder);

            base.Load(builder);
        }

        private void RegisterOtpHandlers(ContainerBuilder builder)
        {
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

            builder.RegisterType<GetAppointmentsByPincodeQueryHandler>()
                .AsImplementedInterfaces()
                .WithParameter(
                    new ResolvedParameter(
                        (parameter, _) => parameter.Name == "appointmentClient",
                        (_, context) => context.ResolveNamed<HttpClient>("appointmentClient")))
                .WithParameter("baseAddress", this.cowinBaseAddress)
                .InstancePerLifetimeScope();
        }
    }
}

