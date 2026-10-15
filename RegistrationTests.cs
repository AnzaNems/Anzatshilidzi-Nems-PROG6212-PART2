using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RaceDay.Tests;

public class RegistrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidParticipant_ReturnsOk()
    {
        // Arrange
        var dto = new
        {
            fullName = "Valid Participant",
            email = $"valid{Guid.NewGuid()}@test.com",
            password = "Password123!",
            role = "Participant"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var email = $"dup{Guid.NewGuid()}@test.com";
        var dto = new { fullName = "User", email, password = "Password123!", role = "Participant" };

        await _client.PostAsJsonAsync("/api/Auth/register", dto);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        // Arrange
        var dto = new
        {
            fullName = "Hacker",
            email = $"hack{Guid.NewGuid()}@test.com",
            password = "Password123!",
            role = "SuperAdmin"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_MissingEmail_ReturnsBadRequest()
    {
        // Arrange
        var dto = new { fullName = "No Email", email = "", password = "Password123!", role = "Participant" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShortPassword_ReturnsBadRequest()
    {
        // Arrange
        var dto = new
        {
            fullName = "Weak Pass",
            email = $"weak{Guid.NewGuid()}@test.com",
            password = "abc",
            role = "Participant"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
