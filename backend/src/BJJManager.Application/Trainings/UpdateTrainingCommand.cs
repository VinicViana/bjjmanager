using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Enums;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Trainings;

public record UpdateTrainingCommand(
    Guid Id,
    DateOnly TrainingDate,
    string AcademyName,
    SelfEvaluation SelfEvaluation,
    string? Notes) : IRequest;

public class UpdateTrainingCommandValidator : AbstractValidator<UpdateTrainingCommand>
{
    public UpdateTrainingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AcademyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SelfEvaluation).IsInEnum();
    }
}

public class UpdateTrainingCommandHandler : IRequestHandler<UpdateTrainingCommand>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTrainingCommandHandler(
        ITrainingSessionRepository trainingSessionRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTrainingCommand request, CancellationToken cancellationToken)
    {
        var session = await _trainingSessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null || session.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Training session '{request.Id}' was not found.");

        session.UpdateDetails(request.TrainingDate, request.AcademyName, request.SelfEvaluation, request.Notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
