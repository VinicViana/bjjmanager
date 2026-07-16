using BJJManager.Domain.Entities;

namespace BJJManager.Application.Common.Interfaces;

public interface ITechniqueRepository
{
    Task<Technique?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Technique>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Technique technique, CancellationToken cancellationToken);
    void Remove(Technique technique);
}
