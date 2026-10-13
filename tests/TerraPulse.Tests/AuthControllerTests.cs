using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TerraPulse.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/auth/login";

        var json = """
        {
            "email": "admin@terrapulse.ai",
            "senha": "Terra@123"
        }
        """;

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}