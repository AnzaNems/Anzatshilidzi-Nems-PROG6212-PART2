using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Tests;

public class EnrolmentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EnrolmentTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync(string email, string password)
    {
        var res = await _client.PostAsJsonAsync("/api/Auth/login", new { email, password });
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.GetProperty("token").GetString()!;
    }

    private async Task<int> GetFirstCategoryIdAsync()
    {
        var json = await _client.GetStringAsync("/api/Categories/event/1");
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement[0].GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task Enrol_AsParticipant_ReturnsOk()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var categoryId = await GetFirstCategoryIdAsync();

        // Act
        var response = await _client.PostAsJsonAsync("/api/Enrolments/event/1", new { categoryId });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Enrol_AsOrganiser_ReturnsForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("organiser@raceday.co.za", "Organiser@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Enrolments/event/1", new { categoryId = 1 });

        // Assert — organisers cannot enrol
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Enrol_NoToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/Enrolments/event/1", new { categoryId = 1 });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyEnrolments_AsParticipant_ReturnsOk()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Enrolments/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEventEnrolments_AsParticipant_ReturnsForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Enrolments/event/1");

        // Assert — participants cannot view all enrolments
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
