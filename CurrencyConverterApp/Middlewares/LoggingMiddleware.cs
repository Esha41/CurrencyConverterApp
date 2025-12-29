using System.Diagnostics;
using System.Security.Claims;

namespace CurrencyConverterApp.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(
            RequestDelegate next,
            ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            //correlate requests
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-Id"] = correlationId;

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var clientId = context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
            var httpMethod = context.Request.Method;
            var targetEndpoint = context.Request.Path;

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Request completed | CorrelationId: {CorrelationId} | ClientIP: {ClientIP} | ClientId: {ClientId} | HTTPMethod: {Method} | TargetEndpoint: {Path} | ResponseCode: {StatusCode} | ResponseTimeMs: {ElapsedMs}",
                correlationId,
                clientIp,
                clientId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
