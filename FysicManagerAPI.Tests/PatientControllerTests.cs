using System.Net;
using System.Net.Http.Json;
using FysicManagerAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FysicManagerAPI.Tests;

public class PatientControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetPatient_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/patient/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var patient = await response.Content.ReadFromJsonAsync<object>();
        Assert.NotNull(patient);
    }

    [Fact]
    public async Task CreatePatient_ReturnsCreated()
    {
        var patient = new
        {
            firstName = "Jane",
            lastName = "Doe",
            initials = "J.D.",
            dateOfBirth = "1990-05-15T00:00:00",
            email = "jane.doe@example.com",
            phoneNumber = "+1234567890",
            address = "123 Main Street",
            postalCode = "12345",
            city = "Amsterdam",
            country = "Netherlands"
        };
        var response = await _client.PostAsJsonAsync("/api/patient", patient);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePatient_ReturnsOk()
    {
        var update = new
        {
            firstName = "Updated",
            lastName = "Patient"
        };
        var response = await _client.PutAsJsonAsync("/api/patient/1", update);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
