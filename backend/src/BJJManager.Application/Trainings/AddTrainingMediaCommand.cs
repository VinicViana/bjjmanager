using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Enums;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Trainings;

public record AddTrainingMediaCommand(Guid TrainingId, string FileName, string FileUrl, MediaType MediaType) : IRequest<TrainingMediaDto>;

public class AddTrainingMediaCommandValidator : AbstractValidator<AddTrainingMediaCommand>
{
    public AddTrainingMediaCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.FileUrl).NotEmpty();
        RuleFor(x => x.MediaType).IsInEnum();
    }
}

public class AddTrainingMediaCommandHandler : IRequestHandler<AddTrainingMediaCommand, TrainingMediaDto>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddTrainingMediaCommandHandler(
        ITrainingSessionRepository trainingSessionRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TrainingMediaDto> Handle(AddTrainingMediaCommand request, CancellationToken cancellationToken)
    {
        var session = await _trainingSessionRepository.GetByIdAsync(request.TrainingId, cancellationToken);

        if (session is null || session.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Training session '{request.TrainingId}' was not found.");

        var media = session.AddMedia(request.FileName, request.FileUrl, request.MediaType);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return media.ToDto();
    }
}
