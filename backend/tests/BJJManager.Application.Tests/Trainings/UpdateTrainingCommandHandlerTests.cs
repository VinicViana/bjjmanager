using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Trainings;
using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Trainings;

public class UpdateTrainingCommandHandlerTests
{
    private readonly ITrainingSessionRepository _trainingSessionRepository = Substitute.For<ITrainingSessionRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private UpdateTrainingCommandHandler CreateHandler() =>
        new(_trainingSessionRepository, _currentUserService, _unitOfWork);

    [Fact]
    public async Task Handle_WhenSessionBelongsToAnotherUser_ThrowsNotFoundException()
    {
        var owner = Guid.NewGuid();
        var session = new TrainingSession(owner, new DateOnly(2026, 1, 10), "Gracie Barra", SelfEvaluation.Good, null);
        _trainingSessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        _currentUserService.UserId.Returns(Guid.NewGuid());

        var command = new UpdateTrainingCommand(session.Id, new DateOnly(2026, 1, 11), "Alliance", SelfEvaluation.Excellent, null);

        var act = () => CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenSessionExistsAndOwned_UpdatesDetails()
    {
        var userId = Guid.NewGuid();
        var session = new TrainingSession(userId, new DateOnly(2026, 1, 10), "Gracie Barra", SelfEvaluation.Good, null);
        _trainingSessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        _currentUserService.UserId.Returns(userId);

        var command = new UpdateTrainingCommand(session.Id, new DateOnly(2026, 1, 11), "Alliance", SelfEvaluation.Excellent, "Updated");

        await CreateHandler().Handle(command, CancellationToken.None);

        session.AcademyName.Should().Be("Alliance");
        session.SelfEvaluation.Should().Be(SelfEvaluation.Excellent);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
