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

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var therapists = _context.Therapists.Select(t => t.ToDTO()).ToList();
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
        return Ok(therapist.ToDTO());
    }    [HttpGet("{id}/workshifts")]
    public IActionResult GetWorkshifts(string id)
    {
        var therapist = _context.Therapists.FirstOrDefault(t => t.Id == id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }

        // Load workshifts with all necessary related entities directly
        var workshifts = _context.Workshifts
            .Include(ws => ws.Practice)
            .Include(ws => ws.Therapist)
            .Where(ws => ws.Therapist.Id == id)
            .ToList()
            .Select(ws => ws.ToDTO())
            .ToList();

        _logger.LogInformation("Fetched workshifts for therapist {Id}: {WorkshiftsJson}", id, JsonSerializer.Serialize(workshifts));
        return Ok(workshifts);
    }

    [HttpGet("{id}/practices")]
    public IActionResult GetPractices(string id)
    {
        var therapist = _context.Therapists.Include(t => t.Practices).FirstOrDefault(t => t.Id == id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }
        var practices = therapist.Practices?.Select(p => p.ToDTO()).ToList();
        _logger.LogInformation("Fetched practices for therapist {Id}: {PracticesJson}", id, JsonSerializer.Serialize(practices));
        return Ok(practices);
    }

    [HttpGet("{id}/specializations")]
    public IActionResult GetSpecializations(string id)
    {
        var therapist = _context.Therapists.Include(t => t.Specializations).FirstOrDefault(t => t.Id == id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }
        var specializations = therapist.Specializations?.ToList();
        _logger.LogInformation("Fetched specializations for therapist {Id}: {SpecializationsJson}", id, JsonSerializer.Serialize(specializations));
        return Ok(specializations);
    }

    [HttpGet("{id}/appointments")]
    public IActionResult GetAppointments(string id)
    {
        var therapist = _context.Therapists.FirstOrDefault(t => t.Id == id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }

        // Load appointments with all necessary related entities directly
        var appointments = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Practice)
            .Include(a => a.Therapist)
            .Include(a => a.AppointmentType)
            .Where(a => a.Therapist.Id == id)
            .ToList();

        var appointmentDTOs = appointments.Select(a => a.ToDTO()).ToList();
        _logger.LogInformation("Fetched appointments for therapist {Id}: {AppointmentsJson}", id, JsonSerializer.Serialize(appointmentDTOs));
        return Ok(appointmentDTOs);
    }

    [HttpPost]
    public IActionResult Create(string id, [FromBody] Therapist therapist)
    {
        if (therapist == null)
        {
            _logger.LogError("Received null therapist data");
            return BadRequest("Therapist data cannot be null");
        }
        _context.Therapists.Add(therapist);
        _context.SaveChanges();
        _logger.LogInformation("Created new therapist: {TherapistJson}", JsonSerializer.Serialize(therapist));
        return CreatedAtAction(nameof(Get), therapist.ToDTO());
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
        return Ok(new { Message = "Therapist updated successfully", Therapist = existing.ToDTO() });
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
        return Ok(new { Message = "Therapist deleted successfully", Therapist = therapist.ToDTO() });
    }
}
