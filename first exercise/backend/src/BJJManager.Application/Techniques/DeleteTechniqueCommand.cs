using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using MediatR;

namespace BJJManager.Application.Techniques;

public record DeleteTechniqueCommand(Guid Id) : IRequest;

public class DeleteTechniqueCommandHandler : IRequestHandler<DeleteTechniqueCommand>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTechniqueCommandHandler(
        ITechniqueRepository techniqueRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTechniqueCommand request, CancellationToken cancellationToken)
    {
        var technique = await _techniqueRepository.GetByIdAsync(request.Id, cancellationToken);

        if (technique is null || technique.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Technique '{request.Id}' was not found.");

        _techniqueRepository.Remove(technique);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
