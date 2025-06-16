namespace FysicManagerAPI.Models;

using System.Text.Json.Serialization;

public class Workshift
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; set; }

    [JsonPropertyName("therapist")]
    public required Therapist Therapist { get; set; }

    [JsonPropertyName("practice")]
    public required Practice Practice { get; set; }

    public WorkshiftDTO ToDTO()
    {
        return new WorkshiftDTO
        {
            Id = Id,
            StartTime = StartTime,
            EndTime = EndTime,
            Practice = Practice.ToDTO()
        };
    }
}