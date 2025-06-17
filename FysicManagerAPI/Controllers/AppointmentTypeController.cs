namespace FysicManagerAPI.Controllers;

using System.Text.Json;
using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/appointmenttype")]
public class AppointmentTypeController(ILogger<AppointmentTypeController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<AppointmentTypeController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var appointmentTypes = _context.AppointmentTypes.ToList();
        if (appointmentTypes == null || appointmentTypes.Count == 0)
        {
            _logger.LogInformation("No appointment types found");
            return NotFound("No appointment types found");
        }
        _logger.LogInformation("Fetched all appointment types: {AppointmentTypesJson}", JsonSerializer.Serialize(appointmentTypes));
        return Ok(appointmentTypes);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var appointmentType = _context.AppointmentTypes.Find(id);
        if (appointmentType == null)
        {
            _logger.LogWarning("Appointment type with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched appointment type data: {AppointmentTypeJson}", JsonSerializer.Serialize(appointmentType));
        return Ok(appointmentType);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AppointmentType appointmentType)
    {
        if (appointmentType == null)
        {
            _logger.LogWarning("Received null appointment type for creation");
            return BadRequest("Appointment type cannot be null");
        }

        _context.AppointmentTypes.Add(appointmentType);
        _context.SaveChanges();
        _logger.LogInformation("Created new appointment type: {AppointmentTypeJson}", JsonSerializer.Serialize(appointmentType));
        return CreatedAtAction(nameof(Get), new { id = appointmentType.Id }, appointmentType);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] AppointmentType appointmentType)
    {
        if (appointmentType == null || appointmentType.Id != id)
        {
            _logger.LogWarning("Received invalid appointment type for update");
            return BadRequest("Invalid appointment type data");
        }

        var existingAppointmentType = _context.AppointmentTypes.Find(id);
        if (existingAppointmentType == null)
        {
            _logger.LogWarning("Appointment type with ID {Id} not found for update", id);
            return NotFound();
        }

        existingAppointmentType.Name = appointmentType.Name;
        existingAppointmentType.Description = appointmentType.Description;
        _context.SaveChanges();
        _logger.LogInformation("Updated appointment type: {AppointmentTypeJson}", JsonSerializer.Serialize(existingAppointmentType));
        return Ok(existingAppointmentType);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var appointmentType = _context.AppointmentTypes.Find(id);
        if (appointmentType == null)
        {
            _logger.LogWarning("Appointment type with ID {Id} not found for deletion", id);
            return NotFound();
        }

        _context.AppointmentTypes.Remove(appointmentType);
        _context.SaveChanges();
        _logger.LogInformation("Deleted appointment type with ID {Id}", id);
        return NoContent();
    }
}
