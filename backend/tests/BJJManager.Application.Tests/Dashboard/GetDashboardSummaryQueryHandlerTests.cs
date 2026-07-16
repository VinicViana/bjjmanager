using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Dashboard;
using BJJManager.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BJJManager.Application.Tests.Dashboard;

public class GetDashboardSummaryQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ITrainingSessionRepository _trainingSessionRepository = Substitute.For<ITrainingSessionRepository>();
    private readonly ITechniqueRepository _techniqueRepository = Substitute.For<ITechniqueRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();

    [Fact]
    public async Task Handle_ReturnsWelcomeNameAndTotals()
    {
        var user = new User("Vinicius", "hash");
        var userId = user.Id;
        _currentUserService.UserId.Returns(userId);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _trainingSessionRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(4);
        _techniqueRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(7);

        var handler = new GetDashboardSummaryQueryHandler(
            _userRepository, _trainingSessionRepository, _techniqueRepository, _currentUserService);

        var result = await handler.Handle(new GetDashboardSummaryQuery(), CancellationToken.None);

        result.Should().Be(new DashboardSummaryDto("Vinicius", 4, 7));
    }
}
