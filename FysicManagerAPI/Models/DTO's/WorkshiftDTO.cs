namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public class WorkshiftDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; set; }

    [JsonPropertyName("therapist")]
    public required TherapistSummaryDTO Therapist { get; set; }

    [JsonPropertyName("practice")]
    public required PracticeSummaryDTO Practice { get; set; }
}

public class WorkshiftSummaryDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; set; }

    [JsonPropertyName("therapist")]
    public required TherapistSummaryDTO Therapist { get; set; }

    [JsonPropertyName("practice")]
    public required PracticeSummaryDTO Practice { get; set; }
}