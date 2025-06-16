namespace FysicManagerAPI.Models.DTOs;

using System.Text.Json.Serialization;

public record AppointmentDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("patientId")]
    public required string PatientId { get; set; }

    [JsonPropertyName("therapistId")]
    public required string TherapistId { get; set; }
    [JsonPropertyName("practiceId")]
    public required string PracticeId { get; set; }

    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
