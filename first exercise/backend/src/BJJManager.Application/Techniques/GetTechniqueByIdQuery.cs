using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Techniques;

public record GetTechniqueByIdQuery(Guid Id) : IRequest<TechniqueDto>;

public class GetTechniqueByIdQueryHandler : IRequestHandler<GetTechniqueByIdQuery, TechniqueDto>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTechniqueByIdQueryHandler(ITechniqueRepository techniqueRepository, ICurrentUserService currentUserService)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TechniqueDto> Handle(GetTechniqueByIdQuery request, CancellationToken cancellationToken)
    {
        var technique = await _techniqueRepository.GetByIdAsync(request.Id, cancellationToken);

        if (technique is null || technique.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Technique '{request.Id}' was not found.");

        return technique.ToDto();
    }
}
