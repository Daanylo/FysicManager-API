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
            .Include(a => a.Practice)
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
        .Include(a => a.Practice)
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
        .Include(a => a.Practice)
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

    [HttpGet("{id}/patient")]
    public IActionResult GetPatient(string id)
    {
        var appointment = _context.Appointments
        .Include(a => a.Patient)
        .FirstOrDefault(a => a.Id == id);
        if (appointment == null || appointment.Patient == null)
        {
            _logger.LogWarning("Appointment with ID {Id} or its patient not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched patient for appointment {Id}: {PatientJson}", id, JsonSerializer.Serialize(appointment.Patient));
        return Ok(appointment.Patient.ToDTO());
    }

    [HttpGet("{id}/therapist")]
    public IActionResult GetTherapist(string id)
    {
        var appointment = _context.Appointments
        .Include(a => a.Therapist)
        .FirstOrDefault(a => a.Id == id);
        if (appointment == null || appointment.Therapist == null)
        {
            _logger.LogWarning("Appointment with ID {Id} or its therapist not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched therapist for appointment {Id}: {TherapistJson}", id, JsonSerializer.Serialize(appointment.Therapist));
        return Ok(appointment.Therapist.ToDTO());
    }

    [HttpGet("{id}/practice")]
    public IActionResult GetPractice(string id)
    {
        var appointment = _context.Appointments
        .Include(a => a.Practice)
        .FirstOrDefault(a => a.Id == id);
        if (appointment == null || appointment.Practice == null)
        {
            _logger.LogWarning("Appointment with ID {Id} or its practice not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched practice for appointment {Id}: {PracticeJson}", id, JsonSerializer.Serialize(appointment.Practice));
        return Ok(appointment.Practice.ToDTO());
    }

    [HttpPost]
    public IActionResult Create([FromBody] AppointmentSummaryDTO appointment)
    {
        if (appointment == null)
        {
            _logger.LogError("Received null appointment data");
            return BadRequest("Appointment data cannot be null");
        }
        Appointment app = ToAppointment(appointment);
        _context.Appointments.Add(app);
        _context.SaveChanges();
        _logger.LogInformation("Created new appointment: {AppointmentJson}", JsonSerializer.Serialize(appointment));
        return CreatedAtAction(nameof(Get), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] AppointmentSummaryDTO appointment)
    {
        if (appointment == null)
        {
            _logger.LogError("Received invalid appointment data for update");
            _logger.LogInformation("Received appointment data: {AppointmentJson}", JsonSerializer.Serialize(appointment));
            return BadRequest("Invalid appointment data");
        }
        var existingAppointment = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Therapist)
            .Include(a => a.AppointmentType)
            .Include(a => a.Practice)
        .FirstOrDefault(a => a.Id == id);
        if (existingAppointment == null)
        {
            _logger.LogWarning("Appointment with ID {Id} not found for update", id);
            return NotFound();
        }
        var existing = existingAppointment.ToSummaryDTO();
        if (!string.IsNullOrEmpty(appointment.Description) && appointment.Description != existing.Description)
            existing.Description = appointment.Description;
        if (!string.IsNullOrEmpty(appointment.PatientId) && appointment.PatientId != existing.PatientId)
            existing.PatientId = appointment.PatientId;
        if (!string.IsNullOrEmpty(appointment.TherapistId) && appointment.TherapistId != existing.TherapistId)
            existing.TherapistId = appointment.TherapistId;
        if (!string.IsNullOrEmpty(appointment.AppointmentTypeId) && appointment.AppointmentTypeId != existing.AppointmentTypeId)
            existing.AppointmentTypeId = appointment.AppointmentTypeId;
        if (!string.IsNullOrEmpty(appointment.PracticeId) && appointment.PracticeId != existing.PracticeId)
            existing.PracticeId = appointment.PracticeId;
        if (appointment.Time != default)
            existing.Time = appointment.Time;
        if (appointment.Duration != 0)
            existing.Duration = appointment.Duration;
        if (!string.IsNullOrEmpty(appointment.Notes))
            existing.Notes = appointment.Notes;
        var updatedAppointment = ToAppointment(existing);
        existingAppointment.Description = updatedAppointment.Description;
        existingAppointment.Patient = updatedAppointment.Patient;
        existingAppointment.Therapist = updatedAppointment.Therapist;
        existingAppointment.AppointmentType = updatedAppointment.AppointmentType;
        existingAppointment.Practice = updatedAppointment.Practice;
        existingAppointment.Time = updatedAppointment.Time;
        existingAppointment.Duration = updatedAppointment.Duration;
        existingAppointment.Notes = updatedAppointment.Notes;
        _context.SaveChanges();
        _logger.LogInformation("Updated appointment: {AppointmentJson}", JsonSerializer.Serialize(existing));
        return Ok(new { Message = "Appointment updated successfully", Appointment = updatedAppointment.ToDTO() });
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

    public Appointment ToAppointment(AppointmentSummaryDTO appointmentSummary)
    {
        return new Appointment
        {
            Id = appointmentSummary.Id,
            Description = appointmentSummary.Description,
            Patient = _context.Patients.FirstOrDefault(p => p.Id == appointmentSummary.PatientId) ?? throw new ArgumentException("Patient not found"),
            Therapist = _context.Therapists.FirstOrDefault(t => t.Id == appointmentSummary.TherapistId) ?? throw new ArgumentException("Therapist not found"),
            Practice = _context.Practices.FirstOrDefault(p => p.Id == appointmentSummary.PracticeId) ?? throw new ArgumentException("Practice not found"),
            AppointmentType = _context.AppointmentTypes.FirstOrDefault(at => at.Id == appointmentSummary.AppointmentTypeId) ?? throw new ArgumentException("Appointment type not found"),
            Time = appointmentSummary.Time,
            Duration = appointmentSummary.Duration,
            Notes = appointmentSummary.Notes
        };
    }
}
