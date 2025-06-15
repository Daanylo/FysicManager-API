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
    [JsonPropertyName("practice")]
    public required PracticeSummaryDTO Practice { get; set; }

    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

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
    [JsonPropertyName("practice")]
    public required PracticeSummaryDTO Practice { get; set; }

    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}