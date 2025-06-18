namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public class Therapist
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("specializations")]    [JsonIgnore] // Prevent circular reference during JSON serialization
    public List<Specialization>? Specializations { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonIgnore] // Prevent circular reference during JSON serialization
    public List<Practice>? Practices { get; set; } = [];
    [JsonIgnore] // Prevent circular reference during JSON serialization
    public List<Workshift>? Workshifts { get; set; } = [];
    [JsonIgnore] // Prevent circular reference during JSON serialization
    public List<Appointment>? Appointments { get; set; } = [];

    

    public TherapistDTO ToDTO()
    {
        return new TherapistDTO
        {
            Id = Id,
            Name = Name,
            PhoneNumber = PhoneNumber,
            Email = Email,
            SpecializationIds = Specializations?.Select(s => s.Id).ToList() ?? [],
            PracticeIds = Practices?.Select(p => p.Id).ToList() ?? [],
            WorkshiftIds = Workshifts?.Select(w => w.Id).ToList() ?? [],
        };
    }
}