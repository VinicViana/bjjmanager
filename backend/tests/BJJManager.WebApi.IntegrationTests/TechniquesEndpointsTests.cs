using System.Net;
using System.Net.Http.Json;
using BJJManager.Application.Techniques;
using BJJManager.WebApi.IntegrationTests.Common;
using FluentAssertions;
using Xunit;

namespace BJJManager.WebApi.IntegrationTests;

public class TechniquesEndpointsTests : IntegrationTestBase
{
    public TechniquesEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_WithNoSteps_ReturnsBadRequest()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/techniques", new
        {
            name = "Armbar",
            position = "Mount",
            description = "Classic armbar",
            steps = Array.Empty<string>()
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsOrderedSteps()
    {
        var client = await CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/techniques", new
        {
            name = "Armbar",
            position = "Mount",
            description = "Classic armbar",
            steps = new[] { "Break posture", "Isolate the arm" }
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<TechniqueDto>();

        var getResponse = await client.GetAsync($"/api/techniques/{created!.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<TechniqueDto>();

        fetched!.Steps.Select(s => s.Order).Should().Equal(1, 2);
    }

    [Fact]
    public async Task Delete_ForAnotherUsersTechnique_ReturnsNotFound()
    {
        var ownerClient = await CreateAuthenticatedClientAsync();
        var createResponse = await ownerClient.PostAsJsonAsync("/api/techniques", new
        {
            name = "Armbar",
            position = "Mount",
            description = "Classic armbar",
            steps = new[] { "Break posture" }
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TechniqueDto>();

        var otherClient = await CreateAuthenticatedClientAsync();

        var response = await otherClient.DeleteAsync($"/api/techniques/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
