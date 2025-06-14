namespace FysicManagerAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using FysicManagerAPI.Models;
using FysicManagerAPI.Data;
using System.Text.Json;

[ApiController]
public class PracticeController(ILogger<PracticeController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<PracticeController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet]
    [Route("api/practices")]
    public IActionResult GetPractices()
    {
        var practices = _context.Practices.ToList();
        if (practices == null || !practices.Any())
        {
            _logger.LogInformation("No practices found");
            return NotFound("No practices found");
        }
        _logger.LogInformation("Fetched all practices: {PracticesJson}", JsonSerializer.Serialize(practices));
        return Ok(practices);
    }

    [HttpGet]
    [Route("api/practice/{id}")]
    public IActionResult GetPractice(string id)
    {
        var practice = _context.Practices.Find(id);
        if (practice == null)
        {
            _logger.LogWarning("Practice with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched practice data: {PracticeJson}", JsonSerializer.Serialize(practice));
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
        if (string.IsNullOrEmpty(practice.Name) || string.IsNullOrEmpty(practice.Address))
        {
            _logger.LogError("Invalid practice data: {PracticeJson}", JsonSerializer.Serialize(practice));
            return BadRequest("Practice name and address are required");
        }
        _context.Practices.Add(practice);
        _context.SaveChanges();
        _logger.LogInformation("Created new practice: {PracticeJson}", JsonSerializer.Serialize(practice));
        return CreatedAtAction(nameof(GetPractice), new { id = practice.Id }, practice);
    }

    [HttpPut]
    [Route("api/practice/{id}")]
    public IActionResult UpdatePractice(string id, [FromBody] Practice practice)
    {
        if (practice == null)
        {
            _logger.LogError("Received invalid practice data for update");
            return BadRequest("Invalid practice data");
        }
        var existing = _context.Practices.Find(id);
        if (existing == null)
        {
            _logger.LogWarning("Practice with ID {Id} not found for update", id);
            return NotFound();
        }

        if (practice.Name != null) existing.Name = practice.Name;
        if (practice.Address != null) existing.Address = practice.Address;
        if (practice.PostalCode != null) existing.PostalCode = practice.PostalCode;
        if (practice.City != null) existing.City = practice.City;
        if (practice.Country != null) existing.Country = practice.Country;
        if (practice.PhoneNumber != null) existing.PhoneNumber = practice.PhoneNumber;
        if (practice.Email != null) existing.Email = practice.Email;
        if (practice.Website != null) existing.Website = practice.Website;
        if (practice.Color != null) existing.Color = practice.Color;

        _context.SaveChanges();
        _logger.LogInformation("Updated practice with ID {Id}: {PracticeJson}", id, JsonSerializer.Serialize(existing));
        return Ok(existing);
    }

    [HttpDelete]
    [Route("api/practice/{id}")]
    public IActionResult DeletePractice(string id)
    {
        var practice = _context.Practices.Find(id);
        if (practice == null)
        {
            _logger.LogWarning("Practice with ID {Id} not found for deletion", id);
            return NotFound();
        }
        _context.Practices.Remove(practice);
        _context.SaveChanges();
        _logger.LogInformation("Deleted practice with ID {Id}: {PracticeJson}", id, JsonSerializer.Serialize(practice));
        return Ok(new { Message = "Practice deleted successfully", Practice = practice });
    }

    [HttpGet]
    [Route("api/practice")]
    public IActionResult GetAllPractices()
    {
        var practices = _context.Practices.ToList();
        _logger.LogInformation("Fetched all practices: {PracticesJson}", JsonSerializer.Serialize(practices));
        return Ok(practices);
    }
}