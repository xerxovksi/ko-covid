namespace KO.Covid.Application
{
    using System;

    public static class Constants
    {
        public static readonly string ActiveCacheKey = "ActiveUsers";
        public static readonly string InternalTokensCacheKey = "InternalTokens";
        public static readonly string PublicTokensCacheKey = "PublicTokens";
        public static readonly string StatesCacheKey = "States";

        public static readonly TimeSpan ActiveCacheDuration = TimeSpan.FromHours(23);
        public static readonly TimeSpan TokenCacheDuration = TimeSpan.FromHours(18);
        public static readonly TimeSpan CredentialCacheDuration = TimeSpan.FromMinutes(2);
        public static readonly TimeSpan GeoCacheDuration = TimeSpan.FromDays(3);
        public static readonly TimeSpan AppointmentsCacheDuration = TimeSpan.FromSeconds(30);
    }
}
