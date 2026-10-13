using Microsoft.AspNetCore.Mvc.Testing;

namespace TerraPulse.Tests;

public class IndicadoresAmbientaisControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IndicadoresAmbientaisControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/indicadores-ambientais?pageNumber=1&pageSize=10";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}