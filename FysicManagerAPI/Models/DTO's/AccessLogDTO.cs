namespace FysicManagerAPI.Models.DTO_s;

public class AccessLogDTO
{
    public string Id { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public int? ResponseSize { get; set; }
    public string? DataSummary { get; set; }
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public bool IsSuccessful { get; set; }
}

public static class AccessLogExtensions
{
    public static AccessLogDTO ToDTO(this AccessLog accessLog)
    {
        return new AccessLogDTO
        {
            Id = accessLog.Id,
            Timestamp = accessLog.Timestamp,
            IpAddress = accessLog.IpAddress,
            UserAgent = accessLog.UserAgent,
            HttpMethod = accessLog.HttpMethod,
            RequestPath = accessLog.RequestPath,
            QueryString = accessLog.QueryString,
            StatusCode = accessLog.StatusCode,
            ResponseTimeMs = accessLog.ResponseTimeMs,
            ResponseSize = accessLog.ResponseSize,
            DataSummary = accessLog.DataSummary,
            UserId = accessLog.UserId,
            SessionId = accessLog.SessionId,
            IsSuccessful = accessLog.IsSuccessful
        };
    }
}
