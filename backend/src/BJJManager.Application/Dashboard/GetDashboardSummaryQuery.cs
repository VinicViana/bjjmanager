using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Dashboard;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public record DashboardSummaryDto(string UserName, int TotalTrainings, int TotalTechniques);

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardSummaryQueryHandler(
        IUserRepository userRepository,
        ITrainingSessionRepository trainingSessionRepository,
        ITechniqueRepository techniqueRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _trainingSessionRepository = trainingSessionRepository;
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            throw new NotFoundException($"User '{userId}' was not found.");

        var totalTrainings = await _trainingSessionRepository.CountByUserIdAsync(userId, cancellationToken);
        var totalTechniques = await _techniqueRepository.CountByUserIdAsync(userId, cancellationToken);

        return new DashboardSummaryDto(user.Name, totalTrainings, totalTechniques);
    }
}
