using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Techniques;
using BJJManager.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Techniques;

public class UpdateTechniqueCommandHandlerTests
{
    private readonly ITechniqueRepository _techniqueRepository = Substitute.For<ITechniqueRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private UpdateTechniqueCommandHandler CreateHandler() =>
        new(_techniqueRepository, _currentUserService, _unitOfWork);

    [Fact]
    public async Task Handle_WhenTechniqueBelongsToAnotherUser_ThrowsNotFoundException()
    {
        var owner = Guid.NewGuid();
        var technique = new Technique(owner, "Armbar", "Mount", "Classic armbar", new[] { "Break posture" });
        _techniqueRepository.GetByIdAsync(technique.Id, Arg.Any<CancellationToken>()).Returns(technique);
        _currentUserService.UserId.Returns(Guid.NewGuid());

        var command = new UpdateTechniqueCommand(technique.Id, "Armbar", "Mount", "Updated", new[] { "Step 1" });

        var act = () => CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenOwned_ReplacesStepsAndRenumbers()
    {
        var userId = Guid.NewGuid();
        var technique = new Technique(userId, "Armbar", "Mount", "Classic armbar", new[] { "Break posture", "Isolate arm" });
        _techniqueRepository.GetByIdAsync(technique.Id, Arg.Any<CancellationToken>()).Returns(technique);
        _currentUserService.UserId.Returns(userId);

        var command = new UpdateTechniqueCommand(technique.Id, "Armbar", "Mount", "Updated desc", new[] { "New step" });

        await CreateHandler().Handle(command, CancellationToken.None);

        technique.Description.Should().Be("Updated desc");
        technique.Steps.Should().ContainSingle().Which.Description.Should().Be("New step");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
