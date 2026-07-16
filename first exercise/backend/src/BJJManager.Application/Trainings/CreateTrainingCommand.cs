using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Trainings;

public record CreateTrainingCommand(
    DateOnly TrainingDate,
    string AcademyName,
    SelfEvaluation SelfEvaluation,
    string? Notes) : IRequest<TrainingSessionDto>;

public class CreateTrainingCommandValidator : AbstractValidator<CreateTrainingCommand>
{
    public CreateTrainingCommandValidator()
    {
        RuleFor(x => x.AcademyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SelfEvaluation).IsInEnum();
    }
}

public class CreateTrainingCommandHandler : IRequestHandler<CreateTrainingCommand, TrainingSessionDto>
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTrainingCommandHandler(
        ITrainingSessionRepository trainingSessionRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TrainingSessionDto> Handle(CreateTrainingCommand request, CancellationToken cancellationToken)
    {
        var session = new TrainingSession(
            _currentUserService.UserId,
            request.TrainingDate,
            request.AcademyName,
            request.SelfEvaluation,
            request.Notes);

        await _trainingSessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return session.ToDto();
    }
}
