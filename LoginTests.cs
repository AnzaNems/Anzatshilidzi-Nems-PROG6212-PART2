using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RaceDay.Tests;

public class LoginTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_CorrectCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var dto = new { email = "participant@raceday.co.za", password = "Participant@123" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("token", body);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new { email = "participant@raceday.co.za", password = "WrongPassword!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new { email = "nobody@nowhere.com", password = "Password123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnapprovedOrganiser_ReturnsForbidden()
    {
        // Arrange — register an organiser (auto-unapproved)
        var email = $"org{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register",
            new { fullName = "Pending Org", email, password = "Password123!", role = "Organiser" });

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login",
            new { email, password = "Password123!" });

        // Assert — proves the Part 1 security fix
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
