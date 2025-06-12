namespace FysicManagerAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using FysicManagerAPI.Models;

public class PracticeController(ILogger<PracticeController> logger) : ControllerBase
{
    private readonly ILogger<PracticeController> _logger = logger;

    [HttpGet]
    [Route("api/practice/{id}")]
    public IActionResult GetPractice(string id)
    {
        // Simulate fetching practice data
        var practice = new Practice
        {
            Id = id,
            Name = "FysicManager Practice",
            Address = "123 Main St",
            PostalCode = "12345",
            City = "Amsterdam",
            Country = "Netherlands",
            PhoneNumber = "+31123456789",
            Email = "example@example.com",
            Website = "https://example.com",
            Color = "#FFFFFF"
        };
        _logger.LogInformation("Fetched practice data: {@Practice}", practice);
        return Ok(practice);
    }
    [HttpPost]
    [Route("api/practice")]
    public IActionResult CreatePractice([FromBody] Practice practice)
    {
        if (practice == null)
        {
            _logger.LogError("Received null practice data");
            return BadRequest("Practice data cannot be null");
        }

        // Simulate saving the practice to a database
        _logger.LogInformation("Creating new practice: {@Practice}", practice);

        Practice newPractice = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = practice.Name,
            Address = practice.Address,
            PostalCode = practice.PostalCode,
            City = practice.City,
            Country = practice.Country,
            PhoneNumber = practice.PhoneNumber,
            Email = practice.Email,
            Website = practice.Website,
            Color = practice.Color
        };

        if (string.IsNullOrEmpty(newPractice.Name) || string.IsNullOrEmpty(newPractice.Address))
        {
            _logger.LogError("Invalid practice data: {@Practice}", newPractice);
            return BadRequest("Practice name and address are required");
        }

        return CreatedAtAction(nameof(GetPractice), new { id = practice.Id }, practice);
    }
    [HttpPut]
    [Route("api/practice/{id}")]
    public IActionResult UpdatePractice(string id, [FromBody] Practice practice)
    {
        if (practice == null || practice.Id != id)
        {
            _logger.LogError("Received invalid practice data for update");
            return BadRequest("Invalid practice data");
        }

        // Simulate updating the practice in a database
        _logger.LogInformation("Updating practice with ID {Id}: {@Practice}", id, practice);
        return Ok(practice);
    }
    [HttpDelete]
    [Route("api/practice/{id}")]
    public IActionResult DeletePractice(string id)
    {
        // Simulate deleting the practice from a database
        _logger.LogInformation("Deleting practice with ID {Id}", id);
        return NoContent();
    }
    [HttpGet]
    [Route("api/practice")]
    public IActionResult GetAllPractices()
    {
        // Simulate fetching all practices
        var practices = new List<Practice>
        {
            new Practice { Id = "1", Name = "Practice One", Address = "Address One", PostalCode = "1111AA", City = "City One", Country = "Country One", PhoneNumber = "+31111111111", Email = "example@example.com", Website = "https://example.com", Color = "#FFFFFF" },
            new Practice { Id = "2", Name = "Practice Two", Address = "Address Two", PostalCode = "2222BB", City = "City Two", Country = "Country Two", PhoneNumber = "+31222222222", Email = "example@example.com", Website = "https://example.com", Color = "#FFFFFF" }
        };
        _logger.LogInformation("Fetched all practices: {@Practices}", practices);
        return Ok(practices);
    }
}