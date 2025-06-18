namespace FysicManagerAPI.Models.DTOs;

using System.Text.Json.Serialization;
using FysicManagerAPI.Controllers;

public record AppointmentDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("patient")]
    public required PatientDTO Patient { get; set; }

    [JsonPropertyName("therapist")]
    public required TherapistDTO Therapist { get; set; }
    [JsonPropertyName("practice")]
    public required PracticeDTO Practice { get; set; }

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
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("patientId")]
    public required string PatientId { get; set; }

    [JsonPropertyName("therapistId")]
    public required string TherapistId { get; set; }
    [JsonPropertyName("practiceId")]
    public required string PracticeId { get; set; }

    [JsonPropertyName("appointmentTypeId")]
    public required string AppointmentTypeId { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

}
