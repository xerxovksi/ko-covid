namespace KO.Covid.Api.IoC
{
    using Autofac;
    using Autofac.Core;
    using KO.Covid.Application;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Application.Models;
    using KO.Covid.Application.Otp;
    using MediatR;
    using System;
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

            this.RegisterOtpHandlers(builder);

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
        }
    }
}

