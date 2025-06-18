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
        return new WorkshiftDTO
        {
            Id = Id,
            StartTime = StartTime,
            EndTime = EndTime,
            Therapist = Therapist?.ToDTO() ?? throw new InvalidOperationException("Therapist cannot be null"),
            Practice = Practice?.ToDTO() ?? throw new InvalidOperationException("Practice cannot be null")
        };
    }
}