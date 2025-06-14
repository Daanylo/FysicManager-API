using System.Text.Json;
using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FysicManagerAPI.Controllers;

public class PatientController(ILogger<PatientController> logger, AppDbContext context) : ControllerBase
{
    private readonly ILogger<PatientController> _logger = logger;
    private readonly AppDbContext _context = context;

    [HttpGet]
    [Route("api/patient/{id}")]
    public IActionResult GetPatient(string id)
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

    [HttpPost]
    [Route("api/patient")]
    public IActionResult CreatePatient([FromBody] Patient patient)
    {
        if (patient == null)
        {
            _logger.LogError("Received null patient data");
            return BadRequest("Patient data cannot be null");
        }
        _context.Patients.Add(patient);
        _context.SaveChanges();
        _logger.LogInformation("Patient created: {PatientJson}", JsonSerializer.Serialize(patient));
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }

    [HttpPut]
    [Route("api/patient/{id}")]
    public IActionResult UpdatePatient(string id, [FromBody] Patient patient)
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

    [HttpDelete]
    [Route("api/patient/{id}")]
    public IActionResult DeletePatient(string id)
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

    [HttpGet]
    [Route("api/patient")]
    public IActionResult GetAllPatients()
    {
        var patients = _context.Patients.ToList();
        _logger.LogInformation("Fetched all patients: {PatientsJson}", JsonSerializer.Serialize(patients));
        return Ok(patients);
    }
}