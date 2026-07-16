using BJJManager.Application.Common.Models;
using BJJManager.Domain.Entities;

namespace BJJManager.Application.Common.Interfaces;

public interface ITrainingSessionRepository
{
    Task<TrainingSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TrainingSession>> GetByUserIdAsync(Guid userId, TrainingSessionFilter filter, CancellationToken cancellationToken);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(TrainingSession trainingSession, CancellationToken cancellationToken);
    void Remove(TrainingSession trainingSession);
}
