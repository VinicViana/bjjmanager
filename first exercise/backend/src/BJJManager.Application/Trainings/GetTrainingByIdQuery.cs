using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Trainings;

public record GetTrainingByIdQuery(Guid Id) : IRequest<TrainingSessionDto>;

public class GetTrainingByIdQueryHandler : IRequestHandler<GetTrainingByIdQuery, TrainingSessionDto>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTrainingByIdQueryHandler(ITrainingSessionRepository trainingSessionRepository, ICurrentUserService currentUserService)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TrainingSessionDto> Handle(GetTrainingByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _trainingSessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null || session.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Training session '{request.Id}' was not found.");

        return session.ToDto();
    }
}
