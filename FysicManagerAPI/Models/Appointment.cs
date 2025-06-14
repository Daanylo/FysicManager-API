using System.Text.Json.Serialization;

namespace FysicManagerAPI.Models;

public record Appointment
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("patientId")]
    public string? PatientId { get; set; }
    [JsonPropertyName("therapistId")]
    public string? TherapistId { get; set; }
    [JsonPropertyName("practiceId")]
    public string? PracticeId { get; set; }
    [JsonPropertyName("appointmentTypeId")]
    public string? AppointmentTypeId { get; set; }
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}