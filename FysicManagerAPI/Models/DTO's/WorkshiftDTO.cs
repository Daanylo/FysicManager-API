namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public record WorkshiftDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; set; }
    [JsonPropertyName("therapist")]
    public required TherapistDTO Therapist { get; set; }

    [JsonPropertyName("practice")]
    public required PracticeDTO Practice { get; set; }
}

public record WorkshiftSummaryDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; set; }

    [JsonPropertyName("therapistId")]
    public required string TherapistId { get; set; }

    [JsonPropertyName("practiceId")]
    public required string PracticeId { get; set; }
}