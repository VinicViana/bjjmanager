using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Techniques;

public record GetTechniquesQuery : IRequest<IReadOnlyList<TechniqueDto>>;

public class GetTechniquesQueryHandler : IRequestHandler<GetTechniquesQuery, IReadOnlyList<TechniqueDto>>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTechniquesQueryHandler(ITechniqueRepository techniqueRepository, ICurrentUserService currentUserService)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<TechniqueDto>> Handle(GetTechniquesQuery request, CancellationToken cancellationToken)
    {
        var techniques = await _techniqueRepository.GetByUserIdAsync(_currentUserService.UserId, cancellationToken);

        return techniques.Select(t => t.ToDto()).ToList();
    }
}
