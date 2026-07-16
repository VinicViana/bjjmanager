using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BJJManager.Domain.Tests;

public class TrainingSessionTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void UpdateDetails_ChangesEditableFields()
    {
        var session = new TrainingSession(UserId, new DateOnly(2026, 1, 10), "Gracie Barra", SelfEvaluation.Average, "Ok training");

        session.UpdateDetails(new DateOnly(2026, 1, 11), "Alliance", SelfEvaluation.Excellent, "Great training");

        session.TrainingDate.Should().Be(new DateOnly(2026, 1, 11));
        session.AcademyName.Should().Be("Alliance");
        session.SelfEvaluation.Should().Be(SelfEvaluation.Excellent);
        session.Notes.Should().Be("Great training");
    }

    [Fact]
    public void AddMedia_ThenRemoveMedia_LeavesMediaEmpty()
    {
        var session = new TrainingSession(UserId, new DateOnly(2026, 1, 10), "Gracie Barra", SelfEvaluation.Average, null);

        var media = session.AddMedia("roll.mp4", "https://example.com/roll.mp4", MediaType.Video);
        session.RemoveMedia(media.Id);

        session.Media.Should().BeEmpty();
    }
}
