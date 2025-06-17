namespace FysicManagerAPI.Models.DTOs;

using System.Text.Json.Serialization;

public record SpecializationDTO
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}