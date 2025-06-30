namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public class Workshift
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; set; }    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; set; }

    [JsonIgnore] // Prevent circular reference during JSON serialization
    public required Therapist Therapist { get; set; }

    [JsonIgnore] // Prevent circular reference during JSON serialization
    public required Practice Practice { get; set; }public WorkshiftDTO ToDTO()
    {
        // Convert UTC times back to local time for client consumption
        var localStartTime = StartTime.Kind == DateTimeKind.Utc ? StartTime.ToLocalTime() : StartTime;
        var localEndTime = EndTime.Kind == DateTimeKind.Utc ? EndTime.ToLocalTime() : EndTime;
        
        return new WorkshiftDTO
        {
            Id = Id,
            StartTime = localStartTime,
            EndTime = localEndTime,
            Therapist = Therapist?.ToDTO() ?? throw new InvalidOperationException("Therapist cannot be null"),
            Practice = Practice?.ToDTO() ?? throw new InvalidOperationException("Practice cannot be null")
        };
    }
    
    public WorkshiftSummaryDTO ToSummaryDTO()
    {
        // Convert UTC times back to local time for client consumption
        var localStartTime = StartTime.Kind == DateTimeKind.Utc ? StartTime.ToLocalTime() : StartTime;
        var localEndTime = EndTime.Kind == DateTimeKind.Utc ? EndTime.ToLocalTime() : EndTime;
        
        return new WorkshiftSummaryDTO
        {
            Id = Id,
            StartTime = localStartTime,
            EndTime = localEndTime,
            TherapistId = Therapist?.Id ?? throw new InvalidOperationException("Therapist cannot be null"),
            PracticeId = Practice?.Id ?? throw new InvalidOperationException("Practice cannot be null")
        };
    }
}