namespace FysicManagerAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using FysicManagerAPI.Models;
using FysicManagerAPI.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/practice")]
public class PracticeController(ILogger<PracticeController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<PracticeController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var practices = _context.Practices.ToList();
        if (practices == null || practices.Count == 0)
        {
            _logger.LogInformation("No practices found");
            return NotFound("No practices found");
        }
        _logger.LogInformation("Fetched all practices: {PracticesJson}", JsonSerializer.Serialize(practices));
        return Ok(practices);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
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

    [HttpGet("{id}/therapists")]
    public IActionResult GetTherapists(string id)
    {
        var practice = _context.Practices.Include(p => p.Therapists).FirstOrDefault(p => p.Id == id);
        if (practice == null)
        {
            _logger.LogWarning("Practice with ID {Id} not found", id);
            return NotFound();
        }
        var therapists = practice.Therapists?.Select(t => t.ToDTO()).ToList();
        _logger.LogInformation("Fetched therapists for practice {Id}: {TherapistsJson}", id, JsonSerializer.Serialize(therapists));
        return Ok(therapists);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Practice practice)
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
        return CreatedAtAction(nameof(Get), new { id = practice.Id }, practice);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Practice practice)
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

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
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
}