using System.Net;
using System.Net.Http.Json;
using BJJManager.Application.Trainings;
using BJJManager.Domain.Enums;
using BJJManager.WebApi.IntegrationTests.Common;
using FluentAssertions;
using Xunit;

namespace BJJManager.WebApi.IntegrationTests;

public class TrainingsEndpointsTests : IntegrationTestBase
{
    public TrainingsEndpointsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_WithoutToken_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/trainings");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsTheCreatedSession()
    {
        var client = await CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/trainings", new
        {
            trainingDate = "2026-01-10",
            academyName = "Gracie Barra",
            selfEvaluation = SelfEvaluation.Good,
            notes = "Solid training"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<TrainingSessionDto>(JsonOptions);

        var getResponse = await client.GetAsync($"/api/trainings/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<TrainingSessionDto>(JsonOptions);
        fetched!.AcademyName.Should().Be("Gracie Barra");
    }

    [Fact]
    public async Task GetById_ForAnotherUsersSession_ReturnsNotFound()
    {
        var ownerClient = await CreateAuthenticatedClientAsync();
        var createResponse = await ownerClient.PostAsJsonAsync("/api/trainings", new
        {
            trainingDate = "2026-01-10",
            academyName = "Gracie Barra",
            selfEvaluation = SelfEvaluation.Good,
            notes = (string?)null
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TrainingSessionDto>(JsonOptions);

        var otherClient = await CreateAuthenticatedClientAsync();

        var response = await otherClient.GetAsync($"/api/trainings/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
