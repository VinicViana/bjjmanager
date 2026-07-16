using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BJJManager.Infrastructure.Persistence.Repositories;

public class TechniqueRepository : ITechniqueRepository
{
    private readonly BjjManagerDbContext _context;

    public TechniqueRepository(BjjManagerDbContext context)
    {
        _context = context;
    }

    public Task<Technique?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.Techniques
            .Include(t => t.Steps)
            .Include(t => t.Media)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Technique>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await _context.Techniques
            .Include(t => t.Steps)
            .Include(t => t.Media)
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);

    public Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _context.Techniques.CountAsync(t => t.UserId == userId, cancellationToken);

    public async Task AddAsync(Technique technique, CancellationToken cancellationToken) =>
        await _context.Techniques.AddAsync(technique, cancellationToken);

    public void Remove(Technique technique) => _context.Techniques.Remove(technique);
}
