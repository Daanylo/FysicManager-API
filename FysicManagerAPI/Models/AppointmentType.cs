using System.Text.Json.Serialization;

namespace FysicManagerAPI.Models;

public class AppointmentType
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("color")]
    public string? Color { get; set; }
}