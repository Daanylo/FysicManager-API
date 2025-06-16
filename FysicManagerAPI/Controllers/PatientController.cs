using System.Text.Json;
using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FysicManagerAPI.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientController(ILogger<PatientController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<PatientController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            _logger.LogWarning("Patient with ID {Id} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Fetched patient: {PatientJson}", JsonSerializer.Serialize(patient));
        return Ok(patient);
    }

    [HttpGet("{id}/appointments")]
    public IActionResult GetAppointments(string id)
    {
        var patient = _context.Patients.Include(p => p.Appointments).FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            _logger.LogWarning("Patient with ID {Id} not found", id);
            return NotFound();
        }
        var appointments = patient.Appointments?.Select(a => a.ToDTO()).ToList();
        _logger.LogInformation("Fetched appointments for patient {Id}: {AppointmentsJson}", id, JsonSerializer.Serialize(appointments));
        return Ok(appointments);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Patient patient)
    {
        if (patient == null)
        {
            _logger.LogError("Received null patient data");
            return BadRequest("Patient data cannot be null");
        }
        _context.Patients.Add(patient);
        _context.SaveChanges();
        _logger.LogInformation("Patient created: {PatientJson}", JsonSerializer.Serialize(patient));
        return CreatedAtAction(nameof(Get), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Patient patient)
    {
        if (patient == null || patient.Id != id)
        {
            _logger.LogError("Received invalid patient data for update");
            return BadRequest("Invalid patient data");
        }
        var existing = _context.Patients.Find(id);
        if (existing == null)
        {
            _logger.LogWarning("Patient with ID {Id} not found for update", id);
            return NotFound();
        }
        existing.FirstName = patient.FirstName;
        existing.LastName = patient.LastName;
        existing.Initials = patient.Initials;
        existing.DateOfBirth = patient.DateOfBirth;
        existing.Email = patient.Email;
        existing.PhoneNumber = patient.PhoneNumber;
        existing.Address = patient.Address;
        existing.PostalCode = patient.PostalCode;
        existing.City = patient.City;
        existing.Country = patient.Country;
        _context.SaveChanges();
        _logger.LogInformation("Patient updated: {PatientJson}", JsonSerializer.Serialize(existing));
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var patient = _context.Patients.Find(id);
        if (patient == null)
        {
            _logger.LogWarning("Patient with ID {Id} not found for deletion", id);
            return NotFound();
        }
        _context.Patients.Remove(patient);
        _context.SaveChanges();
        _logger.LogInformation("Deleted patient with ID {Id}: {PatientJson}", id, JsonSerializer.Serialize(patient));
        return Ok(new { Message = "Patient deleted successfully", Patient = patient });
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        var patients = _context.Patients.ToList();
        _logger.LogInformation("Fetched all patients: {PatientsJson}", JsonSerializer.Serialize(patients));
        return Ok(patients);
    }
}