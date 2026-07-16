using BJJManager.Application.Common.Exceptions;
using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Enums;
using FluentValidation;
using MediatR;

namespace BJJManager.Application.Techniques;

public record AddTechniqueMediaCommand(Guid TechniqueId, string FileName, string FileUrl, MediaType MediaType) : IRequest<TechniqueMediaDto>;

public class AddTechniqueMediaCommandValidator : AbstractValidator<AddTechniqueMediaCommand>
{
    public AddTechniqueMediaCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.FileUrl).NotEmpty();
        RuleFor(x => x.MediaType).IsInEnum();
    }
}

public class AddTechniqueMediaCommandHandler : IRequestHandler<AddTechniqueMediaCommand, TechniqueMediaDto>
{
    private readonly ITechniqueRepository _techniqueRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddTechniqueMediaCommandHandler(
        ITechniqueRepository techniqueRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _techniqueRepository = techniqueRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TechniqueMediaDto> Handle(AddTechniqueMediaCommand request, CancellationToken cancellationToken)
    {
        var technique = await _techniqueRepository.GetByIdAsync(request.TechniqueId, cancellationToken);

        if (technique is null || technique.UserId != _currentUserService.UserId)
            throw new NotFoundException($"Technique '{request.TechniqueId}' was not found.");

        var media = technique.AddMedia(request.FileName, request.FileUrl, request.MediaType);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return media.ToDto();
    }
}
