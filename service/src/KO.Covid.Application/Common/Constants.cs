namespace KO.Covid.Application
{
    using System;

    public static class Constants
    {
        public static readonly TimeSpan ActiveCacheDuration = TimeSpan.FromDays(1);
        public static readonly TimeSpan TokenCacheDuration = TimeSpan.FromDays(1);
        public static readonly TimeSpan CredentialCacheDuration = TimeSpan.FromDays(1);
        public static readonly TimeSpan GeoCacheDuration = TimeSpan.FromDays(1);
        public static readonly TimeSpan AppointmentsCacheDuration = TimeSpan.FromMinutes(1);
    }
}
