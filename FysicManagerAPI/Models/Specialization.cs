namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;
using FysicManagerAPI.Models.DTOs;

public class Specialization
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string? Name { get; set; }    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonIgnore] // Prevent circular reference during JSON serialization
    public List<Therapist>? Therapists { get; set; }

    public SpecializationDTO ToDTO()
    {
        return new SpecializationDTO
        {
            Id = Id,
            Name = Name,
            Description = Description
        };
    }

}

