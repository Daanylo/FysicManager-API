namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public class Practice
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("therapists")]
    public List<Therapist>? Therapists { get; set; } = [];

    public PracticeDTO ToDTO()
    {
        return new PracticeDTO
        {
            Id = Id,
            Name = Name,
            Address = Address,
            PostalCode = PostalCode,
            City = City,
            Country = Country,
            PhoneNumber = PhoneNumber,
            Email = Email,
            Website = Website,
            Color = Color,
            Therapists = Therapists?.Select(t => t.ToSummaryDTO()).ToList()
        };
    }

    public PracticeSummaryDTO ToSummaryDTO()
    {
        return new PracticeSummaryDTO
        {
            Id = Id,
            Name = Name,
            Address = Address,
            PostalCode = PostalCode,
            City = City,
            Country = Country,
            PhoneNumber = PhoneNumber
        };
    }
}