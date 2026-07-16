using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Techniques;
using BJJManager.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Techniques;

public class CreateTechniqueCommandHandlerTests
{
    private readonly ITechniqueRepository _techniqueRepository = Substitute.For<ITechniqueRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly Guid _userId = Guid.NewGuid();

    private CreateTechniqueCommandHandler CreateHandler()
    {
        _currentUserService.UserId.Returns(_userId);
        return new CreateTechniqueCommandHandler(_techniqueRepository, _currentUserService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_CreatesTechniqueWithOrderedSteps()
    {
        var command = new CreateTechniqueCommand(
            "Armbar", "Mount", "Classic armbar", new[] { "Break posture", "Isolate the arm" });

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.Steps.Select(s => s.Order).Should().Equal(1, 2);
        await _techniqueRepository.Received(1).AddAsync(
            Arg.Is<Technique>(t => t.UserId == _userId),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
