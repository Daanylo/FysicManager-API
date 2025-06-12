using System.Text.Json.Serialization;

namespace FysicManagerAPI.Models;
public class AppointmentType
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}