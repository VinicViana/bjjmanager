using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace BJJManager.WebApi.IntegrationTests.Common;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected readonly CustomWebApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Factory.EnsureDatabaseCreated();
    }

    protected record LoginResponse(string Token, DateTime ExpiresAtUtc, string UserName);

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(string? name = null, string password = "Secret6")
    {
        name ??= $"user-{Guid.NewGuid():N}";

        var client = Factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new { name, password });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new { name, password });
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        return client;
    }
}
