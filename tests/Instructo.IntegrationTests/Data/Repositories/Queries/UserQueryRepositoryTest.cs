using System.Net;
using System.Net.Http.Headers;

namespace Instructo.IntegrationTests.Data.Repositories.Queries;

[Collection("Integration Tests")]
public class UserEndpointsTests : IntegrationTestBase
{
    private readonly AuthenticationHelper _authHelper;

    public UserEndpointsTests(IntegrationTestFixture fixture) : base(fixture)
    {
        _authHelper = new AuthenticationHelper(fixture);
    }

    [Fact]
    public async Task GetUsers_AsIronMan_ReturnsUsersList()
    {
        // Arrange
        var token = await _authHelper.GetIronManTokenAsync();
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Add more specific assertions based on your API response format
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetUsers_AsStudent_ReturnsForbidden()
    {
        // Arrange
        var token = await _authHelper.GetJwtTokenAsync("student@test.com");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await Client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}