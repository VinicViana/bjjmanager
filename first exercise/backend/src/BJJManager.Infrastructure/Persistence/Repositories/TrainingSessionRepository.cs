using BJJManager.Application.Common.Interfaces;
using BJJManager.Application.Common.Models;
using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BJJManager.Infrastructure.Persistence.Repositories;

public class TrainingSessionRepository : ITrainingSessionRepository
{
    private readonly BjjManagerDbContext _context;

    public TrainingSessionRepository(BjjManagerDbContext context)
    {
        _context = context;
    }

    public Task<TrainingSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.TrainingSessions
            .Include(s => s.Media)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TrainingSession>> GetByUserIdAsync(
        Guid userId, TrainingSessionFilter filter, CancellationToken cancellationToken)
    {
        var sessions = await _context.TrainingSessions
            .Include(s => s.Media)
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);

        IEnumerable<TrainingSession> filtered = sessions;

        if (filter.Date is not null)
        {
            filtered = filtered.Where(s => s.TrainingDate == filter.Date);
        }
        else
        {
            if (filter.Year is not null)
                filtered = filtered.Where(s => s.TrainingDate.Year == filter.Year);

            if (filter.Month is not null)
                filtered = filtered.Where(s => s.TrainingDate.Month == filter.Month);
        }

        return filtered.OrderByDescending(s => s.TrainingDate).ToList();
    }

    public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _context.TrainingSessions.CountAsync(s => s.UserId == userId, cancellationToken);

    public async Task AddAsync(TrainingSession trainingSession, CancellationToken cancellationToken) =>
        await _context.TrainingSessions.AddAsync(trainingSession, cancellationToken);

    public void Remove(TrainingSession trainingSession) => _context.TrainingSessions.Remove(trainingSession);
}
