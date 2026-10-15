using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Tests;

public class ResultTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ResultTests(CustomWebApplicationFactory factory)
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
    public async Task CreateResult_UnknownEnrolment_ReturnsNotFound()
    {
        // Arrange
        var token = await GetTokenAsync("organiser@raceday.co.za", "Organiser@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Results/enrolment/999999",
            new { finishTime = "01:30:00", position = 1, isDisqualified = false });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateResult_AsParticipant_ReturnsForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Results/enrolment/1",
            new { finishTime = "01:30:00", position = 1, isDisqualified = false });

        // Assert — participants cannot capture results
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyResults_AsParticipant_ReturnsOk()
    {
        // Arrange
        var token = await GetTokenAsync("participant@raceday.co.za", "Participant@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Results/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMyResults_AsOrganiser_ReturnsForbidden()
    {
        // Arrange
        var token = await GetTokenAsync("organiser@raceday.co.za", "Organiser@123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/Results/me");

        // Assert — organisers don't have personal results
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
