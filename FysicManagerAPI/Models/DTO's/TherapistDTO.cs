namespace FysicManagerAPI.Models.DTOs;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models;

public record TherapistDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("specializationIds")]
    public List<string>? SpecializationIds { get; set; } = [];
    [JsonPropertyName("practiceIds")]
    public List<string>? PracticeIds { get; set; } = [];
    [JsonPropertyName("workshiftIds")]
    public List<string>? WorkshiftIds { get; set; } = [];

}

public record TherapistSummaryDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}