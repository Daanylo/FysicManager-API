using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FysicManagerAPI.Middleware;

public class AccessLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AccessLoggingMiddleware> _logger;

    public AccessLoggingMiddleware(RequestDelegate next, ILogger<AccessLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        // Skip logging for certain paths (health checks, swagger, etc.)
        if (ShouldSkipLogging(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;
        
        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Capture request details
            var accessLog = new AccessLog
            {
                Timestamp = DateTime.UtcNow,
                IpAddress = GetClientIpAddress(context),
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                HttpMethod = context.Request.Method,
                RequestPath = context.Request.Path.Value ?? "",
                QueryString = context.Request.QueryString.Value,
                SessionId = context.Session?.Id
            };

            // Execute the request
            await _next(context);

            stopwatch.Stop();

            // Capture response details
            accessLog.StatusCode = context.Response.StatusCode;
            accessLog.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            accessLog.ResponseSize = (int)responseBody.Length;
            accessLog.DataSummary = GenerateDataSummary(context, responseBody);

            // Copy the response back to the original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            // Save to database (async, don't wait to avoid slowing down the response)
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = context.RequestServices.CreateScope();
                    var scopedDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    scopedDbContext.AccessLogs.Add(accessLog);
                    await scopedDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to log access to database");
                }
            });

            // Also log to standard logger for immediate visibility
            _logger.LogInformation(
                "API Access: {Method} {Path} - {StatusCode} ({ResponseTime}ms) from {IP}",
                accessLog.HttpMethod,
                accessLog.RequestPath,
                accessLog.StatusCode,
                accessLog.ResponseTimeMs,
                accessLog.IpAddress
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error in access logging middleware");
            context.Response.Body = originalBodyStream;
            throw;
        }
    }

    private static bool ShouldSkipLogging(PathString path)
    {
        var pathValue = path.Value?.ToLower() ?? "";
        return pathValue.Contains("/swagger") ||
               pathValue.Contains("/health") ||
               pathValue.Contains("/favicon") ||
               pathValue.Contains("/_framework") ||
               pathValue.Contains("/api/accesslog"); // Don't log access to access logs to avoid recursion
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP first (for load balancers/proxies)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static string? GenerateDataSummary(HttpContext context, MemoryStream responseBody)
    {
        try
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method.ToUpper();
            var statusCode = context.Response.StatusCode;

            // Don't log response content for non-successful requests
            if (statusCode < 200 || statusCode >= 400)
            {
                return $"{method} {path} - Status: {statusCode}";
            }

            // Reset stream position to read content
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseContent = Encoding.UTF8.GetString(responseBody.ToArray());

            // Parse JSON response to count items
            if (context.Response.ContentType?.Contains("application/json") == true && !string.IsNullOrWhiteSpace(responseContent))
            {
                try
                {
                    using var document = JsonDocument.Parse(responseContent);
                    var root = document.RootElement;

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        var count = root.GetArrayLength();
                        var entityType = ExtractEntityTypeFromPath(path);
                        return $"Retrieved {count} {entityType} records";
                    }
                    else if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("id", out _))
                    {
                        var entityType = ExtractEntityTypeFromPath(path);
                        return $"Retrieved single {entityType} record";
                    }
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, just return a generic summary
                }
            }

            return $"{method} {path}";
        }
        catch (Exception)
        {
            return $"{context.Request.Method} {context.Request.Path}";
        }
    }

    private static string ExtractEntityTypeFromPath(string path)
    {
        if (path.Contains("/patient")) return "patient";
        if (path.Contains("/therapist")) return "therapist";
        if (path.Contains("/practice")) return "practice";
        if (path.Contains("/appointment")) return "appointment";
        if (path.Contains("/specialization")) return "specialization";
        if (path.Contains("/workshift")) return "workshift";
        return "unknown";
    }
}
