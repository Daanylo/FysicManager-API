using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using FysicManagerAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/appointment")]
public class AppointmentController(ILogger<AppointmentController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<AppointmentController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var appointments = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Therapist)
            .Include(a => a.AppointmentType)
            .ToList()
            .Select(a => a.ToDTO())
            .ToList();
        if (appointments == null || appointments.Count == 0)
        {
            _logger.LogInformation("No appointments found");
            return NotFound("No appointments found");
        }
        _logger.LogInformation("Fetched all appointments: {AppointmentsJson}", JsonSerializer.Serialize(appointments));
        return Ok(appointments);
    }

    [HttpGet]
    public IActionResult GetFromTimespan([FromQuery] string? therapistId, [FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var query = _context.Appointments.AsQueryable();
        if (!string.IsNullOrEmpty(therapistId))
        {
            query = query.Where(a => a.Therapist.Id == therapistId);
        }
        if (start.HasValue)
        {
            query = query.Where(a => a.Time >= start.Value);
        }
        if (end.HasValue)
        {
            query = query.Where(a => a.Time <= end.Value);
        }
        var appointments = query
        .Include(a => a.AppointmentType)
        .Include(a => a.Patient)
        .Include(a => a.Therapist)
        .ToList()
        .Select(a => a.ToDTO())
        .ToList();
        _logger.LogInformation("Fetched appointments with filters therapistId={TherapistId}, start={Start}, end={End}: {AppointmentsJson}", therapistId, start, end, JsonSerializer.Serialize(appointments));
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var appointment = _context.Appointments
        .Include(a => a.AppointmentType)
        .Include(a => a.Patient)
        .Include(a => a.Therapist)
        .FirstOrDefault(a => a.Id == id)
        ?.ToDTO();
        if (appointment == null)
        {
            _logger.LogWarning("Appointment with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched appointment: {AppointmentJson}", JsonSerializer.Serialize(appointment));
        return Ok(appointment);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Appointment appointment)
    {
        if (appointment == null)
        {
            _logger.LogError("Received null appointment data");
            return BadRequest("Appointment data cannot be null");
        }
        _context.Appointments.Add(appointment);
        _context.SaveChanges();
        _logger.LogInformation("Created new appointment: {AppointmentJson}", JsonSerializer.Serialize(appointment));
        return CreatedAtAction(nameof(Get), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Appointment appointment)
    {
        if (appointment == null || appointment.Id != id)
        {
            _logger.LogError("Received invalid appointment data for update");
            return BadRequest("Invalid appointment data");
        }
        var existing = _context.Appointments.Find(id);
        if (existing == null)
        {
            _logger.LogWarning("Appointment with ID {Id} not found for update", id);
            return NotFound();
        }
        existing.Patient = appointment.Patient;
        existing.Therapist = appointment.Therapist;
        existing.AppointmentType = appointment.AppointmentType;
        existing.Time = appointment.Time;
        existing.Duration = appointment.Duration;
        existing.Notes = appointment.Notes;
        _context.SaveChanges();
        _logger.LogInformation("Updated appointment: {AppointmentJson}", JsonSerializer.Serialize(existing));
        return Ok(new { Message = "Appointment updated successfully", Appointment = existing });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var appointment = _context.Appointments.Find(id);
        if (appointment == null)
        {
            _logger.LogWarning("Appointment with ID {Id} not found for deletion", id);
            return NotFound();
        }
        _context.Appointments.Remove(appointment);
        _context.SaveChanges();
        _logger.LogInformation("Deleted appointment with ID {Id}: {AppointmentJson}", id, JsonSerializer.Serialize(appointment));
        return Ok(new { Message = "Appointment deleted successfully", Appointment = appointment });
    }
}
