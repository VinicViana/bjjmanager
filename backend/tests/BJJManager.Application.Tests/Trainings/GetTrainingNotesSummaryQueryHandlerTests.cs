using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Common.Models;
using BJJManager.Application.Trainings;
using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Trainings;

public class GetTrainingNotesSummaryQueryHandlerTests
{
    private readonly ITrainingSessionRepository _trainingSessionRepository = Substitute.For<ITrainingSessionRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly Guid _userId = Guid.NewGuid();

    private GetTrainingNotesSummaryQueryHandler CreateHandler()
    {
        _currentUserService.UserId.Returns(_userId);
        return new GetTrainingNotesSummaryQueryHandler(_trainingSessionRepository, _currentUserService);
    }

    [Fact]
    public async Task Handle_OnlyIncludesSessionsFromTheLastMonth()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var withinLastMonth1 = new TrainingSession(_userId, today.AddDays(-5), "Gracie Barra", SelfEvaluation.Good, null);
        var withinLastMonth2 = new TrainingSession(_userId, today.AddDays(-5), "Gracie Barra", SelfEvaluation.Excellent, null);
        var withinLastMonthOtherDay = new TrainingSession(_userId, today.AddDays(-1), "Alliance", SelfEvaluation.Average, null);
        var tooOld = new TrainingSession(_userId, today.AddMonths(-2), "Checkmat", SelfEvaluation.VeryBad, null);

        _trainingSessionRepository
            .GetByUserIdAsync(_userId, Arg.Any<TrainingSessionFilter>(), Arg.Any<CancellationToken>())
            .Returns(new List<TrainingSession> { withinLastMonth1, withinLastMonth2, withinLastMonthOtherDay, tooOld });

        var result = await CreateHandler().Handle(new GetTrainingNotesSummaryQuery(), CancellationToken.None);

        result.TotalSessions.Should().Be(3);
        result.DailyNotes.Should().HaveCount(2);

        var fiveDaysAgo = result.DailyNotes.Single(d => d.Date == today.AddDays(-5));
        fiveDaysAgo.SessionCount.Should().Be(2);
        fiveDaysAgo.AverageScore.Should().Be(4.5); // (Good=4 + Excellent=5) / 2

        var yesterday = result.DailyNotes.Single(d => d.Date == today.AddDays(-1));
        yesterday.SessionCount.Should().Be(1);
        yesterday.AverageScore.Should().Be(3); // Average=3

        result.OverallAverage.Should().Be(4); // (4 + 5 + 3) / 3
    }

    [Fact]
    public async Task Handle_ReturnsZeroAverageWhenNoSessionsInLastMonth()
    {
        _trainingSessionRepository
            .GetByUserIdAsync(_userId, Arg.Any<TrainingSessionFilter>(), Arg.Any<CancellationToken>())
            .Returns(new List<TrainingSession>());

        var result = await CreateHandler().Handle(new GetTrainingNotesSummaryQuery(), CancellationToken.None);

        result.TotalSessions.Should().Be(0);
        result.DailyNotes.Should().BeEmpty();
        result.OverallAverage.Should().Be(0);
    }
}
