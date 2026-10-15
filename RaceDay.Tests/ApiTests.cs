using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Tests;

public class ApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiTests(CustomWebApplicationFactory factory)
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

    [Fact]
    public async Task Register_ValidParticipant_ReturnsOk()
    {
        var dto = new { fullName = "Test", email = $"test{Guid.NewGuid()}@a.com", password = "Password123!", role = "Participant" };
        var res = await _client.PostAsJsonAsync("/api/Auth/register", dto);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Login_UnapprovedOrganiser_Returns403()
    {
        // This proves the Part 1 Security Fix!
        var email = $"org{Guid.NewGuid()}@a.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new { fullName = "Org", email, password = "Password123!", role = "Organiser" });
        var res = await _client.PostAsJsonAsync("/api/Auth/login", new { email, password = "Password123!" });
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_AsParticipant_Returns403()
    {
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var res = await _client.PostAsJsonAsync("/api/Events", new { name = "X", description = "X", eventDate = DateTime.UtcNow.AddDays(10), location = "X", distanceKm = 10, eventType = "Run" });
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_AsOrganiser_ReturnsCreated()
    {
        var token = await GetTokenAsync("organiser@raceday.co.za", "Organiser@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var res = await _client.PostAsJsonAsync("/api/Events", new { name = "X", description = "X", eventDate = DateTime.UtcNow.AddDays(10), location = "X", distanceKm = 10, eventType = "Run" });

        // 🚨 FIX APPLIED HERE: Changed from OK to Created
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task GetProfile_NoToken_Returns401()
    {
        var res = await _client.GetAsync("/api/Profile");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}