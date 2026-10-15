using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Tests;

public class CategoryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoryTests(CustomWebApplicationFactory factory)
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
    public async Task GetCategories_ForSeededEvent_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/Categories/event/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_AsOrganiser_ReturnsOk()
    {
        // Arrange
        var token = await GetTokenAsync("organiser@raceday.co.za", "Organiser@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new { name = $"Veteran {Guid.NewGuid():N}", distanceKm = 42.2, entryFee = 200 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories/event/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_AsParticipant_ReturnsForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new { name = "Sneaky Category", distanceKm = 10.0, entryFee = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories/event/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_NoToken_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new { name = "Anon Category", distanceKm = 5.0, entryFee = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories/event/1", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
