using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Common.Models;
using MediatR;

namespace BJJManager.Application.Trainings;

public record GetTrainingsQuery(DateOnly? Date, int? Month, int? Year) : IRequest<IReadOnlyList<TrainingSessionDto>>;

public class GetTrainingsQueryHandler : IRequestHandler<GetTrainingsQuery, IReadOnlyList<TrainingSessionDto>>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTrainingsQueryHandler(ITrainingSessionRepository trainingSessionRepository, ICurrentUserService currentUserService)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<TrainingSessionDto>> Handle(GetTrainingsQuery request, CancellationToken cancellationToken)
    {
        var filter = new TrainingSessionFilter(request.Date, request.Month, request.Year);

        var sessions = await _trainingSessionRepository.GetByUserIdAsync(_currentUserService.UserId, filter, cancellationToken);

        return sessions.Select(s => s.ToDto()).ToList();
    }
}
