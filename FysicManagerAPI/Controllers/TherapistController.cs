using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using FysicManagerAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/therapist")]
public class TherapistController(ILogger<TherapistController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<TherapistController> _logger = logger;
    private readonly AppDbContext _context = context;    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var therapists = _context.Therapists
        .Include(t => t.Specializations)
        .Include(t => t.Practices)
        // Don't include Workshifts here to avoid circular references
        .Select(t => t.ToDTO()).ToList();
        if (therapists == null || therapists.Count == 0)
        {
            _logger.LogInformation("No therapists found");
            return NotFound("No therapists found");
        }
        _logger.LogInformation("Fetched all therapists: {TherapistsJson}", JsonSerializer.Serialize(therapists));
        return Ok(therapists);
    }    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var therapist = _context.Therapists
        .Include(t => t.Specializations)
        .Include(t => t.Practices)
        // Don't include Workshifts here to avoid circular references
        .FirstOrDefault(t => t.Id == id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched therapist: {TherapistJson}", JsonSerializer.Serialize(therapist));
        return Ok(therapist.ToDTO());
    }
    [HttpGet("{id}/workshifts")]
    public IActionResult GetWorkshifts(string id)
    {
        var therapist = _context.Therapists.FirstOrDefault(t => t.Id == id);
        if (therapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found", id);
            return NotFound();
        }

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
    public IActionResult Create([FromBody] TherapistDTO therapist)
    {
        if (therapist == null)
        {
            _logger.LogError("Received null therapist data");
            return BadRequest("Therapist data cannot be null");
        }
        
        // Generate a new ID for the therapist if not provided
        if (string.IsNullOrEmpty(therapist.Id))
        {
            therapist = therapist with { Id = Guid.NewGuid().ToString() };
        }
        
        try
        {
            Therapist newTherapist = ToTherapist(therapist);
            _context.Therapists.Add(newTherapist);
            _context.SaveChanges();
            _logger.LogInformation("Created new therapist: {TherapistJson}", JsonSerializer.Serialize(therapist));
            return CreatedAtAction(nameof(Get), new { id = newTherapist.Id }, newTherapist.ToDTO());
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("Failed to create therapist: {Error}", ex.Message);
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] TherapistDTO therapist)
    {
        if (therapist == null)
        {
            _logger.LogError("Received invalid therapist data for update");
            _logger.LogInformation("Received therapist data: {TherapistJson}", JsonSerializer.Serialize(therapist));
            return BadRequest("Invalid therapist data");
        }
        var existingTherapist = _context.Therapists
        .Include(t => t.Specializations)
        .Include(t => t.Practices)
        .FirstOrDefault(t => t.Id == id);
        if (existingTherapist == null)
        {
            _logger.LogWarning("Therapist with ID {Id} not found for update", id);
            return NotFound();
        }
        var existing = existingTherapist.ToDTO();
        if (!string.IsNullOrEmpty(therapist.Name) && therapist.Name != existing.Name)
            existing.Name = therapist.Name;
        if (therapist.SpecializationIds != null && !therapist.SpecializationIds.SequenceEqual(existing.SpecializationIds ?? []))
            existing.SpecializationIds = therapist.SpecializationIds;
        if (!string.IsNullOrEmpty(therapist.PhoneNumber) && therapist.PhoneNumber != existing.PhoneNumber)
            existing.PhoneNumber = therapist.PhoneNumber;
        if (!string.IsNullOrEmpty(therapist.Email) && therapist.Email != existing.Email)
            existing.Email = therapist.Email;        if (therapist.PracticeIds != null && !therapist.PracticeIds.SequenceEqual(existing.PracticeIds ?? []))
            existing.PracticeIds = therapist.PracticeIds;
        var updatedTherapist = ToTherapist(existing);
        existingTherapist.Name = updatedTherapist.Name;
        existingTherapist.PhoneNumber = updatedTherapist.PhoneNumber;
        existingTherapist.Email = updatedTherapist.Email;
        existingTherapist.Specializations = updatedTherapist.Specializations;
        existingTherapist.Practices = updatedTherapist.Practices;
        _context.SaveChanges();
        _logger.LogInformation("Updated therapist: {TherapistJson}", JsonSerializer.Serialize(existing));
        return Ok(new { Message = "Therapist updated successfully", Therapist = updatedTherapist.ToDTO() });
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
    public Therapist ToTherapist(TherapistDTO therapistDTO)
    {
        var specializations = new List<Specialization>();
        if (therapistDTO.SpecializationIds != null && therapistDTO.SpecializationIds.Any())
        {
            specializations = _context.Specializations
                .Where(s => therapistDTO.SpecializationIds.Contains(s.Id))
                .ToList();

            // Check if all requested specializations were found
            var notFound = therapistDTO.SpecializationIds.Except(specializations.Select(s => s.Id));
            if (notFound.Any())
            {
                throw new ArgumentException($"Specialization(s) not found: {string.Join(", ", notFound)}");
            }
        }

        var practices = new List<Practice>();
        if (therapistDTO.PracticeIds != null && therapistDTO.PracticeIds.Any())
        {
            practices = [.. _context.Practices.Where(p => therapistDTO.PracticeIds.Contains(p.Id))];

            // Check if all requested practices were found
            var notFound = therapistDTO.PracticeIds.Except(practices.Select(p => p.Id));
            if (notFound.Any())
            {
                throw new ArgumentException($"Practice(s) not found: {string.Join(", ", notFound)}");
            }
        }

        var therapist = new Therapist
        {
            Id = therapistDTO.Id,
            Name = therapistDTO.Name,
            PhoneNumber = therapistDTO.PhoneNumber,
            Email = therapistDTO.Email,
            Specializations = specializations,
            Practices = practices,
            Workshifts = [], // Initialize empty list
            Appointments = [] // Initialize empty list
        };

        return therapist;
    }
}
