using Microsoft.AspNetCore.Mvc.Testing;

namespace TerraPulse.Tests;

public class RecomendacoesSustentaveisControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RecomendacoesSustentaveisControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/recomendacoes-sustentaveis?pageNumber=1&pageSize=10";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}