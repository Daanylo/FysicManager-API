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
            Patient = Patient?.ToDTO() ?? new PatientDTO { 
                Id = "unknown", 
                FirstName = "Unknown", 
                LastName = "Patient", 
                DateOfBirth = DateTime.MinValue, 
                Email = "", 
                PhoneNumber = "" 
            },
            Therapist = Therapist?.ToDTO() ?? new TherapistDTO { 
                Id = "unknown", 
                Name = "Unknown Therapist", 
                Email = "", 
                PhoneNumber = "" 
            },
            Practice = Practice?.ToDTO() ?? new PracticeDTO { 
                Id = "unknown", 
                Name = "Unknown Practice", 
                Address = "", 
                PhoneNumber = "", 
                Email = "" 
            },
            AppointmentType = AppointmentType,
            Time = Time,
            Duration = Duration,
            Notes = Notes
        };
    }

}