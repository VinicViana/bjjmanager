using BJJManager.Application.Common.Interfaces;

namespace BJJManager.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BjjManagerDbContext _context;

    public UnitOfWork(BjjManagerDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}
