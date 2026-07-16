using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Trainings;
using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Trainings;

public class CreateTrainingCommandHandlerTests
{
    private readonly ITrainingSessionRepository _trainingSessionRepository = Substitute.For<ITrainingSessionRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly Guid _userId = Guid.NewGuid();

    private CreateTrainingCommandHandler CreateHandler()
    {
        _currentUserService.UserId.Returns(_userId);
        return new CreateTrainingCommandHandler(_trainingSessionRepository, _currentUserService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_CreatesSessionOwnedByCurrentUser()
    {
        var command = new CreateTrainingCommand(new DateOnly(2026, 1, 10), "Gracie Barra", SelfEvaluation.Good, "Solid training");

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.AcademyName.Should().Be("Gracie Barra");
        await _trainingSessionRepository.Received(1).AddAsync(
            Arg.Is<TrainingSession>(s => s.UserId == _userId),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
