using System.ComponentModel.DataAnnotations;

namespace FysicManagerAPI.Models;

public class AccessLog
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public string HttpMethod { get; set; } = string.Empty;
    
    public string RequestPath { get; set; } = string.Empty;
    
    public string? QueryString { get; set; }
    
    public int StatusCode { get; set; }
    
    public long ResponseTimeMs { get; set; }
    
    public int? ResponseSize { get; set; }
    
    public string? DataSummary { get; set; } // Brief description of what data was accessed
    
    public string? UserId { get; set; } // For future authentication implementation
    
    public string? SessionId { get; set; }
    
    public bool IsSuccessful => StatusCode >= 200 && StatusCode < 400;
}
