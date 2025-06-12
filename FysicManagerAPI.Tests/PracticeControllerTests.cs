using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FysicManagerAPI.Tests;

public class PracticeControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetPractice_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/practice/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var practice = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(practice.ValueKind == JsonValueKind.Object);
    }

    [Fact]
    public async Task CreatePractice_ReturnsCreated()
    {
        var practice = new
        {
            name = "Test Practice",
            address = "123 Main St",
            postalCode = "12345",
            city = "Amsterdam",
            country = "Netherlands",
            phoneNumber = "+31123456789",
            email = "example@example.com",
            website = "https://example.com",
            color = "#FFFFFF"
        };

        var response = await _client.PostAsJsonAsync("/api/practice", practice);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdPractice = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(practice.name, createdPractice.GetProperty("name").GetString());
        Assert.Equal(practice.address, createdPractice.GetProperty("address").GetString());
        Assert.Equal(practice.postalCode, createdPractice.GetProperty("postalCode").GetString());
        Assert.Equal(practice.city, createdPractice.GetProperty("city").GetString());
        Assert.Equal(practice.country, createdPractice.GetProperty("country").GetString());
        Assert.Equal(practice.phoneNumber, createdPractice.GetProperty("phoneNumber").GetString());
        Assert.Equal(practice.email, createdPractice.GetProperty("email").GetString());
        Assert.Equal(practice.website, createdPractice.GetProperty("website").GetString());
        Assert.Equal(practice.color, createdPractice.GetProperty("color").GetString());
    }

    [Fact]
    public async Task UpdatePractice_ReturnsOk()
    {
        var update = new
        {
            id = "1",
            name = "Updated Practice",
            address = "456 New St",
            postalCode = "67890",
            city = "Rotterdam",
            country = "Netherlands",
            phoneNumber = "+31234567890",
            email = "example@example.com",
            website = "https://example.com",
            color = "#000000"
        };
        var response = await _client.PutAsJsonAsync("/api/practice/1", update);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedPractice = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(update.name, updatedPractice.GetProperty("name").GetString());
        Assert.Equal(update.address, updatedPractice.GetProperty("address").GetString());
        Assert.Equal(update.postalCode, updatedPractice.GetProperty("postalCode").GetString());
        Assert.Equal(update.city, updatedPractice.GetProperty("city").GetString());
        Assert.Equal(update.country, updatedPractice.GetProperty("country").GetString());
        Assert.Equal(update.phoneNumber, updatedPractice.GetProperty("phoneNumber").GetString());
        Assert.Equal(update.email, updatedPractice.GetProperty("email").GetString());
        Assert.Equal(update.website, updatedPractice.GetProperty("website").GetString());
        Assert.Equal(update.color, updatedPractice.GetProperty("color").GetString());
    }

    [Fact]
    public async Task DeletePractice_ReturnsNoContent()
    {
        var response = await _client.DeleteAsync("/api/practice/1");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllPractices_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/practice");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var practices = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(practices.ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task CreatePractice_InvalidData_ReturnsBadRequest()
    {
        var practice = new
        {
            name = "", // Invalid name
            address = "123 Main St",
            postalCode = "12345",
            city = "Amsterdam",
            country = "Netherlands",
            phoneNumber = "+31123456789",
            email = "example@example.com",
            website = "https://example.com",
            color = "#FFFFFF"
        };
        var response = await _client.PostAsJsonAsync("/api/practice", practice);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}