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
    [JsonPropertyName("appointmentTypeId")]
    public string? AppointmentTypeId { get; set; }
    [JsonPropertyName("appointmentDate")]
    public DateTime AppointmentDate { get; set; }
    [JsonPropertyName("startTime")]
    public TimeSpan StartTime { get; set; }
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}