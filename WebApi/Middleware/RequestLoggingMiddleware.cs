using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var requestPath = context.Request.Path;
            var method = context.Request.Method;

            _logger.LogInformation($"İstek başladı: {method} {requestPath}");

            await _next(context);

            sw.Stop();
            _logger.LogInformation($"İstek tamamlandı: {method} {requestPath} - Süre: {sw.ElapsedMilliseconds}ms - Durum: {context.Response.StatusCode}");
        }
        catch
        {
            sw.Stop();
            throw;
        }
    }
}
