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
        var patient = _context.Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            _logger.LogWarning("Patient with ID {Id} not found", id);
            return NotFound();
        }

        var appointments = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Practice)
            .Include(a => a.Therapist)
            .Where(a => a.Patient.Id == id)
            .ToList()
            .Select(a => a.ToDTO())
            .ToList();
        
        _logger.LogInformation("Fetched appointments for patient {Id}: {AppointmentsJson}", id, JsonSerializer.Serialize(appointments));
        return Ok(appointments);
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? searchQuery)
    {
        var query = _context.Patients.AsQueryable();
        _logger.LogInformation("Searching patients with query: {SearchQuery}", searchQuery);
        if (!string.IsNullOrEmpty(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            query = query.Where(p => (p.FirstName != null && p.FirstName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                                     (p.LastName != null && p.LastName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                                     (p.PhoneNumber != null && p.PhoneNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                                     (p.Email != null && p.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                                     (p.BSN != null && p.BSN.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                                     p.DateOfBirth.ToString("dd-MM-yyyy").Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }

        var patients = query.Select(p => p.ToDTO()).ToList();
        if (patients.Count == 0)
        {
            _logger.LogInformation("No patients found for search criteria");
            return NotFound("No patients found");
        }
        
        _logger.LogInformation("Fetched patients for search criteria: {PatientsJson}", JsonSerializer.Serialize(patients));
        return Ok(patients);
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
        if (patient == null)
        {
            _logger.LogError("Received invalid patient data for update");
            _logger.LogInformation("Received patient data: {PatientJson}", JsonSerializer.Serialize(patient));
            return BadRequest("Invalid patient data");
        }
        var existing = _context.Patients.Find(id);
        if (existing == null)
        {
            _logger.LogWarning("Patient with ID {Id} not found for update", id);
            return NotFound();
        }
        
        if (!string.IsNullOrEmpty(patient.FirstName) && patient.FirstName != existing.FirstName)
            existing.FirstName = patient.FirstName;
        if (!string.IsNullOrEmpty(patient.LastName) && patient.LastName != existing.LastName)
            existing.LastName = patient.LastName;
        if (!string.IsNullOrEmpty(patient.Initials) && patient.Initials != existing.Initials)
            existing.Initials = patient.Initials;
        if (patient.DateOfBirth != default && patient.DateOfBirth != existing.DateOfBirth)
            existing.DateOfBirth = patient.DateOfBirth;
        if (!string.IsNullOrEmpty(patient.Email) && patient.Email != existing.Email)
            existing.Email = patient.Email;
        if (!string.IsNullOrEmpty(patient.PhoneNumber) && patient.PhoneNumber != existing.PhoneNumber)
            existing.PhoneNumber = patient.PhoneNumber;
        if (!string.IsNullOrEmpty(patient.Address) && patient.Address != existing.Address)
            existing.Address = patient.Address;
        if (!string.IsNullOrEmpty(patient.PostalCode) && patient.PostalCode != existing.PostalCode)
            existing.PostalCode = patient.PostalCode;
        if (!string.IsNullOrEmpty(patient.City) && patient.City != existing.City)
            existing.City = patient.City;
        if (!string.IsNullOrEmpty(patient.Country) && patient.Country != existing.Country)
            existing.Country = patient.Country;
        if (!string.IsNullOrEmpty(patient.BSN) && patient.BSN != existing.BSN)
            existing.BSN = patient.BSN;
            
        _context.SaveChanges();
        _logger.LogInformation("Patient updated: {PatientJson}", JsonSerializer.Serialize(existing));
        return Ok(new { Message = "Patient updated successfully", Patient = existing.ToDTO() });
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