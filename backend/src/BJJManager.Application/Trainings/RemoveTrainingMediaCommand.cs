using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Trainings;

public record RemoveTrainingMediaCommand(Guid TrainingId, Guid MediaId) : IRequest;

public class RemoveTrainingMediaCommandHandler : IRequestHandler<RemoveTrainingMediaCommand>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTrainingMediaCommandHandler(
        ITrainingSessionRepository trainingSessionRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveTrainingMediaCommand request, CancellationToken cancellationToken)
    {
        var session = await _trainingSessionRepository.GetByIdAsync(request.TrainingId, cancellationToken);

        if (session is null || session.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Training session '{request.TrainingId}' was not found.");

        session.RemoveMedia(request.MediaId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
