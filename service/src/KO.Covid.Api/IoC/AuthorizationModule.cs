namespace KO.Covid.Api.IoC
{
    using Autofac;
    using KO.Covid.Api.Authorization;

    public class AuthorizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SubscriberAuthorizationHandler>()
                .AsImplementedInterfaces()
                .SingleInstance();

            base.Load(builder);
        }
    }
}
