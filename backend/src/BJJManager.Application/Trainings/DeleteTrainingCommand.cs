using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Trainings;

public record DeleteTrainingCommand(Guid Id) : IRequest;

public class DeleteTrainingCommandHandler : IRequestHandler<DeleteTrainingCommand>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTrainingCommandHandler(
        ITrainingSessionRepository trainingSessionRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTrainingCommand request, CancellationToken cancellationToken)
    {
        var session = await _trainingSessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null || session.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Training session '{request.Id}' was not found.");

        _trainingSessionRepository.Remove(session);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
