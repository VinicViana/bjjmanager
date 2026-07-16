using System.Net;
using System.Net.Http.Json;
using BJJManager.WebApi.IntegrationTests.Common;
using FluentAssertions;
using Xunit;

namespace BJJManager.WebApi.IntegrationTests;

public class AuthEndpointsTests : IntegrationTestBase
{
    public AuthEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        var client = Factory.CreateClient();
        var name = $"user-{Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("/api/auth/register", new { name, password = "Secret6" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithShortPassword_ReturnsBadRequest()
    {
        var client = Factory.CreateClient();
        var name = $"user-{Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("/api/auth/register", new { name, password = "abc" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        var client = Factory.CreateClient();
        var name = $"user-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/auth/register", new { name, password = "Secret6" });

        var response = await client.PostAsJsonAsync("/api/auth/login", new { name, password = "Secret6" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        body!.Token.Should().NotBeNullOrEmpty();
        body.UserName.Should().Be(name);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var name = $"user-{Guid.NewGuid():N}";
        await client.PostAsJsonAsync("/api/auth/register", new { name, password = "Secret6" });

        var response = await client.PostAsJsonAsync("/api/auth/login", new { name, password = "WrongPassword" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
