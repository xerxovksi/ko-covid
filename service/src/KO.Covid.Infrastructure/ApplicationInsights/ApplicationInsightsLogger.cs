namespace KO.Covid.Infrastructure.ApplicationInsights
{
    using KO.Covid.Application;
    using KO.Covid.Application.Contracts;
    using KO.Covid.Domain;
    using Microsoft.Extensions.Logging;
    using System;

    public class ApplicationInsightsLogger<T> : ITelemetryLogger<T>
    {
        private readonly ILogger<T> logger = null;
        private readonly Correlator correlator = null;

        public ApplicationInsightsLogger(ILogger<T> logger, Correlator correlator)
        {
            this.logger = logger;
            this.correlator = correlator;
        }

        public void LogInformation(string message, params object[] args) =>
            this.logger.LogInformation(correlator.Id, message.WithId(correlator.Id), args);

        public void LogInformation(Exception exception, string message, params object[] args) =>
            this.logger.LogInformation(correlator.Id, exception, message.WithId(correlator.Id), args);

        public void LogWarning(string message, params object[] args) =>
            this.logger.LogWarning(correlator.Id, message.WithId(correlator.Id), args);

        public void LogWarning(Exception exception, string message, params object[] args) =>
            this.logger.LogWarning(correlator.Id, exception, message.WithId(correlator.Id), args);

        public void LogError(string message, params object[] args) =>
            this.logger.LogError(correlator.Id, message.WithId(correlator.Id), args);

        public void LogError(Exception exception, string message, params object[] args) =>
            this.logger.LogError(correlator.Id, exception, message.WithId(correlator.Id), args);
    }
}
