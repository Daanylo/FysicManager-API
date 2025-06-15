using System.Text.Json.Serialization;
using System.Xml;
using FysicManagerAPI.Models.DTOs;

namespace FysicManagerAPI.Models;

public record Patient
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    [JsonPropertyName("initials")]
    public string? Initials { get; set; }
    [JsonPropertyName("bsn")]
    public string? BSN { get; set; }
    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }
    [JsonPropertyName("city")]
    public string? City { get; set; }
    [JsonPropertyName("country")]
    public string? Country { get; set; }
    [JsonPropertyName("appointments")]
    public List<Appointment>? Appointments { get; set; } = [];

    public PatientDTO ToDTO()
    {
        return new PatientDTO
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Initials = Initials,
            DateOfBirth = DateOfBirth,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Address = Address,
            PostalCode = PostalCode,
            City = City,
            Country = Country,
            Appointments = Appointments?.Select(a => a.ToSummaryDTO()).ToList()
        };
    }

    public PatientSummaryDTO ToSummaryDTO()
    {
        return new PatientSummaryDTO
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Initials = Initials,
            DateOfBirth = DateOfBirth,
            Email = Email,
            PhoneNumber = PhoneNumber
        };
    }
}