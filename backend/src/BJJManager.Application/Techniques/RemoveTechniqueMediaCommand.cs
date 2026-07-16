using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Techniques;

public record RemoveTechniqueMediaCommand(Guid TechniqueId, Guid MediaId) : IRequest;

public class RemoveTechniqueMediaCommandHandler : IRequestHandler<RemoveTechniqueMediaCommand>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTechniqueMediaCommandHandler(
        ITechniqueRepository techniqueRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveTechniqueMediaCommand request, CancellationToken cancellationToken)
    {
        var technique = await _techniqueRepository.GetByIdAsync(request.TechniqueId, cancellationToken);

        if (technique is null || technique.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Technique '{request.TechniqueId}' was not found.");

        technique.RemoveMedia(request.MediaId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
