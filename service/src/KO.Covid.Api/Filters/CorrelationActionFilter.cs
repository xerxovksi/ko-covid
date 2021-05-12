namespace KO.Covid.Api.Filters
{
    using KO.Covid.Application;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class CorrelationActionFilter : IActionFilter
    {
        private const string CorrelationKey = "x-correlation-id";

        private readonly Correlator correlator = null;

        public CorrelationActionFilter(Correlator correlator) =>
            this.correlator = correlator;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            if (headers.ContainsKey(CorrelationKey))
            {
                this.correlator.Id = int.TryParse(headers[CorrelationKey], out var correlationId)
                    ? correlationId
                    : this.correlator.Id;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var headers = context.HttpContext.Response.Headers;
            if (headers.ContainsKey(CorrelationKey) == false)
            {
                headers.Add(
                    CorrelationKey,
                    this.correlator.Id.ToString());
            }
            else
            {
                headers[CorrelationKey] = this.correlator.Id.ToString();
            }
        }
    }
}
