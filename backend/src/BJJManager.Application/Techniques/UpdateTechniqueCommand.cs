using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Techniques;

public record UpdateTechniqueCommand(
    Guid Id,
    string Name,
    string Position,
    string Description,
    IReadOnlyList<string> Steps) : IRequest;

public class UpdateTechniqueCommandValidator : AbstractValidator<UpdateTechniqueCommand>
{
    public UpdateTechniqueCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Steps).NotEmpty().WithMessage("A technique must contain at least one step.");
        RuleForEach(x => x.Steps).NotEmpty().WithMessage("Step description must not be empty.");
    }
}

public class UpdateTechniqueCommandHandler : IRequestHandler<UpdateTechniqueCommand>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTechniqueCommandHandler(
        ITechniqueRepository techniqueRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTechniqueCommand request, CancellationToken cancellationToken)
    {
        var technique = await _techniqueRepository.GetByIdAsync(request.Id, cancellationToken);

        if (technique is null || technique.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Technique '{request.Id}' was not found.");

        technique.UpdateDetails(request.Name, request.Position, request.Description);
        technique.ReplaceSteps(request.Steps);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
