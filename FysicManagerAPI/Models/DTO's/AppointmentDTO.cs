namespace FysicManagerAPI.Models.DTOs;

using System.Text.Json.Serialization;

public record AppointmentDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("patient")]
    public required PatientSummaryDTO Patient { get; set; }

    [JsonPropertyName("therapist")]
    public required TherapistSummaryDTO Therapist { get; set; }

    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }

    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

public record AppointmentSummaryDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("patient")]
    public required PatientSummaryDTO Patient { get; set; }

    [JsonPropertyName("therapist")]
    public required TherapistSummaryDTO Therapist { get; set; }

    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }

    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}