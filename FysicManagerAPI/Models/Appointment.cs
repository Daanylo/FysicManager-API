using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

namespace FysicManagerAPI.Models;

public record Appointment
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("patient")]
    public required Patient Patient { get; set; }
    [JsonPropertyName("therapist")]
    public required Therapist Therapist { get; set; }
    [JsonPropertyName("practice")]
    public required Practice Practice { get; set; }
    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    public AppointmentDTO ToDTO()
    {
        return new AppointmentDTO
        {
            Id = Id,
            Description = Description,
            Patient = Patient?.ToDTO() ?? throw new ArgumentNullException(nameof(Patient), "Patient cannot be null"),
            Therapist = Therapist?.ToDTO() ?? throw new ArgumentNullException(nameof(Therapist), "Therapist cannot be null"),
            Practice = Practice?.ToDTO() ?? throw new ArgumentNullException(nameof(Practice), "Practice cannot be null"),
            AppointmentType = AppointmentType,
            Time = Time,
            Duration = Duration,
            Notes = Notes
        };
    }
    
    public AppointmentSummaryDTO ToSummaryDTO()
    {
        return new AppointmentSummaryDTO
        {
            Id = Id,
            Description = Description,
            PatientId = Patient?.Id ?? throw new ArgumentNullException(nameof(Patient), "Patient cannot be null"),
            TherapistId = Therapist?.Id ?? throw new ArgumentNullException(nameof(Therapist), "Therapist cannot be null"),
            PracticeId = Practice?.Id ?? throw new ArgumentNullException(nameof(Practice), "Practice cannot be null"),
            AppointmentTypeId = AppointmentType.Id,
            Time = Time,
            Duration = Duration,
            Notes = Notes
        };
    }

}