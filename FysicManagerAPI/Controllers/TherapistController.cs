using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/therapist")]
public class TherapistController(ILogger<TherapistController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<TherapistController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet]
    public IActionResult GetAll()
    {
        var therapists = _context.Therapists.ToList();
        if (therapists == null || therapists.Count == 0)
        {
            _logger.LogInformation("No therapists found");
            return NotFound("No therapists found");
        }
        _logger.LogInformation("Fetched all therapists: {TherapistsJson}", JsonSerializer.Serialize(therapists));
        return Ok(therapists);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var therapist = _context.Therapists.Find(id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched therapist: {TherapistJson}", JsonSerializer.Serialize(therapist));
        return Ok(therapist);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Therapist therapist)
    {
        if (therapist == null)
        {
            _logger.LogError("Received null therapist data");
            return BadRequest("Therapist data cannot be null");
        }
        _context.Therapists.Add(therapist);
        _context.SaveChanges();
        _logger.LogInformation("Created new therapist: {TherapistJson}", JsonSerializer.Serialize(therapist));
        return CreatedAtAction(nameof(Get), new { id = therapist.Id }, therapist);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Therapist therapist)
    {
        if (therapist == null || therapist.Id != id)
        {
            _logger.LogError("Received invalid therapist data for update");
            return BadRequest("Invalid therapist data");
        }
        var existing = _context.Therapists.Find(id);
        if (existing == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found for update", id);
            return NotFound();
        }
        existing.Name = therapist.Name;
        existing.Specializations = therapist.Specializations;
        existing.PhoneNumber = therapist.PhoneNumber;
        existing.Email = therapist.Email;
        existing.Practices = therapist.Practices;
        _context.SaveChanges();
        _logger.LogInformation("Updated therapist: {TherapistJson}", JsonSerializer.Serialize(existing));
        return Ok(new { Message = "Therapist updated successfully", Therapist = existing });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var therapist = _context.Therapists.Find(id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found for deletion", id);
            return NotFound();
        }
        _context.Therapists.Remove(therapist);
        _context.SaveChanges();
        _logger.LogInformation("Deleted therapist with ID {Id}: {TherapistJson}", id, JsonSerializer.Serialize(therapist));
        return Ok(new { Message = "Therapist deleted successfully", Therapist = therapist });
    }
}
