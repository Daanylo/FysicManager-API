using System.Text.Json;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FysicManagerAPI.Controllers;

public class PatientController(ILogger<PatientController> logger) : ControllerBase
{
    [HttpGet]
    [Route("api/patient/{id}")]
    public IActionResult GetPatient(int id)
    {
        // Simulate fetching patient data
        var patient = new { Id = id, Name = "John Doe", Age = 30 };

        return Ok(patient);
    }

    [HttpPost]
    [Route("api/patient")]
    public IActionResult CreatePatient([FromBody] dynamic patientData)
    {
        // Simulate creating a new patient
        logger.LogInformation("Creating a new patient with data: {PatientData}", (object)patientData);
        JsonElement patientJson = JsonSerializer.Deserialize<JsonElement>(patientData.ToString());
        Patient patient = new()
        {
            FirstName = patientJson.GetProperty("firstName").GetString(),
            LastName = patientJson.GetProperty("lastName").GetString(),
            Initials = patientJson.GetProperty("initials").GetString(),
            DateOfBirth = patientJson.GetProperty("dateOfBirth").GetDateTime(),
            Email = patientJson.GetProperty("email").GetString(),
            PhoneNumber = patientJson.GetProperty("phoneNumber").GetString(),
            Address = patientJson.GetProperty("address").GetString(),
            PostalCode = patientJson.GetProperty("postalCode").GetString(),
            City = patientJson.GetProperty("city").GetString(),
            Country = patientJson.GetProperty("country").GetString()
        };
        logger.LogInformation("Patient created: {@Patient}", patient);
        return CreatedAtAction(nameof(GetPatient), new { id = 1 }, patientData);
    }

    [HttpPut]
    [Route("api/patient/{id}")]
    public IActionResult UpdatePatient(string id, [FromBody] dynamic patientData)
    {
        // Simulate fetching the existing patient (in real code, fetch from DB)
        Patient patient = new()
        {
            Id = id,
            FirstName = "ExistingFirstName",
            LastName = "ExistingLastName",
            Initials = "E.F.",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Email = "existing@email.com",
            PhoneNumber = "1234567890",
            Address = "Existing Address",
            PostalCode = "1234AB",
            City = "Existing City",
            Country = "Existing Country"
        };

        logger.LogInformation("Updating patient with ID {Id} with data: {PatientData}", id, (object)patientData);
        JsonElement patientJson = JsonSerializer.Deserialize<JsonElement>(patientData.ToString());

        if (patientJson.TryGetProperty("firstName", out var firstNameProp))
            patient.FirstName = firstNameProp.GetString();
        if (patientJson.TryGetProperty("lastName", out var lastNameProp))
            patient.LastName = lastNameProp.GetString();
        if (patientJson.TryGetProperty("initials", out var initialsProp))
            patient.Initials = initialsProp.GetString();
        if (patientJson.TryGetProperty("dateOfBirth", out var dobProp) && dobProp.ValueKind == JsonValueKind.String && DateTime.TryParse(dobProp.GetString(), out var dob))
            patient.DateOfBirth = dob;
        if (patientJson.TryGetProperty("email", out var emailProp))
            patient.Email = emailProp.GetString();
        if (patientJson.TryGetProperty("phoneNumber", out var phoneProp))
            patient.PhoneNumber = phoneProp.GetString();
        if (patientJson.TryGetProperty("address", out var addressProp))
            patient.Address = addressProp.GetString();
        if (patientJson.TryGetProperty("postalCode", out var postalProp))
            patient.PostalCode = postalProp.GetString();
        if (patientJson.TryGetProperty("city", out var cityProp))
            patient.City = cityProp.GetString();
        if (patientJson.TryGetProperty("country", out var countryProp))
            patient.Country = countryProp.GetString();

        logger.LogInformation("Patient updated: {@Patient}", patient);
        return Ok(new { Message = "Patient updated successfully", Patient = patient });
    }
}