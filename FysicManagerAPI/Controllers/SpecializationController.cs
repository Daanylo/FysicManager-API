using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/specialization")]
public class SpecializationController(ILogger<SpecializationController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<SpecializationController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var specializations = _context.Specializations.ToList();
        if (specializations == null || specializations.Count == 0)
        {
            _logger.LogWarning("No specializations found.");
            return NotFound("No specializations found.");
        }
        _logger.LogInformation("Fetched all specializations: {Specializations}", JsonSerializer.Serialize(specializations));
        return Ok(specializations);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var specialization = _context.Specializations.Find(id);
        if (specialization == null)
        {
            _logger.LogWarning("Specialization with ID {Id} not found.", id);
            return NotFound($"Specialization with ID {id} not found.");
        }
        _logger.LogInformation("Fetched specialization: {Specialization}", JsonSerializer.Serialize(specialization));
        return Ok(specialization);
    }

    [HttpGet("{id}/therapists")]
    public IActionResult GetTherapists(string id)
    {
        var specialization = _context.Specializations.Include(s => s.Therapists).FirstOrDefault(s => s.Id == id);
        if (specialization == null)
        {
            _logger.LogWarning("Specialization with ID {Id} not found.", id);
            return NotFound($"Specialization with ID {id} not found.");
        }
        var therapists = specialization.Therapists?.Select(t => t.ToDTO()).ToList();
        _logger.LogInformation("Fetched therapists for specialization {Id}: {Therapists}", id, JsonSerializer.Serialize(therapists));
        return Ok(therapists);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Specialization specialization)
    {
        if (specialization == null)
        {
            _logger.LogWarning("Attempted to create a null specialization.");
            return BadRequest("Specialization cannot be null.");
        }
        _context.Specializations.Add(specialization);
        _context.SaveChanges();
        _logger.LogInformation("Created specialization: {Specialization}", JsonSerializer.Serialize(specialization));
        return CreatedAtAction(nameof(Get), new { id = specialization.Id }, specialization);
    }
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Specialization updatedSpecialization)
    {
        if (updatedSpecialization == null)
        {
            _logger.LogError("Received invalid specialization data for update");
            _logger.LogInformation("Received specialization data: {SpecializationJson}", JsonSerializer.Serialize(updatedSpecialization));
            return BadRequest("Invalid specialization data");
        }
        var specialization = _context.Specializations.Find(id);
        if (specialization == null)
        {
            _logger.LogWarning("Specialization with ID {Id} not found for update.", id);
            return NotFound($"Specialization with ID {id} not found.");
        }
        
        if (!string.IsNullOrEmpty(updatedSpecialization.Name) && updatedSpecialization.Name != specialization.Name)
            specialization.Name = updatedSpecialization.Name;
        if (!string.IsNullOrEmpty(updatedSpecialization.Description) && updatedSpecialization.Description != specialization.Description)
            specialization.Description = updatedSpecialization.Description;
            
        _context.SaveChanges();
        _logger.LogInformation("Updated specialization: {Specialization}", JsonSerializer.Serialize(specialization));
        return Ok(new { Message = "Specialization updated successfully", Specialization = specialization });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var specialization = _context.Specializations.Find(id);
        if (specialization == null)
        {
            _logger.LogWarning("Specialization with ID {Id} not found for deletion.", id);
            return NotFound($"Specialization with ID {id} not found.");
        }
        _context.Specializations.Remove(specialization);
        _context.SaveChanges();
        _logger.LogInformation("Deleted specialization: {Specialization}", JsonSerializer.Serialize(specialization));
        return NoContent();
    }
}
