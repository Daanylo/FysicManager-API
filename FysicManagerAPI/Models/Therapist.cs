namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public class Therapist
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("specializations")]
    public List<Specialization>? Specializations { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("practices")]
    public List<Practice>? Practices { get; set; } = [];
    [JsonPropertyName("workshifts")]
    public List<Workshift>? Workshifts { get; set; } = [];
    [JsonPropertyName("appointments")]
    public List<Appointment>? Appointments { get; set; } = [];

    

    public TherapistDTO ToDTO()
    {
        return new TherapistDTO
        {
            Id = Id,
            Name = Name,
            PhoneNumber = PhoneNumber,
            Email = Email,
        };
    }
}