using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

namespace FysicManagerAPI.Models;

public record Appointment
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("patient")]
    public required Patient Patient { get; set; }
    [JsonPropertyName("therapist")]
    public required Therapist Therapist { get; set; }
    [JsonPropertyName("practiceId")]
    public required Practice Practice { get; set; }
    [JsonPropertyName("appointmentType")]
    public required AppointmentType AppointmentType { get; set; }
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    public AppointmentDTO ToDTO()
    {
        return new AppointmentDTO
        {
            Id = Id,
            Patient = Patient.ToSummaryDTO(),
            Therapist = Therapist.ToSummaryDTO(),
            AppointmentType = AppointmentType,
            StartTime = Time,
            EndTime = Time.Add(Duration),
            Notes = Notes
        };
    }

    public AppointmentSummaryDTO ToSummaryDTO()
    {
        return new AppointmentSummaryDTO
        {
            Id = Id,
            Patient = Patient.ToSummaryDTO(),
            Therapist = Therapist.ToSummaryDTO(),
            AppointmentType = AppointmentType,
            StartTime = Time,
            EndTime = Time.Add(Duration),
            Notes = Notes
        };
    }

}