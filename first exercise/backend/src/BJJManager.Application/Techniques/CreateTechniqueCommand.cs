using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Techniques;

public record CreateTechniqueCommand(
    string Name,
    string Position,
    string Description,
    IReadOnlyList<string> Steps) : IRequest<TechniqueDto>;

public class CreateTechniqueCommandValidator : AbstractValidator<CreateTechniqueCommand>
{
    public CreateTechniqueCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Steps).NotEmpty().WithMessage("A technique must contain at least one step.");
        RuleForEach(x => x.Steps).NotEmpty().WithMessage("Step description must not be empty.");
    }
}

public class CreateTechniqueCommandHandler : IRequestHandler<CreateTechniqueCommand, TechniqueDto>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTechniqueCommandHandler(
        ITechniqueRepository techniqueRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TechniqueDto> Handle(CreateTechniqueCommand request, CancellationToken cancellationToken)
    {
        var technique = new Technique(
            _currentUserService.UserId,
            request.Name,
            request.Position,
            request.Description,
            request.Steps);

        await _techniqueRepository.AddAsync(technique, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return technique.ToDto();
    }
}
