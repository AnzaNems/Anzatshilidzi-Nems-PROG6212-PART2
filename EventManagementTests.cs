using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Tests;

public class EventManagementTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EventManagementTests(CustomWebApplicationFactory factory)
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

    private object SampleEvent(string suffix) => new
    {
        name = $"Test Event {suffix}",
        description = "Automated test event",
        eventDate = DateTime.UtcNow.AddMonths(3),
        location = "Johannesburg",
        distanceKm = 21.0,
        eventType = "Run"
    };

    [Fact]
    public async Task GetAllEvents_NoAuth_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/Events");

        // Assert — events are public
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_AsOrganiser_ReturnsCreated()
    {
        // Arrange
        var token = await GetTokenAsync("organiser@raceday.co.za", "Organiser@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Events", SampleEvent(Guid.NewGuid().ToString()[..6]));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_AsParticipant_ReturnsForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Events", SampleEvent("blocked"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_NoToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/Events", SampleEvent("anon"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEvent_UnknownId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/Events/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
